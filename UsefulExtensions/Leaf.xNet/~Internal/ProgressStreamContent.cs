﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace Leaf.xNet
{
    internal delegate void ProgressDelegate(long bytes, long totalBytes, long totalBytesExpected);
    
    // ReSharper disable once UnusedType.Global
    internal class ProgressStreamContent : System.Net.Http.StreamContent
    {
        // ReSharper disable once UnusedMember.Global
        public ProgressStreamContent(Stream stream, CancellationToken token)
            : this(new ProgressStream(stream, token))
        {
        }

        // ReSharper disable once UnusedMember.Global
        public ProgressStreamContent(Stream stream, int bufferSize)
            : this(new ProgressStream(stream, CancellationToken.None), bufferSize)
        {
        }

        private ProgressStreamContent(ProgressStream stream)
            : base(stream) => Init(stream);

        private ProgressStreamContent(ProgressStream stream, int bufferSize)
            : base(stream, bufferSize) => Init(stream);

        private void Init(ProgressStream stream)
        {
            stream.ReadCallback = ReadBytes;

            Progress = delegate { };
        }

        private void Reset()
        {
            _totalBytes = 0L;
        }

        private long _totalBytes;
        private long _totalBytesExpected = -1;

        private void ReadBytes(long bytes)
        {
            if (_totalBytesExpected == -1)
                _totalBytesExpected = Headers.ContentLength ?? -1;

            if (_totalBytesExpected == -1 && TryComputeLength(out long computedLength))
                _totalBytesExpected = computedLength == 0 ? -1 : computedLength;

            // If less than zero still then change to -1
            _totalBytesExpected = Math.Max(-1, _totalBytesExpected);
            _totalBytes += bytes;

            Progress(bytes, _totalBytes, _totalBytesExpected);
        }
        
        // ReSharper disable once MemberCanBePrivate.Global
        public ProgressDelegate Progress
        {
            get => _progress;
            set => _progress = value ?? delegate { };
        }
        private ProgressDelegate _progress;

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            Reset();
            return base.SerializeToStreamAsync(stream, context);
        }

        protected override bool TryComputeLength(out long length)
        {
            bool result = base.TryComputeLength(out length);
            _totalBytesExpected = length;
            return result;
        }

        private class ProgressStream : Stream
        {
            private readonly CancellationToken _token;

            public ProgressStream(Stream stream, CancellationToken token)
            {
                ParentStream = stream;
                _token = token;

                ReadCallback = delegate { };
                WriteCallback = delegate { };
            }

            public Action<long> ReadCallback { private get; set; }

            private Action<long> WriteCallback { get;}

            private Stream ParentStream { get; }

            public override bool CanRead => ParentStream.CanRead;

            public override bool CanSeek => ParentStream.CanSeek;

            public override bool CanWrite => ParentStream.CanWrite;

            public override bool CanTimeout => ParentStream.CanTimeout;

            public override long Length => ParentStream.Length;

            public override void Flush()
            {
                ParentStream.Flush();
            }

            public override Task FlushAsync(CancellationToken cancellationToken)
            {
                return ParentStream.FlushAsync(cancellationToken);
            }

            public override long Position
            {
                get => ParentStream.Position;
                set => ParentStream.Position = value;
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                _token.ThrowIfCancellationRequested();

                int readCount = ParentStream.Read(buffer, offset, count);
                ReadCallback(readCount);
                return readCount;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                _token.ThrowIfCancellationRequested();
                return ParentStream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                _token.ThrowIfCancellationRequested();
                ParentStream.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                _token.ThrowIfCancellationRequested();
                ParentStream.Write(buffer, offset, count);
                WriteCallback(count);
            }

            public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                _token.ThrowIfCancellationRequested();
                var linked = CancellationTokenSource.CreateLinkedTokenSource(_token, cancellationToken);

                int readCount = await ParentStream.ReadAsync(buffer, offset, count, linked.Token);

                ReadCallback(readCount);
                return readCount;
            }

            public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                _token.ThrowIfCancellationRequested();

                var linked = CancellationTokenSource.CreateLinkedTokenSource(_token, cancellationToken);
                var task = ParentStream.WriteAsync(buffer, offset, count, linked.Token);

                WriteCallback(count);
                return task;
            }


            #region Dispose
            private bool _disposed;
            protected override void Dispose(bool disposing)
            {
                if (_disposed)
                    return;
                
                if (disposing)
                    ParentStream?.Dispose();

                _disposed = true;
            }
            #endregion
        }
    }
}

