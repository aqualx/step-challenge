using System.ComponentModel;

public class UserDTO
{
    [Description("Unique identifier for the user.")]
    public string Id { get; set; }

    [Description("The name of the user")]
    public string Name { get; set; } = string.Empty;

    [Description("Unique identifier for the team that the user belongs to. The team must exist in the system.")]
    public string? TeamId { get; set; }
}