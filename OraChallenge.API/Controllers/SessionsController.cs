using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OraChallenge.API.Models;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using OraChallenge.API.JsonApi;
using JsonApiFramework.JsonApi;
using JsonApiFramework.Server;

namespace OraChallenge.API.Controllers
{
    [Produces("application/json")]
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
            var username = CreateRandomUsername();

            var user = new User() { UserName = username };
            _context.User.Add(user);
            await _context.SaveChangesAsync();
            var token = SiteAuthorizationExtensions.CreateJwtToken(user.Id.ToString());

            this.Response.Headers.Add("Authorization", "Bearer " + token);

            var session = new Session()
            {
                Id = user.Id,
                CreatedAt = DateTime.Now,
                User = user
            };

            var currentRequestUrl = this.HttpContext.Request.GetUri();

            // Build new document.
            var newDocument = new OraChallengeDocumentContext(currentRequestUrl.Host, currentRequestUrl.Port).NewDocument(this.Request.GetUri())


                // Resource
                .Resource(session)
                .Relationships()
                .Relationship("creator")
                .Links()
                .AddSelfLink()
                .AddRelatedLink()
                .LinksEnd()
                .RelationshipEnd()
                .RelationshipsEnd()
                .Links()
                .AddSelfLink()
                .LinksEnd()
                .ResourceEnd()

                // With included resources
                .Included()
                // Convert related "to-one" CLR Person resource to JSON API resource
                // Automatically generate "to-one" resource linkage in article to related author
                .ToOne(user, "creator", user)
                .ToOneEnd()
                .IncludedEnd()



                .WriteDocument();
            return Created(currentRequestUrl.AbsoluteUri, newDocument);
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