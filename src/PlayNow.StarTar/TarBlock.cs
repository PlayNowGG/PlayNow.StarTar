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

namespace PlayNow.StarTar
{
    /// <summary>
    ///     Contains Tar block helper methods.
    /// </summary>
    internal static class TarBlock
    {
        /// <summary>
        ///     The size of a single tar block.
        /// </summary>
        public const int Size = 512;

        /// <summary>
        ///     Returns the remaining size of last written block when the specified number of bytes have been written.
        /// </summary>
        /// <param name="numberOfWrittenBytes">The number of written bytes.</param>
        /// <returns>The remaining size of the last written block.</returns>
        public static int RemainingBlockSize(long numberOfWrittenBytes) => (int) (Size - numberOfWrittenBytes % Size) % Size;
    }
}