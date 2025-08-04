using Appilcation.CustomerAttributes;
using Appilcation.IRepository;
using Appilcation.Tool;
using Asp.Versioning;
using Domain;
using Domain.DTO.User;
using Domain.Entities.User;
using Domain.Request;
using Domain.Request.User;
using Domain.Response;
using Domain.Response.User;
using Domain.RoutePaths;
using ExternalServices;
using ExternalServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GenTokenDto = Domain.Tool.GenTokenDto;

namespace AuthApi.Controller.V1;

[ApiController]
[ApiVersion("1.0")]
public class AuthController(IConfiguration configuration, IUserRepository repository,IAsanFinance asanFinance) : ControllerBase
{
    [HttpPost(RoutePaths.Login), CommonException, ReqRespLog]
    public async Task<Result<LoginResp>> Login([FromBody] LoginReq req)
    {
        var user = await repository.GetUserByEmail(req.Email);
        if (user == null)
            throw new Exception("User Not Found");
        var flm = (DateTime.Now - user.LastFailedLogin).TotalMinutes;

        if (user.FailedCount > 5 && flm < 10)
        {
            await repository.FailedLogin(user.Id);
            throw new Exception($"Please try {10 - flm} minute");
        }

        if (user.Password != req.Password)
        {
            await repository.FailedLogin(user.Id);
            throw new Exception("Password incorrect");
        }

        return await CreateToken(user).SuccessResult();
    }

    [HttpPost(RoutePaths.LoginByRefreshToken)]
    public async Task<Result<LoginResp>> LoginByRefreshToken(LoginByRefTokReq req)
    {
        var user = await repository.GetByRefreshToken(req.RefreshToken ?? "", req.Id);
        return await CreateToken(user).SuccessResult();
    }

    [HttpPost(RoutePaths.Register), CommonException]
    public async Task<Result<int>> Register([FromBody] RegisterUserDto req)
    {
        return await repository.Register(req).SuccessResult();
    }

    #region Porfile
    
    [HttpPost(RoutePaths.UpdateProfile)]
    [Authorize]
    public async Task<Result<bool>> UpdateProfile(EditProfilReq req)
    {
        req.UserId = GetId();
        return await repository.EditProfile( req ).SuccessResult();
    }

    [HttpGet(RoutePaths.GetProfile)]
    [Authorize]
    public async Task<Result<List<UserClaims>>> GetProfile([FromQuery]int? id)
    {
        return await repository.GetClaims( id??GetId(), "").SuccessResult();
    }


    [HttpGet(RoutePaths.GetRoleValue)]
    [Authorize]
    public async Task<Result<List<RoleValueDto>>> GetRoleValue()
        =>await repository.GetRoleValue().SuccessResult();
    
    [HttpPost(RoutePaths.SetClaim)]
    [Authorize]
    public async Task<Result<int>> SetClaim([FromBody]SetClaimReq req)
    {
        return await repository.SetClaim(req).SuccessResult();
    }

    #endregion

    #region notification
    [HttpGet(RoutePaths.GetNotif)]
    [Authorize]
    public async Task<Result<List<NotifResp>>> GetNotif()
        =>await repository.GetNotif(GetId()).SuccessResult();
 
 
    [HttpGet(RoutePaths.GetUnReadNotifCount)]
    [Authorize]
    public async  Task<Result<int>> GetUnReadNotifCount()
        =>await repository.GetUnReadNotifCount( GetId()).SuccessResult();
 
 
    [HttpPost(RoutePaths.ReadNotif)]
    [Authorize]
    public async Task<Result> ReadNotif([FromBody]IntListReq ids)
        =>await repository.ReadNotif(ids).SuccessResult();
 
 
    [HttpPost(RoutePaths.InstPosition)]
    [Authorize]
    public async Task<Result> InstPosition([FromBody]PositionReq req)
        =>await repository.InstPosition( req).SuccessResult();
 
 
    [HttpGet(RoutePaths.GetPosition)]
    [Authorize]
    public async Task<Result<List<PositionReq>>> GetPosition()
        =>await repository.GetPosition().SuccessResult();
 
 
    [HttpDelete(RoutePaths.DeletePosition)]
    [Authorize]
    public async Task<Result> DeletePosition([FromRoute]int id)
        =>await repository.DeletePosition(id).SuccessResult();
    

    #endregion
    
    [HttpGet(RoutePaths.SearchUsers)]
    [Authorize]
    public async Task<Result<List<SearchUserResp>>> SearchUsers(string name)
    {
        return await repository.SearchUsers( name, GetId()).SuccessResult();
    }
    
    [HttpGet(RoutePaths.GetUserFromAsanFinance)]
    [Authorize]
    public async Task<Result<AsanFinanceResp>> GetUserFromAsanFinance(string pin)
    {
        return await asanFinance.GetAsanFinanceAsync(pin,"000000000" );
    }

    private int GetId()=>
        int.Parse(User.Claims.First(mm => mm.Type == "Id").Value);
    
    
    

    private async Task<LoginResp> CreateToken(GetUserDto user)
    {
        var token = TokenTool.Generate(configuration, new GenTokenDto()
        {
            Email = user.Email,
            Id = user.Id,
            //Roles = roles.ToList()
        });
        var refToken = TokenTool.CreateRefreshToken();

        // if (!await _repository.SuccesLogin(refToken, user.Id))
        //     throw new Exception("Login Fallied");
        return new LoginResp() { Token = token, RefreshToken = refToken };
    }
}