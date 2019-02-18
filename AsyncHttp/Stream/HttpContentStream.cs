using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AsyncHttp.Streams
{
    public class HttpContentStream : System.IO.Stream
    {
        public Stream NetworkStream { get; }
        public int ContentLength { get; }
        public HttpContentStream(System.IO.Stream networkStream, int contentLength)
        {
            NetworkStream = networkStream;
            ContentLength = contentLength;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => ContentLength;

        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }


        public override void Flush()
        {
        }

        int totalReadCount = 0;
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (totalReadCount >= ContentLength)
            {
                return 0;
            }
            var readCount = NetworkStream.Read(buffer, offset, count);
            totalReadCount += readCount;
            return readCount;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
