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
using System.Collections.Generic;
using PlayNow.StarTar.FileModes;

namespace PlayNow.StarTar.Headers
{
    /// <summary>
    ///     Represents a bag of additions to a tar header.
    /// </summary>
    internal class TarHeaderBag : IReadOnlyTarHeader
    {
        private readonly List<TarHeaderFlag> _flags = new List<TarHeaderFlag>();

        /// <summary>
        ///     Adds the specified addition for this bag.
        /// </summary>
        /// <param name="addition">The addition to add to this bag.</param>
        /// <returns>true if an addition was added from the specified stream; false otherwise.</returns>
        public bool Add(TarEntryStream addition)
        {
            var tmp = 0;

            switch (addition.Header.Flag)
            {
                case TarHeaderFlag.LongName:
                    Name = addition.ReadString((int) (addition.Header.Size ?? 0), ref tmp);
                    break;
                case TarHeaderFlag.LongLink:
                    LinkName = addition.ReadString((int) (addition.Header.Size ?? 0), ref tmp);
                    break;
                default:
                    return false;
            }

            _flags.Add(addition.Header.Flag);
            return true;
        }

        /// <summary>
        ///     Returns a new read-only header instance based on the specified <paramref name="header" /> with the additions from
        ///     this header bag.
        /// </summary>
        /// <param name="header">The header to base the new instance on.</param>
        /// <returns>The newly created header istance.</returns>
        public IReadOnlyTarHeader ApplyTo(ITarHeader header)
        {
            return new TarHeaderBag
            {
                Checksum = header.Checksum,
                DeviceMajor = header.DeviceMajor,
                DeviceMinor = header.DeviceMinor,
                Flag = header.Flag,
                LinkName = _flags.Contains(TarHeaderFlag.LongLink) ? LinkName : header.LinkName,
                Mode = new ReadOnlyFileModeGroup(header.Mode),
                Name = _flags.Contains(TarHeaderFlag.LongName) ? Name : header.Name,
                Size = header.Size,
                GroupId = header.GroupId,
                GroupName = header.GroupName,
                LastModification = header.LastModification,
                UserId = header.UserId,
                UserName = header.UserName,
                FileNamePrefix = header.FileNamePrefix
            };
        }

        #region Implementation of IReadOnlyTarHeader

        /// <summary>
        ///     Gets the checksum of this header.
        /// </summary>
        public string Checksum { get; private set; }

        /// <summary>
        ///     Gets the device major value of this entry.
        /// </summary>
        public string DeviceMajor { get; private set; }

        /// <summary>
        ///     Gets the device minor value of this entry.
        /// </summary>
        public string DeviceMinor { get; private set; }

        /// <summary>
        ///     Gets the file mode of this entry.
        /// </summary>
        public IReadOnlyFileModeGroup Mode { get; private set; }

        /// <summary>
        ///     Gets the name of the entry.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Gets the size of the entry.
        /// </summary>
        public long? Size { get; private set; }

        /// <summary>
        ///     Gets the flags of this entry.
        /// </summary>
        public TarHeaderFlag Flag { get; private set; }

        /// <summary>
        ///     Gets the group identifier of the owning group of this entry. The value is an octal representation of the group
        ///     identifier.
        /// </summary>
        public string GroupId { get; private set; }

        /// <summary>
        ///     Gets the name of the owning group of this entry.
        /// </summary>
        public string GroupName { get; private set; }

        /// <summary>
        ///     Gets the last modification date of this entry.
        /// </summary>
        public DateTime? LastModification { get; private set; }

        /// <summary>
        ///     Gets the name of the link if <see cref="P:PlayNow.StarTar.Headers.IReadOnlyTarHeader.Flag" /> is set to an
        ///     appropriate value.
        /// </summary>
        public string LinkName { get; private set; }

        /// <summary>
        ///     Gets the user identifier of the owning user of this entry. The value is an octal representation of the group
        ///     identifier.
        /// </summary>
        public string UserId { get; private set; }

        /// <summary>
        ///     Gets the name of the owning user of this entry.
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        ///     Gets the file name prefix of this entry.
        /// </summary>
        public string FileNamePrefix { get; private set; }

        #endregion
    }
}