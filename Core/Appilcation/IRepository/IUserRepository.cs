using Domain.DTO.User;
using Domain.Entities.User;
using Domain.Request.User;
using Domain.Response.User;

namespace Appilcation.IRepository;

public interface IUserRepository
{
    Task<int> Register(RegisterUserDto dto);

    Task<GetUserDto> GetUserByEmail(string email);

    Task<GetUserDto> GetByRefreshToken(string refToken, int id);
    Task<bool> SuccessLogin(string refToken, int id);
    Task<bool> FailedLogin(int id);
    Task<List<UserClaims>> GetClaims(int userId, string claimType);
    Task<int> SetClaim(SetClaimReq req);
    Task<bool> RemoveClaim(int id);
    Task<bool> EditClaim(int claimId, string value);
    Task<bool> EditProfile(EditProfilReq req);
    Task<List<SearchUserResp>> SearchUsers(string name, int userId);
    Task<int> RemoveClaimByType(SetClaimReq req);
    Task<List<RoleValueDto>> GetRoleValue();
}