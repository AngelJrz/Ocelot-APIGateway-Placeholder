using System.Text;
using Ocelot.Middleware;
using ocelotgateway.Handlers;
using ocelotgateway.Aggregators;
using Ocelot.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
	.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
	.AddEnvironmentVariables();

builder.Services.AddOcelot(builder.Configuration)
	.AddDelegatingHandler<RemoveEncodingDelegatingHandler>(true) //True is because you use this for all the request (global)
	.AddSingletonDefinedAggregator<GetAllAggregator>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // When you specify an endpoint to authenticate, the request needs aprove this auth
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			ValidateIssuer = false,
			ValidateAudience = false,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("MyAnonymousSecretKey")),
			ClockSkew = new TimeSpan(0)
		};
	});

var app = builder.Build();

await app.UseOcelot();
app.UseAuthentication();
app.Run();
