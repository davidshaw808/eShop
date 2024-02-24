
var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("getorder", (Guid orderId, HttpRequest request) => {
    /*TODO*/
    var bearerToken = request.Headers.Authorization;
}).RequireAuthorization(/*add policies*/);

app.Run();