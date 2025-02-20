using System.ComponentModel;

public class CounterDTO
{
    [Description("Counter ID")]
    public string Id { get; set; }
    [Description("User ID")]
    public string UserId { get; set; }

    [Description("User steps walked")]
    public uint Steps { get; set; }

    [Description("Counter timestamp")]
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
}