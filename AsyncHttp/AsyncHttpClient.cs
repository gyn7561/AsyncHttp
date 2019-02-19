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
        public async Task<HttpResponse> ExecuteAsync(HttpRequest httpRequest)
        {
            HttpConnection httpConnection = new HttpConnection(httpConnectionPool);
            await httpConnection.SendRequestAsync(httpRequest);
            return await httpConnection.ReadResponseAsync();
        }

        public void Execute(HttpRequest httpRequest, Action<HttpResponse> callback)
        {
            this.ExecuteAsync(httpRequest).ContinueWith(t =>
            {
                callback(t.Result);
            });
        }

        public HttpResponse Execute(HttpRequest httpRequest)
        {
            var responseTask = ExecuteAsync(httpRequest);
            responseTask.Wait();
            return responseTask.Result;
        }

        public async Task<HttpResponse> GetAsync(string url)
        {
            var uri = new Uri(url);
            return await ExecuteAsync(new HttpRequest()
            {
                Uri = uri
            });
        }

        public async Task<String> GetStringAsync(string url)
        {
            var res = await this.GetAsync(url);
            return await res.BodyStream.ReadAsStringAsync();
        }

        public async Task<String> GetStringAsync(HttpRequest httpRequest)
        {
            var res = this.Execute(httpRequest);
            return await res.BodyStream.ReadAsStringAsync();
        }

        public String GetString(string url)
        {
            var task = this.GetStringAsync(url);
            task.Wait();
            return task.Result;
        }

        public String GetString(HttpRequest httpRequest)
        {
            var task = this.GetStringAsync(httpRequest);
            task.Wait();
            return task.Result;
        }

        public async Task<byte[]> GetByteArrayAsync(string url)
        {
            var res = await this.GetAsync(url);
            return await res.BodyStream.ReadAsByteArrayAsync();
        }

        public async Task<byte[]> GetByteArrayAsync(HttpRequest httpRequest)
        {
            var res = this.Execute(httpRequest);
            return await res.BodyStream.ReadAsByteArrayAsync();
        }

        public byte[] GetByteArray(string url)
        {
            var task = this.GetByteArrayAsync(url);
            task.Wait();
            return task.Result;
        }

        public byte[] GetByteArray(HttpRequest httpRequest)
        {
            var task = this.GetByteArrayAsync(httpRequest);
            task.Wait();
            return task.Result;
        }
    }
}
