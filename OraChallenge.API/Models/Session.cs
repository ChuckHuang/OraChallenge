﻿using System;
using JsonApiFramework;

namespace OraChallenge.API.Models
{
    public class Session : IResource
    {
        public int Id { get; set; }
        public string CreatedAt { get; set; }

        public virtual User User { get; set; }
    }
}
