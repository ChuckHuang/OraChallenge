using System;
using JsonApiFramework;
using JsonApiFramework.Http;
using JsonApiFramework.Server;
using JsonApiFramework.ServiceModel.Configuration;
using OraChallenge.API.Models;

namespace OraChallenge.API.JsonApi
{
    public class OraChallengeDocumentContext : DocumentContext
    {
        private static UrlBuilderConfiguration UrlBuilderConfiguration;

        public OraChallengeDocumentContext(string host, int? port = null)
        {
            UrlBuilderConfiguration =  new UrlBuilderConfiguration("http", host, port);
        }

        protected override void OnConfiguring(IDocumentContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseUrlBuilderConfiguration(UrlBuilderConfiguration);
        }

        protected override void OnServiceModelCreating(IServiceModelBuilder serviceModelBuilder)
        {
            var messageConfiguration = serviceModelBuilder.Resource<MessageRecord>();
            // .. Hypermedia
            messageConfiguration.Hypermedia()
                .SetApiCollectionPathSegment("messages");

            // .. ResourceIdentity
            messageConfiguration.ResourceIdentity(x => x.Id)
                .SetApiType("messages");

            // .. Attributes
            messageConfiguration.Attribute(x => x.Message)
                .SetApiPropertyName("message");

            messageConfiguration.Attribute(x => x.CreatedAt)
                .SetApiPropertyName("created_at");


            messageConfiguration.ToOneRelationship<User>(rel: "creator");

            var userConfiguration = serviceModelBuilder.Resource<User>();
            // .. Hypermedia
            userConfiguration.Hypermedia()
                .SetApiCollectionPathSegment("users");

            // .. ResourceIdentity
            userConfiguration.ResourceIdentity(x => x.Id)
                .SetApiType("users");

            // .. Attributes
            userConfiguration.Attribute(x => x.UserName)
                .SetApiPropertyName("username");


            var sessionConfiguration = serviceModelBuilder.Resource<Session>();
            // .. Hypermedia
            sessionConfiguration.Hypermedia()
                .SetApiCollectionPathSegment("sessions");

            sessionConfiguration.Attribute(x => x.CreatedAt)
                .SetApiPropertyName("created_at");

            // .. ResourceIdentity
            sessionConfiguration.ResourceIdentity(x => x.Id)
                .SetApiType("sessions");

            sessionConfiguration.ToOneRelationship<User>(rel: "creator");
        }
    }
}
