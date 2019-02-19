using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AsyncHttp.Streams
{
    internal class CombinedStream : Stream
    {
        private readonly IEnumerator<Stream> list;

        internal CombinedStream(IEnumerator<Stream> list)
        {
            this.list = list;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (list.Current == null)
            {
                var hasNext = list.MoveNext();
                if (!hasNext)
                {
                    return -1;
                }
            }
            var len = list.Current.Read(buffer, offset, count);
            if (len <= 0)
            {
                var hasNext = list.MoveNext();
                if (!hasNext)
                {
                    return -1;
                }
                else
                {
                    return Read(buffer, offset, count);
                }
            }
            else
            {
                return len;
            }
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
