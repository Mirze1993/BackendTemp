using Asp.Versioning;
using Domain.RoutePaths;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controller.V2;

[ApiController]
[ApiVersion("2.0")]
public class AuthController:ControllerBase
{
   
    [HttpPost(RoutePaths.Login)]
    public void LoginV2()
    {
        
    }
}