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
    ///     Represents a read-only wrapper for a file mode group.
    /// </summary>
    public class ReadOnlyFileModeGroup : IReadOnlyFileModeGroup
    {
        private readonly IFileModeGroup _fileModeGroup;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReadOnlyFileModeGroup" /> class.
        /// </summary>
        /// <param name="fileModeGroup">The underlying file mode group.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fileModeGroup" /> is null.</exception>
        public ReadOnlyFileModeGroup(IFileModeGroup fileModeGroup)
        {
            _fileModeGroup = fileModeGroup ?? throw new ArgumentNullException(nameof(fileModeGroup));
        }

        #region Implementation of IReadOnlyFileModeGroup

        /// <summary>
        ///     Gets the collective file mode in an octal string representation.
        /// </summary>
        public string Buffer => _fileModeGroup.Buffer;

        /// <summary>
        ///     Gets the file mode for users in the owning group of the file.
        /// </summary>
        public FileMode GroupMode => _fileModeGroup.GroupMode;

        /// <summary>
        ///     Gets the collective file mode in a decimal numeric representation.
        /// </summary>
        public ushort Mode => _fileModeGroup.Mode;

        /// <summary>
        ///     Gets the file mode for other users.
        /// </summary>
        public FileMode OtherMode => _fileModeGroup.OtherMode;

        /// <summary>
        ///     Gets the mode for the owning user of the file.
        /// </summary>
        public FileMode UserMode => _fileModeGroup.UserMode;

        #endregion
    }
}