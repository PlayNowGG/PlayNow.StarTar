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
using PlayNow.StarTar.Headers;

namespace PlayNow.StarTar
{
    /// <summary>
    ///     Represents a writer for tar archives.
    /// </summary>
    public sealed class TarWriter : IDisposable
    {
        private readonly byte[] _buffer = new byte[1024 * 32];
        private bool _disposed;
        private Stream _stream;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TarWriter" /> class.
        /// </summary>
        /// <param name="stream">The stream to write the archive to.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="stream" /> is null.</exception>
        public TarWriter(Stream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="TarWriter" /> class.
        /// </summary>
        ~TarWriter()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Writes the specified header to the output stream. If <paramref name="header" /> is null, an emtpy block is written.
        /// </summary>
        /// <param name="header">The header to write to the output stream.</param>
        public void WriteHeader(ITarHeader header)
        {
            if (header == null)
                WritePadding(-1);
            else
                header.Serialize(_stream);
        }

        /// <summary>
        ///     Copies the specified <paramref name="stream" /> to the output stream and re-aligns the output stream to the next
        ///     512-block.
        /// </summary>
        /// <param name="stream">The stream to copy.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="stream" /> is null.</exception>
        public void WriteStream(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            AssertNotDisposed();

            int readCount;
            long totalRead = 0;
            while ((readCount = stream.Read(_buffer, 0, _buffer.Length)) > 0)
            {
                _stream.Write(_buffer, 0, readCount);
                totalRead += readCount;
            }

            WritePadding(totalRead);
        }

        /// <summary>
        ///     Writes a tar header for a directory entry to the output stream.
        /// </summary>
        /// <param name="path">The path to the directory in the archive.</param>
        /// <param name="lastModification">The last modification date in UTC of the directory.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="path" /> is null.</exception>
        public void WriteDirectoryEntry(string path, DateTime lastModification)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            AssertNotDisposed();

            path = PathHelper.TarNormalizeDirectory(path);

            if (path.Length >= 100)
            {
                WriteLongNameEntry(path);

                // NOTE: GNU tar puts 100 characters into the name field, without
                // the null terminator. However, the GNU tar specifications state
                // the name should end with a null terminator. Sticking to the
                // specifications here.
                path = path.Substring(0, 99);
            }

            var header = new TarHeader
            {
                Name = path,
                Flag = TarHeaderFlag.Directory,
                LastModification = lastModification
            };

            WriteHeader(header);
        }

        /// <summary>
        ///     Writes a tar header for a file entry to the output stream.
        /// </summary>
        /// <param name="path">The path to the file in the archive.</param>
        /// <param name="size">The size of the file.</param>
        /// <param name="lastModification">The last modification date in UTC of the file.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="fileMode">The file mode.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="path" /> is null.</exception>
        public void WriteFileEntry(string path, long size, DateTime lastModification, string userId, string groupId, IFileModeGroup fileMode)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            AssertNotDisposed();

            path = PathHelper.TarNormalize(path);

            if (path.Length >= 100)
            {
                WriteLongNameEntry(path);

                // NOTE: GNU tar puts 100 characters into the name field, without
                // the null terminator. However, the GNU tar specifications state
                // the name should end with a null terminator. Sticking to the
                // specifications here.
                path = path.Substring(0, 99);
            }

            var header = new TarHeader
            {
                Name = path,
                Mode = fileMode,
                Size = size,
                Flag = TarHeaderFlag.NormalFileAlt,
                UserId = userId,
                GroupId = groupId,
                LastModification = lastModification
            };

            WriteHeader(header);
        }

        /// <summary>
        ///     Writes the directory with the specified <paramref name="path" /> and its contents to the archive.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="skipTopLevelDirectory">
        ///     If set to <c>true</c>, only the contents of the directory with the specified
        ///     <paramref name="path" /> are written to the archive.
        /// </param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="path" /> is null.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified directory could not be found.</exception>
        public void WriteDirectory(string path, bool skipTopLevelDirectory)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            AssertNotDisposed();

            // Make sure the directory exists.
            path = PathHelper.Normalize(path);

            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException("The specified directory could not be found.");

            // Compute the base path.
            var basePath = skipTopLevelDirectory ? path : Directory.GetParent(path).FullName;
            basePath = PathHelper.AppendDirectorySeparator(basePath);

            WriteDirectoryInternal(path, basePath, skipTopLevelDirectory);
        }

        /// <summary>
        ///     Asserts this instance has not yet been disposed of.
        /// </summary>
        private void AssertNotDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(TarWriter));
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            _disposed = true;

            if (disposing)
            {
                // Write the finishing two null blocks.
                WriteHeader(null);
                WriteHeader(null);

                _stream = null;
            }
        }

        /// <summary>
        ///     Writes padding to the output stream to align it to the next 512-block.
        /// </summary>
        /// <param name="writtenSize">Number of written bytes since last alignment.</param>
        private void WritePadding(long writtenSize)
        {
            var padding = writtenSize < 0 ? TarBlock.Size : TarBlock.RemainingBlockSize(writtenSize);

            Array.Clear(_buffer, 0, Math.Min(_buffer.Length, padding));
            while (padding > 0)
            {
                var writeCount = Math.Min(_buffer.Length, padding);
                _stream.Write(_buffer, 0, writeCount);
                padding -= writeCount;
            }
        }

        /// <summary>
        ///     Writes a long name entry to the archive.
        /// </summary>
        /// <param name="name">The long name.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="name" /> is null.</exception>
        private void WriteLongNameEntry(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            name = PathHelper.TarNormalize(name);

            var header = new TarHeader
            {
                Name = "././@LongLink", // Is supposed to say LongLink, even if it's a long name
                Mode = new FileModeGroup("644"), // Seems to be the default for long links?
                Size = name.Length + 1,
                Flag = TarHeaderFlag.LongName
            };

            // Write the header.
            WriteHeader(header);

            // Write the content.
            var length = _stream.Write(name, name.Length + 1);
            WritePadding(length);
        }

        /// <summary>
        ///     Writes a directory and its contents to the archive. This method is recursive.
        /// </summary>
        /// <param name="path">The path to the directory.</param>
        /// <param name="basePath">The base path  which to remove from the written path names.</param>
        /// <param name="skipDirectory">if set to <c>true</c> skip the directory itself.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path" /> or <paramref name="basePath" /> is null.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified directory could not be found.</exception>
        private void WriteDirectoryInternal(string path, string basePath, bool skipDirectory)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (basePath == null) throw new ArgumentNullException(nameof(basePath));

            AssertNotDisposed();

            // Make sure the directory exists.
            path = PathHelper.Normalize(path);

            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException("The specified directory could not be found.");

            // Write the directory to the archive.
            if (!skipDirectory)
            {
                var dirInfo = new DirectoryInfo(path);
                WriteDirectoryEntry(path.Substring(basePath.Length), dirInfo.LastWriteTimeUtc);
            }

            // Write the files in the directory to the archive.
            foreach (var filePath in Directory.GetFiles(path))
            {
                var fileInfo = new FileInfo(filePath);

                WriteFileEntry(filePath.Substring(basePath.Length), fileInfo.Length, fileInfo.LastWriteTimeUtc, null, null, new FileModeGroup("777"));
                WriteStream(fileInfo.OpenRead());
            }

            // Write directories in the directory to the archive.
            foreach (var directoryPath in Directory.GetDirectories(path))
            {
                WriteDirectoryInternal(directoryPath, basePath, false);
            }
        }

        #region IDisposable

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}