using AsyncHttp.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using AsyncHttp.Enums;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using AsyncHttp.Entity;
using AsyncHttp.Extension;

namespace AsyncHttp.Http
{

    public class HttpConnection
    {
        private readonly HttpConnectionPool httpConnectionPool;
        private HttpTcpConnection httpTcpConnection;
        public HttpConnection(HttpConnectionPool httpConnectionPool)
        {
            this.httpConnectionPool = httpConnectionPool;
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

        public async Task SendRequestAsync(HttpRequest httpRequest)
        {
            var isSsl = httpRequest.Uri.Scheme == "https";
            this.httpTcpConnection = httpConnectionPool.GetConnection(httpRequest.Uri.Host, httpRequest.Uri.Port, isSsl);
            if (!httpTcpConnection.Connected)
            {
                await httpTcpConnection.ConnectAsync(httpRequest.Uri.Host, httpRequest.Uri.Port);
                if (isSsl)
                {
                    SslStream sslStream = new SslStream(httpTcpConnection.NetworkStream, false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                    httpTcpConnection.NetworkStream = sslStream;
                    await sslStream.AuthenticateAsClientAsync(httpRequest.Uri.Host);
                }
            }


            if (httpRequest.Body != null && !httpRequest.Headers.ContainsKey("Content-Type"))
            {
                httpRequest.Headers["Content-Type"] = httpRequest.Body.ContentType;
            }
            if (httpRequest.Body != null && !httpRequest.Headers.ContainsKey("Content-Length"))
            {
                httpRequest.Headers["Content-Length"] = httpRequest.Body.ContentLength.ToString();
            }
            var cmd = httpRequest.ToHttpCommand();
            var data = Encoding.UTF8.GetBytes(cmd);
            var reqStream = new MemoryStream(data);
            await httpTcpConnection.SendDataAsync(reqStream);
            if (httpRequest.Body != null)
            {
                httpRequest.Body?.WriteBody(httpTcpConnection.NetworkStream);
            }
        }

        private readonly byte[] HEADER_BODY_SPLIT = new byte[] { 13, 10, 13, 10 };// \r\n\r\n

        public async Task<HttpResponse> ReadResponseAsync()
        {
            var bufferSize = 100;
            var buffer = new byte[bufferSize];
            var data = new byte[0];
            var findSplitHeader = false;
            var splitIndex = -1;
            while (!findSplitHeader)
            {
                var length = await httpTcpConnection.NetworkStream.ReadAsync(buffer, 0, buffer.Length);
                if (length > 0)
                {
                    var newData = new byte[data.Length + length];
                    Array.Copy(data, newData, data.Length);
                    Array.Copy(buffer, 0, newData, data.Length, length);
                    data = newData;
                    splitIndex = data.IndexOf(HEADER_BODY_SPLIT, startIndex: 0);
                    if (splitIndex == -1)
                    {
                        continue;
                    }
                    else
                    {
                        findSplitHeader = true;
                    }
                }
                else
                {
                    throw new Exception("WTF........!!!!!!!!!!");
                }
            }

            var httpResponse = new HttpResponse();
            var headerData = data.Take(splitIndex).ToArray();
            var bodyData = data.Skip(splitIndex + HEADER_BODY_SPLIT.Length).ToArray();
            var headers = Encoding.UTF8.GetString(headerData);
            var lines = headers.Split("\r\n");
            var httpStatusString = lines.First().Split(' ', 3);
            httpResponse.HttpVersion = httpStatusString[0];
            httpResponse.HttpCode = httpStatusString[1];
            httpResponse.HttpStatus = httpStatusString[2];
            for (int i = 1; i < lines.Length; i++)
            {
                var header = lines[i];
                var split = header.Split(':', 2);
                if (String.Compare("Content-Length", split[0], false) == 0)
                {
                    httpResponse.ContentLength = int.Parse(split[1]);
                }
                else if (String.Compare("Transfer-Encoding", split[0], false) == 0)
                {
                    var encodings = split[1].Split(',');
                    httpResponse.TransferEncoding = encodings.Select(enc => Enum.Parse<TransferEncoding>(enc, true)).ToList();
                }
                httpResponse.Headers.Add(split[0], split[1]);
            }
            if (httpResponse.TransferEncoding.Contains(TransferEncoding.Chunked))
            {
                httpResponse.BodyStream = new ContentStreamWrap(new HttpChunkedStream(httpTcpConnection.NetworkStream, bodyData), httpTcpConnection);
            }
            else
            {
                httpResponse.BodyStream = new ContentStreamWrap(new HttpContentStream(httpTcpConnection.NetworkStream, httpResponse.ContentLength, bodyData), httpTcpConnection);
            }

            return httpResponse;
        }

    }

}
