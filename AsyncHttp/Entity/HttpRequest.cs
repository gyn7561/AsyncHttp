using System;
using System.Collections.Generic;
using System.Text;

namespace AsyncHttp.Entity
{
    //https://tools.ietf.org/html/rfc7540
    public class HttpRequest
    {
        public string Method { get; set; } = "GET";
        public string Version { get; set; } = "HTTP/1.1";
        public Uri Uri { get; set; }
        public HttpHeaders Headers { get; set; } = new HttpHeaders();
        public IHttpRequestBody Body { get; set; }

        public String ToHttpCommand()
        {
            var commandBuilder = new StringBuilder();
            var headersClone = Headers.Clone() as HttpHeaders;
            if (!headersClone.ContainsKey("Host"))
            {
                headersClone["Host"] = Uri.Host;
            }
            commandBuilder.Append($"{Method} {Uri.ToString()} {Version}\r\n");

            foreach (var kv in headersClone.Headers())
            {
                commandBuilder.Append($"{kv.Key}: {kv.Value}\r\n");
            }
            commandBuilder.Append("\r\n");
            return commandBuilder.ToString();
        }

    }
}
