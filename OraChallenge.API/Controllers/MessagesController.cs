using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonApiFramework.JsonApi;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using OraChallenge.API.Models;
using JsonApiFramework.Server;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using OraChallenge.API.JsonApi;

namespace OraChallenge.API.Controllers
{
    [Produces("application/json")]
    [Route("api/Messages", Name = "MessagesList")]
    
    public class MessagesController : Controller
    {
        private readonly OraChallengeDBContext _context;

        public MessagesController(OraChallengeDBContext context)
        {

            _context = context;
        }

        // GET: api/Messages
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMessages(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var totalCount = _context.MessageRecord.Count();

                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);


                var messages = _context.MessageRecord.Include(m => m.User).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

                var urlHelper = new UrlHelper(this.ControllerContext);
                var currentRequestUrl = this.HttpContext.Request.GetUri();

                // Build new document.
                var document = new OraChallengeDocumentContext(currentRequestUrl.Host, currentRequestUrl.Port).NewDocument(this.Request.GetUri())

                    // Document links
                    .Links()
                    .AddLink(Keywords.Self, new Link(urlHelper.Link("MessagesList",
                new
                {
                    pageNumber = pageNumber,
                    pageSize = pageSize
                })))
                    .AddLink(Keywords.First, new Link(urlHelper.Link("MessagesList",
                        new
                        {
                            pageNumber = 1,
                            pageSize = pageSize
                        })))
                    .AddLink(Keywords.Prev, pageNumber > 1 ? new Link(urlHelper.Link("MessagesList",
                        new
                        {
                            pageNumber = pageNumber - 1,
                            pageSize = pageSize
                        })) : null)
                    .AddLink(Keywords.Next, pageNumber < totalPages ? new Link(urlHelper.Link("MessagesList",
                        new
                        {
                            pageNumber = 1,
                            pageSize = pageSize
                        })) : null)
                    .AddLink(Keywords.Last, new Link(urlHelper.Link("MessagesList",
                        new
                        {
                            pageNumber = totalPages,
                            pageSize = pageSize
                        })))
                    .LinksEnd()

                    // Resource
                    .ResourceCollection(messages)
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
                    .ResourceCollectionEnd()
                    .WriteDocument();

                // Return 200 OK
                // Note: WebApi JsonMediaTypeFormatter serializes the JSON API document into JSON. 
                return this.Ok(document);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }

        // POST: api/Messages
        [HttpPost]
        public async Task<IActionResult> PostMessage([FromBody] ResourceDocument document)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                string authorization = Request.Headers["Authorization"];

                var tokenStr = authorization.Substring("Bearer ".Length).Trim();
                var userid = SiteAuthorizationExtensions.ReadUserIdFromJwtToken(tokenStr);
                ApiProperty des = null;
                document.Data.Attributes.TryGetApiProperty("message", out des);
                var str = des.ToString();
                var sects = str.Split(':');
                var message = new MessageRecord()
                {
                    UserId = int.Parse(userid),
                    Message = sects[1],
                    CreatedAt = DateTime.Now
                };
                _context.MessageRecord.Add(message);
                await _context.SaveChangesAsync();
                message = _context.MessageRecord.Include(m=>m.User).FirstOrDefault(m=>m.Id == message.Id);
                var urlHelper = new UrlHelper(this.ControllerContext);
                var currentRequestUrl = this.HttpContext.Request.GetUri();

                // Build new document.
                var newDocument = new OraChallengeDocumentContext(currentRequestUrl.Host, currentRequestUrl.Port).NewDocument(this.Request.GetUri())

                   
                    // Resource
                    .Resource(message)
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
                    .ToOne(message, "creator", message.User)                  
                    .ToOneEnd()
                    .IncludedEnd()

                    

                    .WriteDocument();
                return Created(currentRequestUrl.AbsoluteUri, newDocument);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }
    }
}