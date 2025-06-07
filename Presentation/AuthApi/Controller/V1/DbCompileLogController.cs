using Appilcation.CustomerAttributes;
using Appilcation.IRepository;
using Asp.Versioning;
using Domain;
using Domain.Entities.DbCompileLog;
using Domain.Request.User;
using Domain.Response;
using Domain.RoutePaths;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controller.V1;

[ApiController]
[ApiVersion("1.0")]
public class DbCompileLogController(IConfiguration configuration,IDbCompileLog repository):ControllerBase
{
    [HttpGet(RoutePaths.DbCompLogGetDistinctList), CommonException, ReqRespLog]
    public async Task<Result<IEnumerable<string>>> GetDistinctList()
    {
        return await repository.GetDistinctList().SuccessResult();
    }
    
    [HttpGet(RoutePaths.DbCompLogGetByName), CommonException, ReqRespLog]
    public async Task<Result<IEnumerable<CompileLog>>> GetByName(string name)
    {
        return await repository.GetByName(name).SuccessResult();
    }
    
    [HttpGet(RoutePaths.DbCompLogGetById), CommonException, ReqRespLog]
    public async Task<Result<CompileLog?>> GetById(int id)
    {
        return await repository.GetById(id).SuccessResult();
    }
    
}