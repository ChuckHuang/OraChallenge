using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OraChallenge.API.Models;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using OraChallenge.API.JsonApi;
using JsonApiFramework.JsonApi;

namespace OraChallenge.API.Controllers
{
    [Produces("application/vnd.api+json")]
    [Route("api/Sessions")]
    public class SessionsController : Controller
    {
        private readonly OraChallengeDBContext _context;
        public SessionsController(OraChallengeDBContext context)
        {
            _context = context;
        }

        // POST: api/Messages
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ResourceDocument document)
        {
            try
            {
                var username = CreateRandomUsername();

                var user = new User() { UserName = username };
                _context.User.Add(user);
                await _context.SaveChangesAsync();
                var token = SiteAuthorizationExtensions.CreateJwtToken(user.Id.ToString());

                this.Response.Headers.Add("Authorization", "Bearer " + token);
               

                var session = new Session()
                {
                    Id = user.Id,
                    CreatedAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                    User = user
                };

                var currentRequestUrl = this.HttpContext.Request.GetUri();

                // Build new document.
                var newDocument = JsonApiUtil.WriteDocumentForSessionsPostResponse(currentRequestUrl, session);
                return Created(currentRequestUrl.AbsoluteUri, newDocument);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

       
        private string CreateRandomUsername()
        {
            var random = new Random();

            var randomAlphanumericLength = random.Next(1, 6);
            string username = "user";
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            for (int i = 0; i < randomAlphanumericLength; i++)
            {
                username += chars[random.Next(chars.Length)];
            }

            return username;
        }
    }
}