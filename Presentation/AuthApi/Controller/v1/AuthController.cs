using Asp.Versioning;
using Domain.RoutePaths;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controller.v1;

[Controller]
[ApiVersion(1)]
[ApiVersion(2)]
public class AuthController:ControllerBase
{
    
    [MapToApiVersion(1)]
    [HttpPost(RoutePaths.Login)]
    public void LoginV1()
    {
        
    }
    
    [MapToApiVersion(2)]
    [HttpPost(RoutePaths.Login)]
    public void LoginV2()
    {
        
    }
}