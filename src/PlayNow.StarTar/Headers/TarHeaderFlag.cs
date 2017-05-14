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

namespace PlayNow.StarTar.Headers
{
    /// <summary>
    ///     Contains a near-complete set of availble flags in a tar header.
    /// </summary>
    public enum TarHeaderFlag : byte
    {
        /// <summary>
        ///     A regular file.
        /// </summary>
        NormalFile = (byte) '0',

        /// <summary>
        ///     A regular file. (Alternative representation of <see cref="NormalFile" />).
        /// </summary>
        NormalFileAlt = (byte) '\0',

        /// <summary>
        ///     A link.
        /// </summary>
        HardLink = (byte) '1',

        /// <summary>
        ///     A symbolic link.
        /// </summary>
        SymbolicLink = (byte) '2',

        /// <summary>
        ///     A character special.
        /// </summary>
        CharacterSpecial = (byte) '3',

        /// <summary>
        ///     A block special.
        /// </summary>
        BlockSpecial = (byte) '4',

        /// <summary>
        ///     A directory.
        /// </summary>
        Directory = (byte) '5',

        /// <summary>
        ///     A FIFO special.
        /// </summary>
        Fifo = (byte) '6',

        /// <summary>
        ///     A contiguous file.
        /// </summary>
        ContiguousFile = (byte) '7',

        /// <summary>
        ///     A global extended header.
        /// </summary>
        GlobalExtendedHeader = (byte) 'g',

        /// <summary>
        ///     An extended header referring to the next file in the archive.
        /// </summary>
        ExtendedHeader = (byte) 'x',

        /// <summary>
        ///     A link extension.
        /// </summary>
        LongLink = (byte) 'K',

        /// <summary>
        ///     A name extension.
        /// </summary>
        LongName = (byte) 'L'
    }
}