using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AsyncHttp.Entity
{
    public class HttpRequestBody : IHttpRequestBody
    {
        private readonly byte[] data;

        public HttpRequestBody(string contentType, byte[] data)
        {
            this.ContentType = contentType;
            this.data = data;
            ContentLength = data.Length;
        }

        public HttpRequestBody(string contentType, string data) : this(contentType, Encoding.UTF8.GetBytes(data))
        {

        }
        public string ContentType { get; set; }
        public int ContentLength { get; set; }

        public void WriteBody(Stream bodyStream)
        {
            bodyStream.Write(data, 0, data.Length);
        }
    }
}
