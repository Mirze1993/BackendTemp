using Domain.Enums;

namespace Domain.Request.User;

public class EditProfilReq
{
    public string ColumnName { get; set; } = string.Empty;
    public EditType EditType { get; set; }
    public int? UserId { get; set; }
    public int? OldId { get; set; }
    public string NewValue { get; set; } = string.Empty;
}