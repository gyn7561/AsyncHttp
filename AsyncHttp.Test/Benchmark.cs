using AsyncHttp.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncHttp.Test
{
    [TestClass]
    public class Benchmark
    {
        private TestContext testContextInstance;

        /// <summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [TestMethod]
        public void SingleThread()
        {
            var count = 100;
            async Task AsyncFunc()
            {
                var url = "http://www.w3school.com.cn/";
                HttpClient httpClient = new HttpClient();
                var start = DateTime.Now;
                for (int i = 0; i < count; i++)
                {
                    var data = await httpClient.GetByteArrayAsync(url);
                }
                var end = DateTime.Now;
                TestContext.WriteLine($"HttpClient request {count} times , cost {(end - start).TotalMilliseconds}ms");

                Thread.Sleep(2000);
                AsyncHttpClient asyncHttpClient = new AsyncHttpClient();
                start = DateTime.Now;
                for (int i = 0; i < count; i++)
                {
                    var request = new HttpRequest()
                    {
                        Uri = new Uri(url)
                    };
                    var res = await asyncHttpClient.ExecuteAsync(request);
                    var data = await res.BodyStream.ReadAsByteArrayAsync();
                }
                end = DateTime.Now;
                TestContext.WriteLine($"AsyncHttpClient request {count} times , cost {(end - start).TotalMilliseconds}ms");
            }
            var func = AsyncFunc();
            func.Wait();
        }

        [TestMethod]
        public void MultiThread()
        {
            var thread = 10;
            var count = 1000;
            var url = "https://www.baidu.com/";
            HttpClient httpClient = new HttpClient();
            var start = DateTime.Now;
            var allTask = new List<Task>();
            var counter = 0;
            for (int i = 0; i < thread; i++)
            {
                allTask.Add(Task.Run(async () =>
                {
                    while (true)
                    {
                        lock (this)
                        {
                            counter++;
                            if (counter > count + 1)
                            {
                                return;
                            }
                        }
                        var data = await httpClient.GetByteArrayAsync(url);
                    }
                }));
            }
            foreach (var task in allTask)
            {
                task.Wait();
            }
            var end = DateTime.Now;
            TestContext.WriteLine($"HttpClient {thread} threads request {count} times , cost {(end - start).TotalMilliseconds}ms");

            Thread.Sleep(2000);
            AsyncHttpClient asyncHttpClient = new AsyncHttpClient();
            start = DateTime.Now;

            allTask = new List<Task>();
            counter = 0;
            for (int i = 0; i < thread; i++)
            {
                allTask.Add(Task.Run(async () =>
                {
                    while (true)
                    {
                        lock (this)
                        {
                            counter++;
                            if (counter > count + 1)
                            {
                                return;
                            }
                        }
                        var request = new HttpRequest()
                        {
                            Uri = new Uri(url)
                        };
                        var res = await asyncHttpClient.ExecuteAsync(request);
                        var data = await res.BodyStream.ReadAsByteArrayAsync();
                    }
                }));
            }
            foreach (var task in allTask)
            {
                task.Wait();
            }
            end = DateTime.Now;
            TestContext.WriteLine($"AsyncHttpClient {thread} threads request {count} times , cost {(end - start).TotalMilliseconds}ms");
        }
    }
}
