namespace Domain.Request.User;

public struct PositionReq
{
    public int Id { get; init; }
    public int? ParentId { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
}