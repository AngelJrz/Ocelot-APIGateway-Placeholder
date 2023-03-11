# Ocelot-APIGateway-Placeholder
This is a reference example to build a Ocelot APIGateway 

## Concepts
- GlobalConfiguration: default setting that are valid to all routes.
- UPSTREAM: Is used to handle the incomming request and then routing it to the downstream path.
- DOWNSTREAM: The route where the incomming request will go.

## Wildcard
We can use a wildcard like this to accept all the inputs (this can be dangerous if you dont handle all the possible cases).
Examples:
- /api/posts
- /api/posts/1
- /api/photos

```json
    {
      "UpstreamPathTemplate": "/api/{wildcard}",
      "UpstreamHttpMethod": [], <-- With this you accept all the methods (PUT, GET, POST, DELETE)

      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "jsonplaceholder.typicode.com",
          "Port": 80
        }
      ],
      "DownstreamPathTemplate": "/{wildcard}"
    }
```
> This can lead to possible errors (ocelot doesn't recognize certain endpoints)

## Aggregates
With this you can mix paths to return multiple information with one request
```json
{
  "Aggregates": [
    {
      "UpstreamPathTemplate": "/getall", <-- This route must be different to "/api/" because ocelot could confuse between aggregates and routing
      "RouteKeys": [ "posts", "photos" ]
    }
  ],
  "Routes": [
```

> Error: The aggregate may return corrupted data, this is because ocelot use "Accept-encoding" header.

> Solution: You have to create a handler (RemoveEncodingDelegatingHandler) to remove the "Accept-encoding" encode and add it to Startup or Program

## Aggregate with code
You can use code to make responses more dinamyc, for example, in the class GetAllAggregator.cs the API returns all the users with their post using two calls to the backend and mixing the responses in one structure. 
 > With simple aggregates your return directly the two request made to the backend.

## Authenticate
> You can put this in the global config

You can use JWT to secure your API calls, you need to make a middleware with the nuget "Microsoft.AspNetCore.Authentication.JwtBearer" and add this method:
```csharp
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


app.UseAuthentication();
```
After specify the auth method, secure your endpoint with this code
```json
    {
      "UpstreamPathTemplate": "/api/getusers",
      "UpstreamHttpMethod": [ "Get"],

      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "jsonplaceholder.typicode.com",
          "Port": 80
        }
      ],
      "DownstreamPathTemplate": "/users",
      "Key": "users",
      "AuthenticationOptions": { <-- Specify the auth method
        "AuthenticationProviderKey": "Bearer"
      }
    }
```
This enables you to create only one implementation of security for all your services, because all request will pass through this gateway.

## Authorization
> You can put this in the global config

In this case, you can put a role in your JWT and ocelot requires that role to take your request as valid.
```json
    {
      "UpstreamPathTemplate": "/api/getusers",
      "UpstreamHttpMethod": [ "Get"],

      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "jsonplaceholder.typicode.com",
          "Port": 80
        }
      ],
      "DownstreamPathTemplate": "/users",
      "Key": "users",
      "AuthenticationOptions": {
		"AuthenticationProviderKey": "Bearer"
      },
      "RouteClaimsRequirement": { 
        "userType": "myRole" <-- Here you put the JWT role
      }
    }
```

## Rate limit
> You can put this in the global config

You can limit the the request number with this property:
```json
    {
      "UpstreamPathTemplate": "/api/getusers",
      "UpstreamHttpMethod": [ "Get"],

      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "jsonplaceholder.typicode.com",
          "Port": 80
        }
      ],
      "DownstreamPathTemplate": "/users",
      "Key": "users",
      "RateLimitOptions": {
        "ClientWhitelist": [],
        "EnableRateLimiting": true,
        "Period": "3s",             <-- Period where you can make a number of requests
        "PeriodTimespan": 5,        <-- If the request fails, you can try after this seconds
        "Limit": 1                  <-- Number of requests you can make in a period
      }
    }
```

> If fails you get an error like this "API calls quota exceeded! maximum admitted 1 per 3s."
