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
