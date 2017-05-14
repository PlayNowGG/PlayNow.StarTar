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
using PlayNow.StarTar.FileModes;

namespace PlayNow.StarTar.Headers
{
    /// <summary>
    ///     Represents a read-only wrapper for a tar header.
    /// </summary>
    public class ReadOnlyTarHeader : IReadOnlyTarHeader
    {
        private readonly ITarHeader _tarHeader;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReadOnlyTarHeader" /> class.
        /// </summary>
        /// <param name="tarHeader">The underlying. tar header.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="tarHeader" /> is null.</exception>
        public ReadOnlyTarHeader(ITarHeader tarHeader)
        {
            _tarHeader = tarHeader ?? throw new ArgumentNullException(nameof(tarHeader));
            Mode = new ReadOnlyFileModeGroup(_tarHeader.Mode);
        }

        #region Implementation of IReadOnlyTarHeader

        /// <summary>
        ///     Gets the checksum of this header.
        /// </summary>
        public string Checksum => _tarHeader.Checksum;

        /// <summary>
        ///     Gets the device major value of this entry.
        /// </summary>
        public string DeviceMajor => _tarHeader.DeviceMajor;

        /// <summary>
        ///     Gets the device minor value of this entry.
        /// </summary>
        public string DeviceMinor => _tarHeader.DeviceMinor;

        /// <summary>
        ///     Gets the file mode of this entry.
        /// </summary>
        public IReadOnlyFileModeGroup Mode { get; }

        /// <summary>
        ///     Gets the name of the entry.
        /// </summary>
        public string Name => _tarHeader.Name;

        /// <summary>
        ///     Gets the size of the entry.
        /// </summary>
        public long? Size => _tarHeader.Size;

        /// <summary>
        ///     Gets the flags of this entry.
        /// </summary>
        public TarHeaderFlag Flag => _tarHeader.Flag;

        /// <summary>
        ///     Gets the group identifier of the owning group of this entry. The value is an octal representation of the group
        ///     identifier.
        /// </summary>
        public string GroupId => _tarHeader.GroupId;

        /// <summary>
        ///     Gets the name of the owning group of this entry.
        /// </summary>
        public string GroupName => _tarHeader.GroupName;

        /// <summary>
        ///     Gets the last modification date of this entry.
        /// </summary>
        public DateTime? LastModification => _tarHeader.LastModification;

        /// <summary>
        ///     Gets the name of the link if <see cref="P:PlayNow.StarTar.Headers.IReadOnlyTarHeader.Flag" /> is set to an
        ///     appropriate value.
        /// </summary>
        public string LinkName => _tarHeader.LinkName;

        /// <summary>
        ///     Gets the user identifier of the owning user of this entry. The value is an octal representation of the group
        ///     identifier.
        /// </summary>
        public string UserId => _tarHeader.UserId;

        /// <summary>
        ///     Gets the name of the owning user of this entry.
        /// </summary>
        public string UserName => _tarHeader.UserName;

        /// <summary>
        ///     Gets the file name prefix of this entry.
        /// </summary>
        public string FileNamePrefix => _tarHeader.FileNamePrefix;

        #endregion
    }
}