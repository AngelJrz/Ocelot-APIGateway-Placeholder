using Ocelot.Middleware;
using Ocelot.DependencyInjection;
using ocelotgateway.Handlers;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
	.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
	.AddEnvironmentVariables();
builder.Services.AddOcelot(builder.Configuration)
	.AddDelegatingHandler<RemoveEncodingDelegatingHandler>(true); //True is because you use this for all the request (global)

var app = builder.Build();
await app.UseOcelot();
app.Run();
