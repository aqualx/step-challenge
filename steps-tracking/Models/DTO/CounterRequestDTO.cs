
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class CounterRequestDTO
{
    [Required]
    [Description("User steps walked")]
    public uint Steps { get; set; }

    [Description("Specify backdated date to send steps for previous periods. No need to send this for current date.")]
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
}