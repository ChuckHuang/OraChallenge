using System;
using System.Collections.Generic;
using JsonApiFramework;

namespace OraChallenge.API.Models
{
    public partial class MessageRecord : IResource
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual User User { get; set; }
    }
}
