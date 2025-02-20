using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("teams")]
public class Team
{
    [Key]
    [Column("id")]
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    [Required]
    [Column("name")]
    public string Name { get; set; }

    public List<User> Users { get; set; }
}