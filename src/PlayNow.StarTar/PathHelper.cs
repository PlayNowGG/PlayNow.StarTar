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
using System.Linq;

namespace PlayNow.StarTar
{
    /// <summary>
    ///     Path utility methods.
    /// </summary>
    internal static class PathHelper
    {
        /// <summary>
        ///     Directory separators on all platforms.
        /// </summary>
        private static readonly char[] Separators = { '/', '\\', '|' };

        /// <summary>
        ///     Directory separator on the current platform.
        /// </summary>
        private static char Separator => Path.DirectorySeparatorChar;

        /// <summary>
        ///     Directory separator used in tar archives.
        /// </summary>
        private static char TarSeparator => '/';

        /// <summary>
        ///     Normalizes the specified path by flattening out relative paths (../) and replacing the directory separators with
        ///     the platform-specific form.
        /// </summary>
        /// <param name="path">The path to normalize.</param>
        /// <returns>The normalized path.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path" /> is null.</exception>
        public static string Normalize(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            return PartialNormalize(Path.GetFullPath(path));
        }

        /// <summary>
        ///     Normalizes the specified path by replacing the directory separators with the platform-specific form.
        /// </summary>
        /// <param name="path">The path to normalize.</param>
        /// <returns>The normalized path.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path" /> is null.</exception>
        public static string PartialNormalize(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            return Separators.Aggregate(path, (current, sep) => current.Replace(sep, Separator));
        }

        /// <summary>
        ///     Normalizes the specified path by replacing the directory separators to the tar form (/).
        /// </summary>
        /// <param name="path">The path to normalize.</param>
        /// <returns>The normalized path.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path" /> is null.</exception>
        public static string TarNormalize(string path)
        {
            return Separators.Aggregate(path, (current, sep) => current.Replace(sep, TarSeparator));
        }

        /// <summary>
        ///     Normalizes the specified path by replacing the directory separators to the tar form (/) and appending another
        ///     directory separator to the end of the path, if not already present.
        /// </summary>
        /// <param name="path">The path to normalize.</param>
        /// <returns>The normalized path.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path" /> is null.</exception>
        public static string TarNormalizeDirectory(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            path = TarNormalize(path);
            return EndsWithDirectorySeparator(path) ? path : path + TarSeparator;
        }

        /// <summary>
        ///     Returns a value indicating whether the specified path ends with a directory separator.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>A value indicating whether the specified path ends with a directory separator.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path" /> is null.</exception>
        public static bool EndsWithDirectorySeparator(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            return Separators.Any(c => path.EndsWith(c.ToString()));
        }

        /// <summary>
        ///     Appends a directory separator for the current platform to the specified <paramref name="path" />.
        /// </summary>
        /// <param name="path">The path to append the directory separator to..</param>
        /// <returns>The newly created path.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path" /> is null.</exception>
        public static string AppendDirectorySeparator(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            return EndsWithDirectorySeparator(path) ? path : path + Separator;
        }

        /// <summary>
        ///     Trims any leading directory separators from the specified path.
        /// </summary>
        /// <param name="path">The path to trim.</param>
        /// <returns>The trimmed path.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path" /> is null.</exception>
        public static string TrimLeadingDirectorySeparators(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            return path.TrimStart(Separators);
        }

        /// <summary>
        ///     Removes the top level folder from the specified <paramref name="relativePath" />.
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        /// <returns>The specified path without the top level folder.</returns>
        public static string RemoveTopLevelFolderFromRelativePath(string relativePath)
        {
            var splits = relativePath.Split(new[] { Separator }, 2);
            if (splits.Length == 2)
                relativePath = splits[1];

            // If it was the top level 
            if (string.IsNullOrEmpty(relativePath) || relativePath == Separator.ToString())
                return null;

            return relativePath;
        }
    }
}