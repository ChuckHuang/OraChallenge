using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using JsonApiFramework.JsonApi;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using OraChallenge.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using OraChallenge.API.JsonApi;

namespace OraChallenge.API.Controllers
{
    [Produces("application/vnd.api+json")]
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

                var userid = SiteAuthorizationExtensions.DecryptUserId(Request.Headers["Authorization"]);
                var newToken = SiteAuthorizationExtensions.CreateJwtToken(userid);
                this.Response.Headers.Add("Authorization", "Bearer " + newToken);

                // Build new document.
                var document = JsonApiUtil.WriteDocumentForMessagesGetResponse(pageNumber, pageSize, currentRequestUrl, urlHelper, totalPages, messages);


                // Return 200 OK
                // Note: WebApi JsonMediaTypeFormatter serializes the JSON API document into JSON. 
                return this.Ok(document);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { title = "Something went wrong, please try again." });
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
                var token = SiteAuthorizationExtensions.GetTokenFromHeader(Request.Headers["Authorization"]);
                var userid = SiteAuthorizationExtensions.DecryptUserId(token);
                var message = JsonApiUtil.GetMessageFromMesasgesPostRequest(document);

                if (string.IsNullOrWhiteSpace(message))
                {
                    return StatusCode(422, new { title = "Field data error." });
                }

                var messageRecord = new MessageRecord()
                {
                    UserId = int.Parse(userid),
                    Message = message,
                    CreatedAt = DateTime.Now
                };
                _context.MessageRecord.Add(messageRecord);
                await _context.SaveChangesAsync();
                messageRecord.User = _context.User.FirstOrDefault(u => u.Id == messageRecord.UserId);

                var newToken = SiteAuthorizationExtensions.CreateJwtToken(userid);
                this.Response.Headers.Add("Authorization", "Bearer " + newToken);

                var currentRequestUrl = this.HttpContext.Request.GetUri();

                // Build new document.
                var newDocument = JsonApiUtil.WriteDocumentForMessagesPostResponse(currentRequestUrl, messageRecord);
                return Created(currentRequestUrl.AbsoluteUri, newDocument);
            }
            catch (Exception e)
            {
                return StatusCode(500, new {title= "Something went wrong, please try again." });
            }

        }

    }
}