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
    ///     Contains the properties of a read-only tar header.
    /// </summary>
    public interface IReadOnlyTarHeader
    {
        /// <summary>
        ///     Gets the checksum of this header.
        /// </summary>
        string Checksum { get; }

        /// <summary>
        ///     Gets the device major value of this entry.
        /// </summary>
        string DeviceMajor { get; }

        /// <summary>
        ///     Gets the device minor value of this entry.
        /// </summary>
        string DeviceMinor { get; }

        /// <summary>
        ///     Gets the file mode of this entry.
        /// </summary>
        IReadOnlyFileModeGroup Mode { get; }

        /// <summary>
        ///     Gets the name of the entry.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets the size of the entry.
        /// </summary>
        long? Size { get; }

        /// <summary>
        ///     Gets the flags of this entry.
        /// </summary>
        TarHeaderFlag Flag { get; }

        /// <summary>
        ///     Gets the group identifier of the owning group of this entry. The value is an octal representation of the group
        ///     identifier.
        /// </summary>
        string GroupId { get; }

        /// <summary>
        ///     Gets the name of the owning group of this entry.
        /// </summary>
        string GroupName { get; }

        /// <summary>
        ///     Gets the last modification date of this entry.
        /// </summary>
        DateTime? LastModification { get; }

        /// <summary>
        ///     Gets the name of the link if <see cref="Flag" /> is set to an appropriate value.
        /// </summary>
        string LinkName { get; }

        /// <summary>
        ///     Gets the user identifier of the owning user of this entry. The value is an octal representation of the group
        ///     identifier.
        /// </summary>
        string UserId { get; }

        /// <summary>
        ///     Gets the name of the owning user of this entry.
        /// </summary>
        string UserName { get; }

        /// <summary>
        ///     Gets the file name prefix of this entry.
        /// </summary>
        string FileNamePrefix { get; }
    }
}