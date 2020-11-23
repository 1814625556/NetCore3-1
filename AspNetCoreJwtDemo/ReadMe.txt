1:
	Install-Package Microsoft.AspNetCore.Authentication.JwtBearer -Version 3.1.10
2:
	配置 action 路由的方式-两种
	[HttpGet("hellocc")]
    [HttpGet,Route("helloxx")]
3:
	[Authorize] 属性既可以 用在 class上 也可以用在 action上
4:
	设置 Header中的 Authorization
	Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiY2hlbmNoYW5nIiwibmJmIjoxNjA2MTE4NTcxLCJleHAiOjE2MDYxMTg1ODEsImlzcyI6ImNoZW5jaGFuZyIsImF1ZCI6ImR5In0.inkSlj-a8hsrcUDE4fvl4hmN5tGMBcmr75ULnMtq8Eg


