using AsyncHttp.Enums;
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
        public Stream BodyContentStream { get; internal set; }
        public List<TransferEncoding> TransferEncoding { get; internal set; } = new List<TransferEncoding>();
        public HttpHeaders Headers { get; internal set; } = new HttpHeaders();

        public async Task<string> String(string encoding = "utf-8")
        {
            return Encoding.UTF8.GetString(await Bytes());
        }

        public async Task<byte[]> Bytes()
        {
            var memoryStream = new MemoryStream();
            await BodyContentStream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

    }
}
