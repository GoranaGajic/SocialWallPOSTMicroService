using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WallPostMicroService.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string Text { get; set; }
        public decimal Rating { get; set; }
        public int Views { get; set; }
        public String Attachment { get; set; }
        public string Location { get; set; }
        public bool Active { get; set; }
        public Guid UserId { get; set; }
        public HashSet<Guid> LikedByUsers { get; set; }
    }
}