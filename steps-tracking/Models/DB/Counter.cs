using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("counters")]
public class Counter
{
    [Key]
    [Column("id")]
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    [Column("Steps")]
    public uint Steps { get; set; }

    [Column("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }

    [Column("user_id")]
    public string UserId { get; set; }

    public User User { get; set; }
}