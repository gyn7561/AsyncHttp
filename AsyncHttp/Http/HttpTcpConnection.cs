using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AsyncHttp.Http
{
    internal class HttpTcpConnection : IDisposable
    {
        private TcpClient tcpClient = new TcpClient();

        public Stream NetworkStream { get; internal set; }

        public Boolean Using { get; set; }

        public HttpTcpConnection()
        {
        }

        public Boolean Connected { get { return tcpClient.Connected; } }

        public async Task ConnectAsync(string host, int port)
        {

            await tcpClient.ConnectAsync(host, port);
            NetworkStream = tcpClient.GetStream();
        }

        public async Task SendData(System.IO.Stream stream)
        {
            await stream.CopyToAsync(NetworkStream);
        }

        public void Dispose()
        {
            NetworkStream.Dispose();
            tcpClient.Dispose();
        }
    }
}
