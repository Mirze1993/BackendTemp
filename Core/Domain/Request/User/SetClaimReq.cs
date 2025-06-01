namespace Domain.Request.User;

public record SetClaimReq(int UserId, string ClaimType, string[] Value);