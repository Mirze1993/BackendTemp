using Asp.Versioning;
using Domain.RoutePaths;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controller.V1;

[ApiController]
[ApiVersion("1.0")]
public class AuthController:ControllerBase
{
    [HttpPost(RoutePaths.Login)]
    public void Login()
    {
    }
    
    [HttpPost(RoutePaths.Register)]
    public void Register()
    {
    }
}