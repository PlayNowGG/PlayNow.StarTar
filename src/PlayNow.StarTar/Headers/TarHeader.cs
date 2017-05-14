// PlayNow.StarTar
// Copyright 2017 PlayNow.GG
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using PlayNow.StarTar.FileModes;

namespace PlayNow.StarTar.Headers
{
    /// <summary>
    ///     Represents a Tar header for a single entry.
    /// </summary>
    public class TarHeader : ITarHeader, IReadOnlyTarHeader
    {
        /// <summary>
        ///     The UStar version.
        /// </summary>
        public const ushort Version = (0x30 << 8) | 0x30;

        // TODO: old GNU support. magic: "ustar  ": "With OLDGNU_MAGIC, uname and gname are valid, though the header is not truly POSIX conforming.""
        /// <summary>
        ///     The Ustar magic value.
        /// </summary>
        public const string Magic = "ustar";

        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0);
        private readonly byte[] _defaultFileSize = { 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30 };

        private string _checksum;
        private string _deviceMajor;
        private string _deviceMinor;

        private string _fileName;
        private string _fileNamePrefix;
        private byte[] _fileSize;
        private char _flag;
        private string _groupId;
        private string _groupName;
        private string _lastModification;
        private string _linkName;
        private string _magic;
        private string _userId;
        private string _userName;
        private ushort _version;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TarHeader" /> class.
        /// </summary>
        public TarHeader()
        {
            Mode = new FileModeGroup("777");
            UserId = "0000000";
            GroupId = "0000000";
        }

        private static void AssertLength(string value, string paramName, int size)
        {
            // Keep space for terminator.
            if (value != null && value.Length >= size)
                throw new ArgumentOutOfRangeException(paramName, value, $"The value cannot be longer than {size - 1}.");
        }

        private string SerializeNumber(string value, int length)
        {
            return (value ?? string.Empty).PadLeft(length, '0');
        }

        private void SerializeInternal(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            _magic = Magic;
            _version = Version;

            var length = TarBlock.Size;
            length -= stream.Write(_fileName, 100);
            length -= stream.Write(SerializeNumber(Mode.Buffer, 7), 8);
            length -= stream.Write(SerializeNumber(_userId, 7), 8);
            length -= stream.Write(SerializeNumber(_groupName, 7), 8);
            length -= stream.Write(_fileSize ?? _defaultFileSize, 12);
            length -= stream.Write(SerializeNumber(_lastModification, 11), 12);
            length -= stream.Write(_checksum, 8);
            length -= stream.Write(_flag);
            length -= stream.Write(_linkName, 100);

            length -= stream.Write(_magic, 6);
            length -= stream.Write(_version);
            length -= stream.Write(_userName, 32);
            length -= stream.Write(_groupName, 32);
            length -= stream.Write(_deviceMajor, 8);
            length -= stream.Write(_deviceMinor, 8);
            length -= stream.Write(_fileNamePrefix, 155);

            // Align to the 512 block.
            for (; length > 0; length--)
            {
                stream.WriteByte(0);
            }
        }

        /// <summary>
        ///     Deserializes the a <see cref="TarHeader" /> from the specified <paramref name="stream" />.
        /// </summary>
        /// <param name="stream">The stream to deserialize a tar header from.</param>
        /// <returns>The deserialized tar header or null if the stream has ended.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="stream" /> is null.</exception>
        public static TarHeader Deserialize(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            var header = DeserializeInternal(stream);

            // File ends in a double NULL block, read twice to make sure we're at the end.
            return header ?? DeserializeInternal(stream);
        }

        private static TarHeader DeserializeInternal(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            var length = 0;
            var header = new TarHeader
            {
                _fileName = stream.ReadString(100, ref length),
                Mode = new FileModeGroup(stream.ReadString(8, ref length)),
                _userId = stream.ReadString(8, ref length),
                _groupId = stream.ReadString(8, ref length),
                _fileSize = stream.ReadBytes(12, ref length),
                _lastModification = stream.ReadString(12, ref length),
                _checksum = stream.ReadString(8, ref length),
                _flag = stream.ReadChar(ref length),
                _linkName = stream.ReadString(100, ref length)
            };

            // If filename is empty, this is a null block
            if (string.IsNullOrEmpty(header._fileName))
            {
                length = TarBlock.RemainingBlockSize(length);

                for (; length > 0; length--)
                {
                    stream.ReadByte();
                }

                return null;
            }
            try
            {
                header._magic = stream.ReadString(6, ref length);
            }
            catch (EndOfStreamException)
            {
                return header;
            }

            // If the magic does not match, this is a pre-POSIX.1-1988
            if (header._magic != Magic)
            {
                if (PathHelper.EndsWithDirectorySeparator(header.Name))
                {
                    // A small helper for backwards compatibility
                    header.Flag = TarHeaderFlag.Directory;
                }

                // Skip the remainder of the block
                length = TarBlock.RemainingBlockSize(length);
                for (; length > 0; length--)
                {
                    stream.ReadByte();
                }
                return header;
            }

            header._version = stream.ReadUShort(ref length);
            header._userName = stream.ReadString(32, ref length);
            header._groupName = stream.ReadString(32, ref length);
            header._deviceMajor = stream.ReadString(8, ref length);
            header._deviceMinor = stream.ReadString(8, ref length);
            header._fileNamePrefix = stream.ReadString(155, ref length);

            // Skip the remainder of the block.
            length = TarBlock.RemainingBlockSize(length);
            for (; length > 0; length--)
            {
                stream.ReadByte();
            }

            return header;
        }

        #region Implementation of IReadOnlyTarHeader

        /// <summary>
        ///     Gets the file mode of this entry.
        /// </summary>
        IReadOnlyFileModeGroup IReadOnlyTarHeader.Mode => new ReadOnlyFileModeGroup(Mode);

        #endregion

        #region Implementation of ITarHeader

        /// <summary>
        ///     Gets or sets the name of the entry.
        /// </summary>
        public string Name
        {
            get => _fileName;
            set
            {
                AssertLength(value, nameof(value), 100);
                _fileName = value ?? string.Empty;
            }
        }

        /// <summary>
        ///     Gets or sets the file name prefix of this entry.
        /// </summary>
        public string FileNamePrefix
        {
            get => _fileNamePrefix;
            set
            {
                AssertLength(value, nameof(value), 155);
                _fileNamePrefix = value ?? string.Empty;
            }
        }

        // TODO: Mode values often appear in 0100777 (file) or 0040777 (directory) forms, but I cannot find any documentation for this. Stricking to 3 diget octal numbers for now.
        /// <summary>
        ///     Gets or sets the file mode of this entry.
        /// </summary>
        public IFileModeGroup Mode { get; set; }

        /// <summary>
        ///     Gets or sets the user identifier of the owning user of this entry. The value is an octal representation of the
        ///     group identifier.
        /// </summary>
        public string UserId
        {
            get => _userId;
            set
            {
                AssertLength(value, nameof(value), 8);
                _userId = (value ?? string.Empty).PadLeft(7, '0');
            }
        }

        /// <summary>
        ///     Gets or sets the group identifier of the owning group of this entry. The value is an octal representation of the
        ///     group identifier.
        /// </summary>
        public string GroupId
        {
            get => _groupId;
            set
            {
                AssertLength(value, nameof(value), 8);
                _groupId = (value ?? string.Empty).PadLeft(7, '0');
            }
        }

        /// <summary>
        ///     Gets or sets the size of the entry.
        /// </summary>
        public long? Size
        {
            get
            {
                if (_fileSize == null)
                    return null;

                // if the byte 0 is 0x80, byte 4-11 contain a big endian size value.
                // NOTE: Can't find docs on this, but it doesn't hurt either (0x80 is not in the lower ASCII table)
                if ((_fileSize[0] & 0x80) == 0x80)
                {
                    var setnet = BitConverter.ToInt64(_fileSize, 4);
                    return IPAddress.NetworkToHostOrder(setnet);
                }

                // Convert from octal number.
                var str = Encoding.ASCII.GetString(_fileSize).Trim('\0');

                if (string.IsNullOrEmpty(str))
                    return null;

                return Convert.ToInt64(str, 8);
            }
            set
            {
                if (value == null) _fileSize = null;
                else
                {
                    // Convert to octal string.
                    var str = Convert.ToString(value.Value, 8).PadLeft(11, '0');

                    // Not supporting the binary form as I can't find documentation on it. Error if the size is too long.
                    if (str.Length > 11)
                        throw new ArgumentOutOfRangeException(nameof(value));

                    // Convert the string to a NUL terminated byte array.
                    var buf = new byte[12];
                    Encoding.ASCII.GetBytes(str.ToCharArray(), 0, 11, buf, 0);
                    buf[11] = 0;

                    _fileSize = buf;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the last modification date of this entry.
        /// </summary>
        public DateTime? LastModification
        {
            get => string.IsNullOrEmpty(_lastModification)
                ? null
                : (DateTime?) Epoch.AddSeconds(Convert.ToInt64(_lastModification, 8));
            set => _lastModification = value == null
                ? null
                : Convert.ToString((long) (value.Value - Epoch).TotalSeconds, 8);
        }

        /// <summary>
        ///     Gets or sets the checksum of this header.
        /// </summary>
        public string Checksum
        {
            get => _checksum;
            set
            {
                AssertLength(value, nameof(value), 8);
                _checksum = value.PadLeft(6, '0');
            }
        }

        /// <summary>
        ///     Gets or sets the flags of this entry.
        /// </summary>
        public TarHeaderFlag Flag
        {
            get => (TarHeaderFlag) _flag;
            set => _flag = (char) value;
        }

        /// <summary>
        ///     Gets or sets the name of the link if <see cref="P:PlayNow.StarTar.Headers.ITarHeader.Flag" /> is set to an
        ///     appropriate value.
        /// </summary>
        public string LinkName
        {
            get => _linkName;
            set
            {
                AssertLength(value, nameof(value), 100);
                _linkName = value;
            }
        }

        /// <summary>
        ///     Gets or sets the name of the owning user of this entry.
        /// </summary>
        public string UserName
        {
            get => _userName;
            set
            {
                AssertLength(value, nameof(value), 32);
                _userName = value;
            }
        }

        /// <summary>
        ///     Gets or sets the name of the owning group of this entry.
        /// </summary>
        public string GroupName
        {
            get => _groupName;
            set
            {
                AssertLength(value, nameof(value), 32);
                _groupName = value;
            }
        }

        /// <summary>
        ///     Gets or sets the device major value of this entry.
        /// </summary>
        public string DeviceMajor
        {
            get => _deviceMajor;
            set
            {
                AssertLength(value, nameof(value), 8);
                _deviceMajor = value;
            }
        }

        /// <summary>
        ///     Gets or sets the device minor value of this entry.
        /// </summary>
        public string DeviceMinor
        {
            get => _deviceMinor;
            set
            {
                AssertLength(value, nameof(value), 8);
                _deviceMinor = value;
            }
        }

        /// <summary>
        ///     Computes a checksum for this instance and sets it to <see cref="P:PlayNow.StarTar.Headers.ITarHeader.Checksum" />.
        /// </summary>
        public void ComputeChecksum()
        {
            // Compute the hash based on a blank checksum.
            _checksum = "        ";

            // Serialize the header into a buffer.
            var buf = new byte[TarBlock.Size];
            using (var ms = new MemoryStream(buf))
            {
                SerializeInternal(ms);
            }

            // Sum the contents of the header.
            var hash = buf.Aggregate<byte, long>(0, (current, b) => unchecked(current + b));
            _checksum = Convert.ToString(hash, 8);
        }

        /// <summary>
        ///     Writes a serialized form of this header to the specified stream.
        /// </summary>
        /// <param name="stream">The stream to write the serialized header to.</param>
        /// <exception cref="System.ArgumentNullException">stream</exception>
        public void Serialize(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            // Compute the checksum before serializing.
            ComputeChecksum();
            SerializeInternal(stream);
        }

        #endregion
    }
}