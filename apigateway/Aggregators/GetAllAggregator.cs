using Newtonsoft.Json;
using Ocelot.Middleware;
using Ocelot.Multiplexer;
using ocelotgateway.Dtos;

namespace ocelotgateway.Aggregators
{
    public class GetAllAggregator : IDefinedAggregator
    {
        public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
        {
            var postsResponseContent = await responses[0].Items.DownstreamResponse().Content.ReadAsStringAsync();
            var usersResponseContent = await responses[1].Items.DownstreamResponse().Content.ReadAsStringAsync();

            var posts = JsonConvert.DeserializeObject<List<Post>>(postsResponseContent);
            var users = JsonConvert.DeserializeObject<List<User>>(usersResponseContent);

            foreach (var user in users)
            {
                var userPost = posts.Where(post => post.UserId == user.Id).ToList();
                user.Posts.AddRange(userPost);
            }

            var postByUserString = JsonConvert.SerializeObject(users);

            var stringContent = new StringContent(postByUserString)
            {
                Headers = { ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json") }
            };

            return new DownstreamResponse(stringContent, System.Net.HttpStatusCode.OK, new List<KeyValuePair<string, IEnumerable<string>>>(), "Ok");
        }
    }
}
