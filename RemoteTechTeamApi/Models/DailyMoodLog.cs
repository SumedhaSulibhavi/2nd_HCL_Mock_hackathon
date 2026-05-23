using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HypeHealthAPI.Models
{
    public class DailyMoodLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(1, 10)]
        public int Score { get; set; }

        [Required]
        public DateTime LogDate { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}