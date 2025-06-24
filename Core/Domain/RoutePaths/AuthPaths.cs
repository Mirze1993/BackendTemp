namespace Domain.RoutePaths;

public static partial class RoutePaths
{
    public const string Register = "/auth/register";
    public const string Login = "/auth/login";
    public const string LoginByRefreshToken = "/auth/loginByRefreshToken";
    
    public const string UpdateProfile = "/user/UpdateProfile";
    public const string GetProfile = "/user/GetProfile";
    public const string SearchUsers = "/user/SearchUsers";
    public const string GetRoleValue = "/user/GetRoleValue";
    public const string RemoveClaimByType = "/user/RemoveClaimByType";
    public const string SetClaim = "/user/SetClaim";
    
    public const string GetNotif = "/user/GetNotif";
    public const string GetUnReadNotifCount = "/user/GetUnReadNotifCount";
    public const string ReadNotif = "/user/ReadNotif";
    public const string InstPosition = "/user/InstPosition";
    public const string GetPosition = "/user/GetPosition";
    public const string DeletePosition = "/user/DeletePosition/{id}";
}