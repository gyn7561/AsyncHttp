using AsyncHttp.Entity;
using AsyncHttp.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AsyncHttp.Test
{
    [TestClass]
    public class Test
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
        private HttpConnectionPool httpConnectionPool = new HttpConnectionPool();

        [TestMethod]
        public void Http()
        {

            //            var cmd = @"GET http://www.w3school.com.cn/ HTTP/1.1
            //Host: www.w3school.com.cn
            //Connection: keep-alive
            //Cache-Control: max-age=0
            //Upgrade-Insecure-Requests: 1
            //User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36
            //Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8
            //Accept-Encoding: deflate
            //Accept-Language: zh-CN,zh;q=0.9

            //";
            var request = new HttpRequest();
            request.Uri = new Uri("http://www.w3school.com.cn/");
            request.Version = "HTTP/1.1";
            request.Headers["Connection"] = "keep-alive";
            request.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";
            request.Headers["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.Headers["Accept-Language"] = "zh-CN,zh;q=0.9";
            request.Headers["Accept-Encoding"] = "gzip, deflate";

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            async Task temp()
            {
                TestContext.WriteLine(request.ToHttpCommand());
                TestContext.WriteLine("-------");
                var conn = new AsyncHttp.Http.HttpConnection(httpConnectionPool);
                await conn.SendRequest(request);
                var res = await conn.ReadResponse();
                var memoryStream = new MemoryStream();
                TestContext.WriteLine(res.Headers.ToHttpCommandString());
                await res.BodyContentStream.CopyToAsync(memoryStream);
                var str = Encoding.GetEncoding("utf-8").GetString(memoryStream.ToArray());
                TestContext.WriteLine(str);
            }

            var s = temp();
            s.Wait();

        }

        [TestMethod]
        public void Https()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var request = new HttpRequest();
            request.Uri = new Uri("https://www.baidu.com");
            request.Version = "HTTP/1.1";
            request.Headers["Connection"] = "keep-alive";
            request.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";
            request.Headers["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.Headers["Accept-Language"] = "zh-CN,zh;q=0.9";
            request.Headers["Accept-Encoding"] = "gzip, deflate";

            async Task temp()
            {
                TestContext.WriteLine(request.ToHttpCommand());
                TestContext.WriteLine("-------");
                var conn = new AsyncHttp.Http.HttpConnection(httpConnectionPool);
                await conn.SendRequest(request);
                var res = await conn.ReadResponse();
                var memoryStream = new MemoryStream();
                TestContext.WriteLine(res.Headers.ToHttpCommandString());
                await res.BodyContentStream.CopyToAsync(memoryStream);
                var str = Encoding.GetEncoding("utf-8").GetString(memoryStream.ToArray());
                TestContext.WriteLine(str);
                //File.WriteAllBytes("test.gz", memoryStream.ToArray());
            }
            var s = temp();
            s.Wait();

        }

        [TestMethod]
        public void Post()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var request = new HttpRequest();
            request.Method = "POST";
            request.Uri = new Uri("http://www.w3school.com.cn/tiy/v.asp");
            request.Version = "HTTP/1.1";
            request.Headers["Connection"] = "keep-alive";
            request.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";
            request.Headers["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.Headers["Accept-Language"] = "zh-CN,zh;q=0.9";
            request.Headers["Accept-Encoding"] = "gzip, deflate";
            request.Body = new HttpRequestBody("application/x-www-form-urlencoded", "code=%3C%21DOCTYPE+HTML%3E%0D%0A%3Chtml%3E%0D%0A%3Cbody%3E%0D%0A%0D%0A%3Cform+actionw3equalsign%22%2Fexample%2Fhtml5%2Fdemo_form.asp%22+methodw3equalsign%22get%22%3E%0D%0AUsername%3A+%3Cinput+typew3equalsign%22text%22+namew3equalsign%22usr_name%22+%2F%3E%0D%0AEncryption%3A+%3Ckeygen+namew3equalsign%22security%22+%2F%3E%0D%0A%3Cinput+typew3equalsign%22submit%22+%2F%3E%0D%0A%3C%2Fform%3E%0D%0A%0D%0A%3C%2Fbody%3E%0D%0A%3C%2Fhtml%3E%0D%0A&bt=");
            async Task temp()
            {
                TestContext.WriteLine(request.ToHttpCommand());
                TestContext.WriteLine("-------");
                var conn = new AsyncHttp.Http.HttpConnection(httpConnectionPool);
                await conn.SendRequest(request);
                var res = await conn.ReadResponse();
                var memoryStream = new MemoryStream();
                TestContext.WriteLine(res.ToHttpCommandString());
                await res.BodyContentStream.CopyToAsync(memoryStream);
                var str = Encoding.GetEncoding("utf-8").GetString(memoryStream.ToArray());
                TestContext.WriteLine(str);
                //File.WriteAllBytes("test.gz", memoryStream.ToArray());
            }
            var s = temp();
            s.Wait();

        }
    }
}
