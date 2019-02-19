# AsyncHttp
a .net core tcp socket implemented http client 一个用原生TCP实现的 http client

##Usage
```

var request = new HttpRequest();
request.Uri = new Uri("https://www.github.com");

//sync style
var html = asyncHttpClient.GetString(request);
Console.WriteLine(html);

//async style
var html = await asyncHttpClient.GetStringAsync(request);
Console.WriteLine(html);

//callback style
asyncHttpClient.Execute(request,(res)=>{
    res.BodyStream.ReadAsString((html) =>
    {
        Console.WriteLine(html);
    });
});

```