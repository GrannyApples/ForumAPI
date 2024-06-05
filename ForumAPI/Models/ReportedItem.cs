using System.ComponentModel.DataAnnotations;

namespace ForumAPI.Models
{
    public class ReportedItem
    {
        [Key]
        public int Id { get; set; }
        public int? PostId { get; set; }
        public int? CommentId { get; set; }
        public string? Reason { get; set; }
        public DateTime ReportDate { get; set; }
        public bool IsReviewed { get; set; } = false;
    }
}
