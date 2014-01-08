using System;
namespace Twitter.Shared.Models
{
    public class Tweet
    {
        public string Text { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}