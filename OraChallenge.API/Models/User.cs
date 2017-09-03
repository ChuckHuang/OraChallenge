using System;
using System.Collections.Generic;
using JsonApiFramework;

namespace OraChallenge.API.Models
{
    public partial class User :IResource
    {
        public User()
        {
            MessageRecord = new HashSet<MessageRecord>();
        }

        public int Id { get; set; }
        public string UserName { get; set; }

        public virtual ICollection<MessageRecord> MessageRecord { get; set; }
    }
}
