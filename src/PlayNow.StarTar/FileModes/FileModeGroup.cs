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

namespace PlayNow.StarTar.FileModes
{
    /// <summary>
    ///     Represents a collection of file modes for a user, group and others to a file.
    /// </summary>
    public class FileModeGroup : IFileModeGroup, IReadOnlyFileModeGroup
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FileModeGroup" /> class.
        /// </summary>
        /// <param name="buffer">The collective file mode in an octal string representation.</param>
        public FileModeGroup(string buffer)
        {
            // TODO: Verify format.
            Buffer = buffer;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileModeGroup" /> class with the specified file modes for the owning
        ///     user, group and others.
        /// </summary>
        /// <param name="user">The file mode for the owning user.</param>
        /// <param name="group">The file mode for the owning group.</param>
        /// <param name="other">The file mode for others.</param>
        public FileModeGroup(FileMode user, FileMode group, FileMode other)
        {
            Mode = (ushort) ((ushort) user * 8 * 8 + (ushort) group * 8 + (ushort) other);
        }

        #region Implementation of IFileModeGroup

        /// <summary>
        ///     Gets the collective file mode in an octal string representation.
        /// </summary>
        public string Buffer { get; private set; }

        /// <summary>
        ///     Gets or sets the collective file mode in a decimal numeric representation.
        /// </summary>
        public ushort Mode
        {
            get => Buffer == null ? (ushort) 0 : Convert.ToUInt16(Buffer.Trim(), 8);
            set => Buffer = Convert.ToString(value, 8);
        }

        /// <summary>
        ///     Gets or sets the mode for the owning user of the file.
        /// </summary>
        public FileMode UserMode
        {
            get => (FileMode) (Mode / (8 * 8) % 8);
            set => Mode = (ushort) ((ushort) value * 8 * 8 + (ushort) GroupMode * 8 + (ushort) OtherMode);
        }

        /// <summary>
        ///     Gets or sets the file mode for users in the owning group of the file.
        /// </summary>
        public FileMode GroupMode
        {
            get => (FileMode) (Mode / 8 % 8);
            set => Mode = (ushort) ((ushort) value * +(ushort) UserMode * 8 * 8 + (ushort) OtherMode);
        }

        /// <summary>
        ///     Gets or sets the file mode for other users.
        /// </summary>
        public FileMode OtherMode
        {
            get => (FileMode) (Mode % 8);
            set => Mode = (ushort) ((ushort) value * +(ushort) UserMode * 8 * 8 + (ushort) GroupMode * 8);
        }

        #endregion
    }
}