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
    ///     Represents a stream when reading an entry from a tar archive using the <see cref="TarReader" />.
    /// </summary>
    public sealed class TarEntryStream : Stream
    {
        private readonly Stream _underlyingStream;
        private bool _disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TarEntryStream" /> class.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="stream">The underlying stream.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="header" /> or <paramref name="stream" /> is null.</exception>
        internal TarEntryStream(IReadOnlyTarHeader header, Stream stream)
        {
            Header = header ?? throw new ArgumentNullException(nameof(header));
            _underlyingStream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        /// <summary>
        ///     Gets the header attached to the entry represented by this stream including any .
        /// </summary>
        public IReadOnlyTarHeader Header { get; }

        /// <summary>
        ///     Gets the position of this stream, even after disposing.
        /// </summary>
        internal long PositionInternal { get; private set; }

        /// <summary>
        ///     Asserts this instance has not yet been disposed of.
        /// </summary>
        private void AssertNotDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(TarEntryStream));
        }

        #region Overrides of Stream

        /// <summary>
        ///     Releases the unmanaged resources used by the <see cref="T:System.IO.Stream" /> and optionally releases the
        ///     managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     true to release both managed and unmanaged resources; false to release only unmanaged
        ///     resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _disposed = true;
        }

        /// <summary>
        ///     When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be
        ///     written to the underlying device.
        /// </summary>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void Flush()
        {
            AssertNotDisposed();
        }

        /// <summary>
        ///     When overridden in a derived class, reads a sequence of bytes from the current stream and advances the
        ///     position within the stream by the number of bytes read.
        /// </summary>
        /// <returns>
        ///     The total number of bytes read into the buffer. This can be less than the number of bytes requested if that
        ///     many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <param name="buffer">
        ///     An array of bytes. When this method returns, the buffer contains the specified byte array with the
        ///     values between <paramref name="offset" /> and (<paramref name="offset" /> + <paramref name="count" /> - 1) replaced
        ///     by the bytes read from the current source.
        /// </param>
        /// <param name="offset">
        ///     The zero-based byte offset in <paramref name="buffer" /> at which to begin storing the data read
        ///     from the current stream.
        /// </param>
        /// <param name="count">The maximum number of bytes to be read from the current stream. </param>
        /// <exception cref="T:System.ArgumentException">
        ///     The sum of <paramref name="offset" /> and <paramref name="count" /> is
        ///     larger than the buffer length.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="buffer" /> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="offset" /> or <paramref name="count" /> is negative.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support reading. </exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            AssertNotDisposed();

            if (PositionInternal == Length)
                return 0;

            var remaining = Length - PositionInternal;
            var readCount = _underlyingStream.Read(buffer, offset, (int) Math.Min(count, remaining));

            if (readCount == 0)
                throw new EndOfStreamException("Unexpected end of stream");

            PositionInternal += readCount;

            return readCount;
        }

        /// <summary>When overridden in a derived class, sets the position within the current stream.</summary>
        /// <returns>The new position within the current stream.</returns>
        /// <param name="offset">A byte offset relative to the <paramref name="origin" /> parameter. </param>
        /// <param name="origin">
        ///     A value of type <see cref="T:System.IO.SeekOrigin" /> indicating the reference point used to
        ///     obtain the new position.
        /// </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     The stream does not support seeking, such as if the stream is
        ///     constructed from a pipe or console output.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>When overridden in a derived class, sets the length of the current stream.</summary>
        /// <param name="value">The desired length of the current stream in bytes. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     The stream does not support both writing and seeking, such as if the
        ///     stream is constructed from a pipe or console output.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current
        ///     position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">
        ///     An array of bytes. This method copies <paramref name="count" /> bytes from
        ///     <paramref name="buffer" /> to the current stream.
        /// </param>
        /// <param name="offset">
        ///     The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes to the
        ///     current stream.
        /// </param>
        /// <param name="count">The number of bytes to be written to the current stream. </param>
        /// <exception cref="T:System.ArgumentException">
        ///     The sum of <paramref name="offset" /> and <paramref name="count" /> is
        ///     greater than the buffer length.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="buffer" />  is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="offset" /> or <paramref name="count" /> is negative.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occured, such as the specified file cannot be found.</exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support writing.</exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     <see cref="M:System.IO.Stream.Write(System.Byte[],System.Int32,System.Int32)" /> was called after the stream was
        ///     closed.
        /// </exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary>When overridden in a derived class, gets a value indicating whether the current stream supports reading.</summary>
        /// <returns>true if the stream supports reading; otherwise, false.</returns>
        public override bool CanRead => true;

        /// <summary>When overridden in a derived class, gets a value indicating whether the current stream supports seeking.</summary>
        /// <returns>true if the stream supports seeking; otherwise, false.</returns>
        public override bool CanSeek => false;

        /// <summary>When overridden in a derived class, gets a value indicating whether the current stream supports writing.</summary>
        /// <returns>true if the stream supports writing; otherwise, false.</returns>
        public override bool CanWrite => false;

        /// <summary>When overridden in a derived class, gets the length in bytes of the stream.</summary>
        /// <returns>A long value representing the length of the stream in bytes.</returns>
        /// <exception cref="T:System.NotSupportedException">A class derived from Stream does not support seeking. </exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        public override long Length
        {
            get
            {
                AssertNotDisposed();
                return Header.Size ?? 0;
            }
        }

        /// <summary>When overridden in a derived class, gets or sets the position within the current stream.</summary>
        /// <returns>The current position within the stream.</returns>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support seeking. </exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        public override long Position
        {
            get
            {
                AssertNotDisposed();
                return PositionInternal;
            }
            set => throw new NotSupportedException();
        }

        #endregion
    }
}