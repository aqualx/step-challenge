public static class ToDTOMapping
{
    public static UserDTO ToUserDTO(this User user)
    {
        return new UserDTO
        {
            Id = user.Id,
            Name = user.Name,
            TeamId = user.TeamId
        };
    }

    public static TeamDTO ToTeamDTO(this Team team)
    {
        return new TeamDTO
        {
            Id = team.Id,
            Name = team.Name
        };
    }

    public static CounterDTO ToCounterDTO(this Counter counter)
    {
        return new CounterDTO
        {
            Id = counter.Id,
            Steps = counter.Steps,
            UpdatedAt = counter.UpdatedAt,
            UserId = counter.UserId,
        };
    }
}