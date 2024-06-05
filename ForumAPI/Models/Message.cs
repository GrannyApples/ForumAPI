using ForumAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ForumAPI.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public DateTime SendDate { get; set; }
        public bool IsRead { get; set; }
    }
}
