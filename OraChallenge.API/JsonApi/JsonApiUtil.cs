using System;
using System.Collections.Generic;
using JsonApiFramework.JsonApi;
using JsonApiFramework.Server;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OraChallenge.API.Models;

namespace OraChallenge.API.JsonApi
{
    public static class JsonApiUtil
    {
        public static Document WriteDocumentForSessionsPostResponse(Uri currentRequestUrl, Session session)
        {
            var meta = new Meta();
            string json = @"{}";
            JObject o = JObject.Parse(json);
            meta.SetData(o);

            var response = new OraChallengeDocumentContext(currentRequestUrl.Host, currentRequestUrl.Port).NewDocument(currentRequestUrl)
                
                .SetMeta(meta)
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
                .ToOne(session, "creator", session.User)
                .Links()
                .AddSelfLink()
                .LinksEnd()
                .ToOneEnd()
                .IncludedEnd()

                .WriteDocument();
            return response;
        }

        public static Document WriteDocumentForMessagesGetResponse(int pageNumber, int pageSize, Uri currentRequestUrl, UrlHelper urlHelper, int totalPages, List<MessageRecord> messages)
        {
            var meta = new Meta();
            string json = @"{count: " + messages.Count + "}";        
            JObject o = JObject.Parse(json);
            meta.SetData(o);

            return new OraChallengeDocumentContext(currentRequestUrl.Host, currentRequestUrl.Port).NewDocument(currentRequestUrl)
                .SetMeta(meta)
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
        }

        public static Document WriteDocumentForMessagesPostResponse(Uri currentRequestUrl, MessageRecord messageRecord)
        {
            var meta = new Meta();
            string json = @"{}";
            JObject o = JObject.Parse(json);
            meta.SetData(o);

            return new OraChallengeDocumentContext(currentRequestUrl.Host, currentRequestUrl.Port).NewDocument(currentRequestUrl)

                .SetMeta(meta)
                // Resource
                .Resource(messageRecord)
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
                .ToOne(messageRecord, "creator", messageRecord.User)
                .Links()
                .AddSelfLink()
                .LinksEnd()
                .ToOneEnd()
                .IncludedEnd()

                .WriteDocument();
        }

        public static string GetMessageFromMesasgesPostRequest(ResourceDocument document)
        {
            ApiProperty des = null;
            document.Data.Attributes.TryGetApiProperty("message", out des);
            var str = des.ToString();
            var sects = str.Split(':');
            return sects[1];
        }
    }
}
