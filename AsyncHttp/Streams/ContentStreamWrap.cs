using AsyncHttp.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AsyncHttp.Streams
{
    public class ContentStreamWrap : Stream
    {
        private readonly Stream stream;
        private readonly HttpTcpConnection httpTcpConnection;

        internal ContentStreamWrap(Stream stream, HttpTcpConnection httpTcpConnection)
        {
            this.stream = stream;
            this.httpTcpConnection = httpTcpConnection;
        }

        public override bool CanRead => stream.CanRead;

        public override bool CanSeek => stream.CanSeek;

        public override bool CanWrite => stream.CanWrite;

        public override long Length => stream.Length;

        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public override void Flush()
        {
            stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = stream.Read(buffer, offset, count);
            if (read <= 0)
            {
                httpTcpConnection.Using = false;
            }
            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
        }
    }
}
