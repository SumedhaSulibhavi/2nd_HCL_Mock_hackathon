using System.ComponentModel.DataAnnotations;

namespace RemoteTechTeamApi.DTOs
{
    public class MoodSubmissionDto
    {
        [Required]
        [Range(1, 10, ErrorMessage = "Energy level slider score must be between 1 and 10.")]
        public int Score { get; set; }
    }
}