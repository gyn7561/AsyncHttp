using AsyncHttp.Entity;
using AsyncHttp.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AsyncHttp
{
    public class AsyncHttpClient
    {
        private HttpConnectionPool httpConnectionPool = new HttpConnectionPool();
        public async Task<HttpResponse> Execute(HttpRequest httpRequest)
        {
            HttpConnection httpConnection = new HttpConnection(httpConnectionPool);
            await httpConnection.SendRequest(httpRequest);
            return await httpConnection.ReadResponse();
        }
    }
}
