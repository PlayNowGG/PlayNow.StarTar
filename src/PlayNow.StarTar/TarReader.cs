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
using PlayNow.StarTar.Headers;

namespace PlayNow.StarTar
{
    /// <summary>
    ///     Represents a reader for tar archives.
    /// </summary>
    public sealed class TarReader : IDisposable
    {
        // TODO: FileName prefix support.

        private TarEntryStream _activeEntryStream;
        private bool _disposed;
        private bool _finished;
        private Stream _stream;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TarReader" /> class.
        /// </summary>
        /// <param name="stream">The stream to read the archive from.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="stream" /> is null.</exception>
        public TarReader(Stream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="TarReader" /> class.
        /// </summary>
        ~TarReader()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Writes the remaining contents of the archive to the directory at the specified <paramref name="targetPath" />.
        /// </summary>
        /// <param name="targetPath">The path to the directory to which to write the remaining contents of the archive.</param>
        /// <param name="skipTopLevelDirectory">if set to <c>true</c>, the top level directory in the archive is skipped.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="targetPath" /> is null.</exception>
        /// <exception cref="TarException">
        ///     Thrown when an entry attempts to write outside of the target path by using parent path
        ///     (../) operators.
        /// </exception>
        /// <exception cref="NotSupportedException">Thrown when an entry contains unsupported flags.</exception>
        public void WriteToDirectory(string targetPath, bool skipTopLevelDirectory)
        {
            if (targetPath == null) throw new ArgumentNullException(nameof(targetPath));

            AssertNotDisposed();

            // Collapse target directory.
            targetPath = PathHelper.Normalize(targetPath);
            targetPath = PathHelper.AppendDirectorySeparator(targetPath);

            // Read all files in the archive.
            TarEntryStream stream;
            while ((stream = Next()) != null)
            {
                // Convert all directory separators to the local machine's prefered form.
                var fileName = PathHelper.PartialNormalize(stream.Header.Name);

                // Trim leading directory separators.
                fileName = PathHelper.TrimLeadingDirectorySeparators(fileName);

                // Skip the top level directory.
                if (skipTopLevelDirectory)
                {
                    fileName = PathHelper.RemoveTopLevelFolderFromRelativePath(fileName);

                    // Skip if it was the top level.
                    if (fileName == null)
                        continue;
                }

                var filePath = PathHelper.Normalize(Path.Combine(targetPath, fileName));

                if (!filePath.StartsWith(targetPath))
                    throw new TarException($"{Path.Combine(targetPath, fileName)} is outside of the specified target directory.");

                // If it's a directory entry.
                if (fileName.EndsWith(Path.DirectorySeparatorChar.ToString()) || stream.Header.Flag == TarHeaderFlag.Directory)
                {
                    Directory.CreateDirectory(filePath);

                    // TODO: FIXME None of the below set the "last modified" date on windows.
                    Directory.SetCreationTimeUtc(filePath, stream.Header.LastModification.Value);
                    Directory.SetLastWriteTimeUtc(filePath, stream.Header.LastModification.Value);
                    Directory.SetLastAccessTimeUtc(filePath, stream.Header.LastModification.Value);
                }

                // If it's a file entry.
                else if (stream.Header.Flag == TarHeaderFlag.NormalFile || stream.Header.Flag == TarHeaderFlag.NormalFileAlt)
                {
                    using (var fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write))
                    {
                        stream.CopyTo(fileStream);
                    }

                    File.SetCreationTimeUtc(filePath, stream.Header.LastModification.Value);
                    File.SetLastWriteTimeUtc(filePath, stream.Header.LastModification.Value);
                }
                else
                {
                    throw new NotSupportedException(
                        $"File entries of type '{stream.Header.Flag}' are not supported by WriteToDirectory at this time.");
                }
            }
        }

        /// <summary>
        ///     Moves the <see cref="_stream" /> to the end of the specified <paramref name="entryStream" />.
        /// </summary>
        /// <param name="entryStream">The entry stream.</param>
        private void MoveToEndOfStream(TarEntryStream entryStream)
        {
            // If previous stream isset, move past it's contents.
            if (entryStream == null) return;

            var length = entryStream.Header.Size ?? 0;
            var padding = TarBlock.RemainingBlockSize(length);
            var remaining = length - entryStream.PositionInternal + padding;

            // Skip remainder and re-align to 512 blocks
            for (; remaining > 0; remaining--)
            {
                _stream.ReadByte();
            }
        }

        /// <summary>
        ///     Closes the active entry stream and moves <see cref="_stream" /> to the end of the entry.
        /// </summary>
        private void CloseActiveEntryStream()
        {
            MoveToEndOfStream(_activeEntryStream);
            _activeEntryStream?.Dispose();
            _activeEntryStream = null;
        }

        /// <summary>
        ///     Returns the next entry in the archive.
        /// </summary>
        /// <returns>The next entry in the archive or null when there are no more remaining entries.</returns>
        public TarEntryStream Next()
        {
            AssertNotDisposed();

            if (_finished)
                return null;

            CloseActiveEntryStream();

            var header = TarHeader.Deserialize(_stream);

            if (header == null)
            {
                _finished = true;
                return null;
            }

            // Compile a header with all additional annotations found.
            var headerBag = new TarHeaderBag();
            var annotationStream = new TarEntryStream(new ReadOnlyTarHeader(header), _stream);

            // Keep adding the stream to the header bag while they are annotations.
            while (headerBag.Add(annotationStream))
            {
                // Re-align the input stream to the 512 blocks and close the annotation stream.
                MoveToEndOfStream(annotationStream);
                annotationStream.Dispose();

                header = TarHeader.Deserialize(_stream);


                // The annotation appears to be the last entry in the archive. Throwing an exception seems little crude
                // since the rest of the archive was just fine.
                if (header == null)
                {
                    _finished = true;
                    return null;
                }

                // Create a new annotation stream for the next round of checks.
                annotationStream = new TarEntryStream(new ReadOnlyTarHeader(header), _stream);
            }

            // The annotation is not used at this point - skip it.
            annotationStream.Dispose();

            _activeEntryStream = new TarEntryStream(headerBag.ApplyTo(header), _stream);
            return _activeEntryStream;
        }

        /// <summary>
        ///     Asserts this instance has not yet been disposed of.
        /// </summary>
        private void AssertNotDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(TarReader));
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
                _activeEntryStream?.Dispose();
                _activeEntryStream = null;
                _stream = null;
            }
        }

        #region IDisposable

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}