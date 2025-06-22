using Appilcation.CustomerAttributes;
using Appilcation.IRepository;
using Appilcation.Tool;
using Asp.Versioning;
using Domain;
using Domain.DTO.User;
using Domain.Entities.User;
using Domain.Request.User;
using Domain.Response;
using Domain.RoutePaths;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GenTokenDto = Domain.Tool.GenTokenDto;

namespace AuthApi.Controller.V1;

[ApiController]
[ApiVersion("1.0")]
public class AuthController(IConfiguration configuration, IUserRepository repository) : ControllerBase
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

        return await CreateToken(user).SuccessResult<LoginResp>();
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
        return await repository.Register(req).SuccessResult<int>();
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

    #endregion

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