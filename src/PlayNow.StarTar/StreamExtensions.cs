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
using System.Text;

namespace PlayNow.StarTar
{
    /// <summary>
    ///     Contains <see cref="Stream" /> extension methods.
    /// </summary>
    internal static class StreamExtensions
    {
        #region Writing

        /// <summary>
        ///     Writes the specified <paramref name="value" /> to the specified <paramref name="stream" />.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="length">The length of the value to write. Any aditional bytes are written as 0.</param>
        /// <returns>The number of written bytes.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="stream" /> is null.</exception>
        public static int Write(this Stream stream, string value, int length)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            var totalLength = length;

            if (value != null)
            {
                foreach (var c in value)
                {
                    stream.WriteByte((byte) c);
                    length--;
                }
            }

            for (; length > 0; length--)
            {
                stream.WriteByte(0);
            }

            return totalLength;
        }

        /// <summary>
        ///     Writes the specified <paramref name="value" /> to the specified <paramref name="stream" />.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>The number of written bytes.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="stream" /> is null.</exception>
        public static int Write(this Stream stream, char value)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            stream.WriteByte((byte) value);
            return 1;
        }

        /// <summary>
        ///     Writes the specified <paramref name="value" /> to the specified <paramref name="stream" />.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>The number of written bytes.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="stream" /> is null.</exception>
        public static int Write(this Stream stream, ushort value)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            stream.WriteByte((byte) ((value >> 8) & 0xff));
            stream.WriteByte((byte) (value & 0xff));

            return 2;
        }

        /// <summary>
        ///     Writes the specified <paramref name="value" /> to the specified <paramref name="stream" />.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="length">The length of the value to write. Any aditional bytes are written as 0.</param>
        /// <returns>The number of written bytes.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="stream" /> is null.</exception>
        public static int Write(this Stream stream, byte[] value, int length)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            if (value == null)
            {
                for (var i = 0; i < length; i++)
                {
                    stream.WriteByte(0);
                }
                return length;
            }

            stream.Write(value, 0, value.Length);

            for (var i = 0; i < length - value.Length; i++)
            {
                stream.WriteByte(0);
            }

            return length;
        }

        #endregion

        #region Reading

        /// <summary>
        ///     Reads a byte array from the specified <paramref name="stream" /> and adds the read count to the specified
        ///     <paramref name="readCount" /> value.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="length">The length of the array to read.</param>
        /// <param name="readCount">The read count value to add the read number of bytes to.</param>
        /// <returns>The read value.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="stream" /> is null.</exception>
        /// <exception cref="System.IO.EndOfStreamException">Thrown if the stream has unexpectedly ended.</exception>
        public static byte[] ReadBytes(this Stream stream, int length, ref int readCount)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            var bytes = new byte[length];
            var read = stream.Read(bytes, 0, length);

            if (read != length)
                throw new EndOfStreamException();

            readCount += length;
            return bytes;
        }

        /// <summary>
        ///     Reads a <see cref="string" /> from the specified <paramref name="stream" /> and adds the read count to the
        ///     specified <paramref name="readCount" /> value.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="length">The maximum length of the string to read.</param>
        /// <param name="readCount">The read count value to add the read number of bytes to.</param>
        /// <returns>The read value.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="stream" /> is null.</exception>
        /// <exception cref="System.IO.EndOfStreamException">Thrown if the stream has unexpectedly ended.</exception>
        public static string ReadString(this Stream stream, int length, ref int readCount)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            var sb = new StringBuilder();

            readCount += length;

            var terminated = false;
            for (; length > 0; length--)
            {
                var b = stream.ReadByte();

                if (b == -1)
                    throw new EndOfStreamException();

                if (b == 0)
                {
                    terminated = true;
                    break;
                }

                sb.Append((char) (byte) b);
            }
            length--;

            if (!terminated)
                throw new EndOfStreamException();

            for (; length > 0; length--)
            {
                if (stream.ReadByte() == -1)
                    throw new EndOfStreamException();
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Reads a <see cref="char" /> from the specified <paramref name="stream" /> and adds the read count to the specified
        ///     <paramref name="readCount" /> value.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="readCount">The read count value to add the read number of bytes to.</param>
        /// <returns>The read value.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="stream" /> is null.</exception>
        /// <exception cref="System.IO.EndOfStreamException">Thrown if the stream has unexpectedly ended.</exception>
        public static char ReadChar(this Stream stream, ref int readCount)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            var b = stream.ReadByte();

            if (b == -1)
                throw new EndOfStreamException();

            readCount++;
            return (char) (byte) b;
        }

        /// <summary>
        ///     Reads a <see cref="ushort" /> from the specified <paramref name="stream" /> and adds the read count to the
        ///     specified <paramref name="readCount" /> value.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="readCount">The read count value to add the read number of bytes to.</param>
        /// <returns>The read value.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="stream" /> is null.</exception>
        /// <exception cref="System.IO.EndOfStreamException">Thrown if the stream has unexpectedly ended.</exception>
        public static ushort ReadUShort(this Stream stream, ref int readCount)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            var l = stream.ReadByte();
            var r = stream.ReadByte();

            if (l == -1 || r == -1)
                throw new EndOfStreamException();

            readCount += 2;
            return (ushort) ((l << 8) | r);
        }

        #endregion
    }
}