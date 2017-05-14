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

namespace PlayNow.StarTar
{
    /// <summary>
    ///     Represents errors which occur while reading or writing tar archives.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class TarException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TarException" /> class.
        /// </summary>
        public TarException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TarException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public TarException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TarException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public TarException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}