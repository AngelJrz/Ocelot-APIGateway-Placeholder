# Ocelot-APIGateway-Placeholder
This is a reference example to build a Ocelot APIGateway 

### Concepts
- GlobalConfiguration: default setting that are valid to all routes.
- UPSTREAM: Is used to handle the incomming request and then routing it to the downstream path.
- DOWNSTREAM: The route where the incomming request will go.

### Wildcard
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
