using System.Text.RegularExpressions;
using Asp.Versioning;
using Domain;
using FaceArt;
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
                             @"\.(png|jpg|svg|gif|doc|docx|log|odt|pages|rtf|txt|csv|key|pps|ppt|pptx|tar|xml|json|pdf|xls|xlsx|db|sql|rar|gz|zip|webm)"))
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
    
    [HttpPost("/File/GetImgSimilarity")]
    //[Authorize(Policy = "UserToken")]
    //[ClaimRequirement("Role","FileWrite")]
    public async Task<Result<float>> GetImgSimilarity(IFormFile file1, IFormFile file2,CancellationToken cancellationToken)
    {
        if (file1 == null || file1.Length == 0
                         || !Regex.IsMatch(file1.FileName.ToLower(),
                             @"\.(png|jpg|svg|gif)"))
            return Result<float>.ErrorResult("type1 error");
        
        if (file2 == null || file2.Length == 0
                          || !Regex.IsMatch(file2.FileName.ToLower(),
                              @"\.(png|jpg|svg|gif)"))
            return Result<float>.ErrorResult("type2 error");
        var ms1 = new MemoryStream();
        var ms2 = new MemoryStream();
        
        var path = Path.Combine(environment.WebRootPath, "Image/capture-f538215d-4911-48ee-bb21-e339aecd45cb.png");
        using (var fs = new FileStream(path, FileMode.Open))
        {
            await fs.CopyToAsync(ms1, cancellationToken);
            fs.Flush();
        }
        
        //await file1.CopyToAsync(ms1);  
        await file2.CopyToAsync(ms2);
        var sim= new ImgSimilarityService().GetSimilarity(ms1, ms2);
        return Result<float>.SuccessResult( sim * 100);

    }


    [HttpPost("/File/SaveJodit")]
    // [Authorize(Policy = "UserToken")]
    // [ClaimRequirement("Role","FileWrite")]
    public async Task<Result<string>> SaveJodit(List<IFormFile> files)
    {
        if (files[0] == null || files[0].Length == 0
                             || !Regex.IsMatch(files[0].FileName.ToLower(),
                                 @"\.(png|jpg|gif|doc|docx|log|odt|pages|rtf|txt|csv|key|pps|ppt|pptx|tar|xml|json|pdf|xls|xlsx|db|sql|rar|gz|zip|webm)"))
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
