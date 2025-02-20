using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("users")]
public class User
{
    [Key]
    [Column("id")]
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    [Required]
    [Column("name")]
    public string Name { get; set; }

    [Column("team_id")]
    public string? TeamId { get; set; }

    [Column("role")]
    public Role UserRole { get; set; } = Role.Editor;

    [Column("pwd")]
    public string Password { get; set; } // TODO: store HASH

    public Team Team { get; set; }
    public List<Counter> Counters { get; set; }
}

public enum Role
{
    Admin = 0,
    Editor = 1,
    Viewer = 2
}