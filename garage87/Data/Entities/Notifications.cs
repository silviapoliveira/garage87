using System;

namespace garage87.Data.Entities
{
    public class Notifications : IEntity
    {
        public int Id { get; set; }


        public string UserId { get; set; }


        public int? AssignId { get; set; }


        public bool IsRead { get; set; }


        public string Message { get; set; }


        public DateTime MessageDate { get; set; }
    }
}
