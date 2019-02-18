using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using AsyncHttp.Extension;

namespace AsyncHttp.Streams
{
    public class HttpChunkedStream : System.IO.Stream
    {
        public System.IO.Stream NetworkStream { get; }
        public HttpChunkedStream(System.IO.Stream networkStream)
        {
            NetworkStream = networkStream;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }


        private readonly byte[] CRLF = new byte[] { 13, 10 };

        public override void Flush()
        {
        }

        private byte[] lastPacket = new byte[0];
        private int needToReadCount = 0;
        private bool readToEnd = false;
        private int uselessLength = 0;
        private int totalRead = 0;
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (readToEnd)
            {
                return 0;
            }
            //[chunk size][\r\n][chunk data][\r\n][chunk size][\r\n][chunk data][\r\n][chunk size = 0][\r\n][\r\n]
            var innerBuffer = new byte[count];

            var readCount = NetworkStream.Read(innerBuffer, offset, count);
            totalRead += readCount;
            var combineDataArray = new byte[lastPacket.Length + readCount];
            Array.Copy(lastPacket, 0, combineDataArray, 0, lastPacket.Length);
            Array.Copy(innerBuffer, 0, combineDataArray, lastPacket.Length, readCount);

            var readIndex = 0;
            var writeCount = 0;
            if (needToReadCount > 0)
            {
                var realReadNum = Math.Min(needToReadCount, readCount);
                Array.Copy(combineDataArray, 0, buffer, 0, realReadNum);
                writeCount += realReadNum;
                needToReadCount -= realReadNum;
                if (needToReadCount > 0)
                {
                    return realReadNum;
                }
                else
                {
                    readIndex += realReadNum;
                    uselessLength = CRLF.Length;
                }
            }

            var leftSize = readCount - readIndex;
            if (leftSize >= uselessLength)
            {
                readIndex += uselessLength;
                uselessLength = 0;
            }
            else
            {
                uselessLength -= leftSize;
                if (readIndex > 0)
                {
                    return readIndex;
                }
                else
                {
                    return Read(buffer, offset, count);
                }
            }


            while (true)
            {
                leftSize = readCount - readIndex;

                var crlfIndex = combineDataArray.IndexOf(CRLF, startIndex: readIndex);
                if (crlfIndex == -1)
                {
                    lastPacket = new byte[leftSize];
                    Array.Copy(combineDataArray, readIndex, lastPacket, 0, leftSize);
                    if (writeCount == 0)
                    {
                        return Read(buffer, offset, count);
                    }
                    else
                    {
                        return writeCount;
                    }
                }
                var stringLength = crlfIndex - readIndex;
                var hexString = Encoding.UTF8.GetString(combineDataArray, readIndex, stringLength);
                var packetLength = int.Parse(hexString, System.Globalization.NumberStyles.HexNumber);

                //TODO parse error handle
                var packetLeftSize = leftSize - stringLength - CRLF.Length;
                if (packetLength == 0 && packetLeftSize == CRLF.Length)
                {
                    //last packet should end with \r\n\r\n 
                    //finish
                    readToEnd = true;
                    return writeCount;
                }
                if (packetLeftSize < packetLength + CRLF.Length)
                {
                    Array.Copy(combineDataArray, readIndex + stringLength + CRLF.Length, buffer, writeCount, packetLeftSize);
                    needToReadCount = packetLength - packetLeftSize;
                    writeCount += packetLeftSize;
                    break;
                }
                else
                {
                    Array.Copy(combineDataArray, readIndex + stringLength + CRLF.Length, buffer, writeCount, packetLength);
                    readIndex += packetLength + stringLength + CRLF.Length * 2;
                    writeCount += packetLength;
                }
            }

            return writeCount;
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
