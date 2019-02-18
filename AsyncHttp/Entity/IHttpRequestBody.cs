using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AsyncHttp.Entity
{
    public interface IHttpRequestBody
    {
        string ContentType { get; set; }
        int ContentLength { get; set; }
        void WriteBody(System.IO.Stream bodyStream);
    }
}
