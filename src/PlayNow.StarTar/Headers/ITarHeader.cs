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
using PlayNow.StarTar.FileModes;

namespace PlayNow.StarTar.Headers
{
    /// <summary>
    ///     Contains the properties and methods of tar header for a single entry.
    /// </summary>
    public interface ITarHeader
    {
        /// <summary>
        ///     Gets or sets the checksum of this header.
        /// </summary>
        string Checksum { get; set; }

        /// <summary>
        ///     Gets or sets the device major value of this entry.
        /// </summary>
        string DeviceMajor { get; set; }

        /// <summary>
        ///     Gets or sets the device minor value of this entry.
        /// </summary>
        string DeviceMinor { get; set; }

        /// <summary>
        ///     Gets or sets the file mode of this entry.
        /// </summary>
        IFileModeGroup Mode { get; set; }

        /// <summary>
        ///     Gets or sets the name of the entry.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        ///     Gets or sets the size of the entry.
        /// </summary>
        long? Size { get; set; }

        /// <summary>
        ///     Gets or sets the flags of this entry.
        /// </summary>
        TarHeaderFlag Flag { get; set; }

        /// <summary>
        ///     Gets or sets the group identifier of the owning group of this entry. The value is an octal representation of the
        ///     group identifier.
        /// </summary>
        string GroupId { get; set; }

        /// <summary>
        ///     Gets or sets the name of the owning group of this entry.
        /// </summary>
        string GroupName { get; set; }

        /// <summary>
        ///     Gets or sets the last modification date of this entry.
        /// </summary>
        DateTime? LastModification { get; set; }

        /// <summary>
        ///     Gets or sets the name of the link if <see cref="Flag" /> is set to an appropriate value.
        /// </summary>
        string LinkName { get; set; }

        /// <summary>
        ///     Gets or sets the user identifier of the owning user of this entry. The value is an octal representation of the
        ///     group identifier.
        /// </summary>
        string UserId { get; set; }

        /// <summary>
        ///     Gets or sets the name of the owning user of this entry.
        /// </summary>
        string UserName { get; }

        /// <summary>
        ///     Gets or sets the file name prefix of this entry.
        /// </summary>
        string FileNamePrefix { get; set; }

        /// <summary>
        ///     Computes a checksum for this instance and sets it to <see cref="Checksum" />.
        /// </summary>
        void ComputeChecksum();

        /// <summary>
        ///     Writes a serialized form of this header to the specified stream.
        /// </summary>
        /// <param name="stream">The stream to write the serialized header to.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stream" /> is null.</exception>
        void Serialize(Stream stream);
    }
}