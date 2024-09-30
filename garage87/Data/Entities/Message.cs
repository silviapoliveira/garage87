using System;

namespace garage87.Data.Entities
{
    public class Message : IEntity
    {
        public int Id { get; set; }


        public string MessageDetail { get; set; }


        public DateTime MessageDate { get; set; }


        public string ReplyMessage { get; set; }


        public DateTime ReplyDate { get; set; }


        public string RepliedBy { get; set; }
    }
}
