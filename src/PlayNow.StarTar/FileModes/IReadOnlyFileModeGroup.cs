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

namespace PlayNow.StarTar.FileModes
{
    /// <summary>
    ///     Contains the properties for a read-only file mode group.
    /// </summary>
    public interface IReadOnlyFileModeGroup
    {
        /// <summary>
        ///     Gets the collective file mode in an octal string representation.
        /// </summary>
        string Buffer { get; }

        /// <summary>
        ///     Gets the collective file mode in a decimal numeric representation.
        /// </summary>
        ushort Mode { get; }

        /// <summary>
        ///     Gets the file mode for users in the owning group of the file.
        /// </summary>
        FileMode GroupMode { get; }

        /// <summary>
        ///     Gets the file mode for other users.
        /// </summary>
        FileMode OtherMode { get; }

        /// <summary>
        ///     Gets the mode for the owning user of the file.
        /// </summary>
        FileMode UserMode { get; }
    }
}