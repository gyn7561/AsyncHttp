using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AsyncHttp.Http
{
    public class HttpConnectionPool
    {

        private struct HostAndPort
        {
            public string Host { get; set; }
            public int Port { get; set; }
            public bool Ssl { get; set; }
        }

        private Dictionary<HostAndPort, Queue<HttpTcpConnection>> connections = new Dictionary<HostAndPort, Queue<HttpTcpConnection>>();

        public HttpConnectionPool()
        {

        }

        internal HttpTcpConnection GetConnection(string host, int port, bool ssl)
        {
            var key = new HostAndPort() { Host = host, Port = port, Ssl = ssl };
            lock (this)
            {
                if (!connections.ContainsKey(key))
                {
                    connections.Add(key, new Queue<HttpTcpConnection>());
                }
            }
            var queue = connections[key];
            lock (queue)
            {
                if (queue.Any())
                {
                    var readCount = 0;
                    while (readCount < queue.Count)
                    {
                        readCount++;
                        var conn = queue.Dequeue();
                        if (!conn.Using && conn.Connected)
                        {
                            queue.Enqueue(conn);
                            conn.Using = true;
                            return conn;
                        }
                        if (conn.Using || conn.Connected)
                        {
                            queue.Enqueue(conn);
                        }
                    }
                }
                var newConn = new HttpTcpConnection() { Using = true };
                queue.Enqueue(newConn);
                return newConn;
            }
        }
    }
}
