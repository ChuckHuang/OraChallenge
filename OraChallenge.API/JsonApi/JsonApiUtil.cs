using System;
using JsonApiFramework.JsonApi;
using JsonApiFramework.Server;
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

    }
}
