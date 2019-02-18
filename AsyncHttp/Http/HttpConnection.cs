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

namespace AsyncHttp.Http
{

    public class HttpConnection
    {
        private HttpTcpConnection httpTcpConnection;

        public HttpConnection()
        {
            httpTcpConnection = new HttpTcpConnection();
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

        public async Task SendRequest(HttpRequest httpRequest)
        {
            await httpTcpConnection.ConnectAsync(httpRequest.Uri.Host, httpRequest.Uri.Port);
            if (httpRequest.Uri.Scheme == "https")
            {
                SslStream sslStream = new SslStream(httpTcpConnection.NetworkStream, false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                httpTcpConnection.NetworkStream = sslStream;
                await sslStream.AuthenticateAsClientAsync(httpRequest.Uri.Host);
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
            await httpTcpConnection.SendData(reqStream);
            if (httpRequest.Body != null)
            {
                httpRequest.Body?.WriteBody(httpTcpConnection.NetworkStream);
            }
        }

        private readonly byte[] HEADER_BODY_SPLIT = new byte[] { 13, 10, 13, 10 };// \r\n\r\n


        public async Task<HttpResponse> ReadResponse()
        {
            var data = new List<byte>();
            var oneByteBuffer = new byte[1];
            var findSplitHeader = false;
            while (!findSplitHeader)
            {
                var length = await httpTcpConnection.NetworkStream.ReadAsync(oneByteBuffer, 0, oneByteBuffer.Length);
                if (length == 1)
                {
                    data.Add(oneByteBuffer[0]);
                    if (data.Count >= 4)
                    {
                        findSplitHeader = HEADER_BODY_SPLIT[0] == data[data.Count - 4] &&
                            HEADER_BODY_SPLIT[1] == data[data.Count - 3] &&
                            HEADER_BODY_SPLIT[2] == data[data.Count - 2] &&
                            HEADER_BODY_SPLIT[3] == data[data.Count - 1];
                    }
                }
                else
                {
                    throw new Exception("WTF........!!!!!!!!!!");
                }
            }

            var httpResponse = new HttpResponse();
            var headerData = data.Take(data.Count - HEADER_BODY_SPLIT.Length).ToArray();
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
                httpResponse.Headers[split[0]] = split[1];
            }
            if (httpResponse.TransferEncoding.Contains(TransferEncoding.Chunked))
            {
                httpResponse.BodyContentStream = new HttpChunkedStream(httpTcpConnection.NetworkStream);
            }
            else
            {
                httpResponse.BodyContentStream = new HttpContentStream(httpTcpConnection.NetworkStream, httpResponse.ContentLength);
            }

            return httpResponse;
        }
    }

}
