# AsyncHttp
a .net core tcp socket implemented http client 一个用原生TCP实现的 http client

```
var request = new HttpRequest();
request.Method = "POST";
request.Uri = new Uri("http://www.w3school.com.cn/tiy/v.asp");
request.Version = "HTTP/1.1";
request.Headers["Connection"] = "keep-alive";
request.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";
request.Headers["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
request.Headers["Accept-Language"] = "zh-CN,zh;q=0.9";
request.Headers["Accept-Encoding"] = "gzip, deflate";
request.Headers["Content-Type"] = "application/x-www-form-urlencoded";
request.Body = new HttpRequestBody("application/x-www-form-urlencoded", "code=%3C%21DOCTYPE+HTML%3E%0D%0A%3Chtml%3E%0D%0A%3Cbody%3E%0D%0A%0D%0A%3Cform+actionw3equalsign%22%2Fexample%2Fhtml5%2Fdemo_form.asp%22+methodw3equalsign%22get%22%3E%0D%0AUsername%3A+%3Cinput+typew3equalsign%22text%22+namew3equalsign%22usr_name%22+%2F%3E%0D%0AEncryption%3A+%3Ckeygen+namew3equalsign%22security%22+%2F%3E%0D%0A%3Cinput+typew3equalsign%22submit%22+%2F%3E%0D%0A%3C%2Fform%3E%0D%0A%0D%0A%3C%2Fbody%3E%0D%0A%3C%2Fhtml%3E%0D%0A&bt=");
TestContext.WriteLine(request.ToHttpCommand());
TestContext.WriteLine("-------");
var conn = new AsyncHttp.Http.HttpConnection();
await conn.SendRequest(request);
var res = await conn.ReadResponse();
var memoryStream = new MemoryStream();
TestContext.WriteLine(res.Headers.ToHttpCommandString());
await res.BodyContentStream.CopyToAsync(memoryStream);
var str = Encoding.GetEncoding("utf-8").GetString(memoryStream.ToArray());
TestContext.WriteLine(str);
```

### TEST RESULT
```
POST http://www.w3school.com.cn/tiy/v.asp HTTP/1.1
Connection: keep-alive
User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8
Accept-Language: zh-CN,zh;q=0.9
Accept-Encoding: gzip, deflate
Content-Type: application/x-www-form-urlencoded
Host: www.w3school.com.cn


-------
Cache-Control:  private
Content-Type:  text/html; Charset=GB2312
Server:  Microsoft-IIS/10.0
Set-Cookie:  ASPSESSIONIDCCDCSQRT=JPPCKHABFNMIAMGALIOPNMMM; path=/
X-Powered-By:  ASP.NET
Date:  Mon, 18 Feb 2019 10:30:27 GMT
Content-Length:  237

<!DOCTYPE HTML>
<html>
<body>

<form action="/example/html5/demo_form.asp" method="get">
Username: <input type="text" name="usr_name" />
Encryption: <keygen name="security" />
<input type="submit" />
</form>

</body>
</html>
```
