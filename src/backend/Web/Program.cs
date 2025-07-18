using FiTrack.Application;
using FiTrack.Infrastructure;
using FiTrack.Infrastructure.Data;
using Shared.Extensions;
using Web;
using Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);



builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();
builder.AddSharedAuthorization();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}
else
{   
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseMiddleware<SwaggerAuthMiddleware>();
}



app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSwaggerUi(settings =>
{
    settings.Path = "/api";
    settings.DocumentPath = "/api/specification.json";
});


app.UseExceptionHandler(options => { });

app.UseAuthentication();
app.UseAuthorization();

app.Map("/", () => Results.Redirect("/api"));

app.MapEndpoints();

app.Run();

public partial class Program { }
