public class TeamDTO
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string Name { get; set; }
    public List<UserDTO> Users { get; set; }
}