using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class UserRequestDTO
{
    [Required]
    [Description("The name of the user")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Description("Unique identifier for the team that the user belongs to. The team must exist in the system.")]
    public string TeamId { get; set; }

    [Description("User password")]
    [Required]
    public string Password { get; set; }
}