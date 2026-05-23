using System.ComponentModel.DataAnnotations;

namespace RemoteTechTeamApi.DTOs
{
    public class CreateKudosDto
    {
        [Required]
        public int ReceiverId { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "Kudos messages must be between 3 and 250 characters.")]
        public string Message { get; set; } = string.Empty;
    }
}