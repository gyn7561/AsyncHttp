using AsyncHttp.Enums;
using AsyncHttp.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AsyncHttp.Entity
{
    public class HttpResponse
    {
        public int ContentLength { get; internal set; } = -1;
        public string HttpVersion { get; internal set; }
        public string HttpStatus { get; internal set; }
        public string HttpCode { get; internal set; }
        public ContentStreamWrap BodyStream { get; internal set; }
        public List<TransferEncoding> TransferEncoding { get; internal set; } = new List<TransferEncoding>();
        public HttpHeaders Headers { get; internal set; } = new HttpHeaders();

        public string ToHttpCommandString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"{HttpVersion} {HttpCode} {HttpStatus}\r\n");
            stringBuilder.Append(Headers.ToHttpCommandString());
            return stringBuilder.ToString();
        }


    }
}
