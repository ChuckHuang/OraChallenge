using System;
using System.Collections.Generic;
using JsonApiFramework.JsonApi;
using JsonApiFramework.Server;
using Microsoft.AspNetCore.Mvc.Routing;
using OraChallenge.API.Models;

namespace OraChallenge.API.JsonApi
{
    public static class JsonApiUtil
    {
        public static Document WriteDocumentForSessionsPostResponse(Uri currentRequestUrl, Session session)
        {
            return new OraChallengeDocumentContext(currentRequestUrl.Host, currentRequestUrl.Port).NewDocument(currentRequestUrl)


                // Resource
                .Resource(session)
                .Relationships()
                .Relationship("creator")
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
                .ToOne(session.User, "creator", session.User)
                .Links()
                .AddSelfLink()
                .LinksEnd()
                .ToOneEnd()
                .IncludedEnd()

                .WriteDocument();
        }

        public static Document WriteDocumentForMessagesGetResponse(int pageNumber, int pageSize, Uri currentRequestUrl, UrlHelper urlHelper, int totalPages, List<MessageRecord> messages)
        {
            return new OraChallengeDocumentContext(currentRequestUrl.Host, currentRequestUrl.Port).NewDocument(currentRequestUrl)

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
            return new OraChallengeDocumentContext(currentRequestUrl.Host, currentRequestUrl.Port).NewDocument(currentRequestUrl)

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
