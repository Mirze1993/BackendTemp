namespace Domain.Request.User;

public record NotifResp(int Id, string Title, string Body,
    bool IsRead, DateTime CreateDate, string Icon, int Type, int UserId);
  