using Appilcation.CustomerAttributes;
using Appilcation.IRepository;
using Appilcation.Tool;
using Asp.Versioning;
using Domain;
using Domain.DTO.User;
using Domain.Request.User;
using Domain.Response;
using Domain.RoutePaths;
using Microsoft.AspNetCore.Mvc;
using GenTokenDto = Domain.Tool.GenTokenDto;

namespace AuthApi.Controller.V1;

[ApiController]
[ApiVersion("1.0")]
public class AuthController (IConfiguration configuration,IUserRepository repository):ControllerBase
{
    [HttpPost(RoutePaths.Login),CommonException,ReqRespLog]
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
    
    [HttpPost(RoutePaths.Register),CommonException]
    public async Task<Result<int>> Register([FromBody] RegisterUserDto req)
    {
       return await repository.Register(req).SuccessResult<int>();
    }
    
    private async Task<LoginResp> CreateToken(GetUserDto user)
    {
        
        var token = TokenTool.Generate(configuration, new GenTokenDto()
        {
            Email = user.Email,
            Id = user.Id,
            Name = "name",
            //Roles = roles.ToList()
        });
        var refToken = TokenTool.CreateRefreshToken();

        // if (!await _repository.SuccesLogin(refToken, user.Id))
        //     throw new Exception("Login Fallied");
        return new LoginResp(  ){Token = token, RefreshToken = refToken};
    }
}