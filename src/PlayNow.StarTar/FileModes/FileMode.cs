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
    ///     Contains all available file modes.
    /// </summary>
    [Flags]
    public enum FileMode : byte
    {
        /// <summary>
        ///     No access.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Allow executing the file.
        /// </summary>
        Execute = 1,

        /// <summary>
        ///     Allow writing the file.
        /// </summary>
        Write = 2,

        /// <summary>
        ///     Allow reading the file.
        /// </summary>
        Read = 4
    }
}