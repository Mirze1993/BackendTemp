using System.Text.RegularExpressions;
using Asp.Versioning;
using Domain;
using FileApi.CustomerAttribute;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileApi.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]

public class FileController : ControllerBase
{
    private readonly ILogger<FileController> _logger;
    private readonly IWebHostEnvironment environment;

    public FileController(ILogger<FileController> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        this.environment = environment;
    }

    [HttpPost("/File/Save")]
    //[Authorize(Policy = "UserToken")]
    //[ClaimRequirement("Role","FileWrite")]
    public async Task<Result<string>> Save(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0
                         || !Regex.IsMatch(file.FileName.ToLower(),
                             @"\.(png|jpg|svg|gif|doc|docx|log|odt|pages|rtf|txt|csv|key|pps|ppt|pptx|tar|xml|json|pdf|xls|xlsx|db|sql|rar|gz|zip)"))
            return Result<string>.ErrorResult("type error");


        var name = Path.Combine("Image",
            Path.GetFileNameWithoutExtension(file.FileName)
            + "-"
            + Guid.NewGuid() + Path.GetExtension(file.FileName));

        var path = Path.Combine(environment.WebRootPath, name);


        using (var fs = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(fs, cancellationToken);
            fs.Flush();
        }

        return Result<string>.SuccessResult("/StaticFiles/" + name);
        ;
    }


    [HttpPost("/File/SaveJodit")]
    // [Authorize(Policy = "UserToken")]
    // [ClaimRequirement("Role","FileWrite")]
    public async Task<Result<string>> SaveJodit(List<IFormFile> files)
    {
        if (files[0] == null || files[0].Length == 0
                             || !Regex.IsMatch(files[0].FileName.ToLower(),
                                 @"\.(png|jpg|gif|doc|docx|log|odt|pages|rtf|txt|csv|key|pps|ppt|pptx|tar|xml|json|pdf|xls|xlsx|db|sql|rar|gz|zip)"))
            return Result<string>.ErrorResult("type error");

        var name = Path.Combine("Image",
            Path.GetFileNameWithoutExtension(files[0].FileName)
            + "-"
            + Guid.NewGuid() + Path.GetExtension(files[0].FileName));

        var path = Path.Combine(environment.WebRootPath, name);

        using (var fs = new FileStream(path, FileMode.Create))
        {
            await files[0].CopyToAsync(fs);
            fs.Flush();
        }

        return Result<string>.SuccessResult("/StaticFiles/" + name);
    }


    [HttpDelete("/File/Delete")]
    //[Authorize(Policy = "FileRead")]
    public Result Delete(string photoUrl)
    {
        if (string.IsNullOrEmpty(photoUrl))
            return Result.ErrorResult("photoUrl is empity");
        var path = Path.Combine(environment.WebRootPath, photoUrl);
        if (!System.IO.File.Exists(path))
            return Result.ErrorResult("file not fount");
        System.IO.File.Delete(path);
        return Result.SuccessResult();
    }

    [HttpPut("/File/Update")]
    [Authorize(Policy = "FileRead")]
    public async Task<Result> Update(IFormFile file, string photoUrl, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(photoUrl))
            return Result.ErrorResult("photoUrl is empity");
        var path = Path.Combine(environment.WebRootPath, photoUrl);
        if (!System.IO.File.Exists(path) || file == null)
            return Result.ErrorResult("file not fount");


        using (var fs = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(fs, cancellationToken);
            fs.Flush();
        }

        return Result.SuccessResult();
    }
}

public class UploadResponse
{
    public string ImageUrl { get; set; } = string.Empty;
}

public class UploadReq
{
    public string? path { get; set; }
    public string? source { get; set; }
    public List<IFormFile> files { get; set; }
}
