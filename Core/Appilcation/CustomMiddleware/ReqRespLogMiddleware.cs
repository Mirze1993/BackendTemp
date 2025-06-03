using System.Text;
using Appilcation.CustomerAttributes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;

namespace Appilcation.CustomMiddleware;

public class ReqRespLogMiddleware(ILogger<ReqRespLogMiddleware> logger) : IMiddleware
{
    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager = new();

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var d = context.GetEndpoint()?.Metadata.GetMetadata<ReqRespLogAttribute>();

        if (d is null)
        {
            await next(context);
            return;
        }

        if (d.ReqResp is ReqRespLogEnum.Request or ReqRespLogEnum.ReqResp)
            await LoginRequest(context, d.MaxReqSize);

        if (d.ReqResp is ReqRespLogEnum.Response or ReqRespLogEnum.ReqResp)
        {
            var originalBodyStream = context.Response.Body;
            await using var responseBody = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBody;
            await next.Invoke(context);
            await LogResponse(context, responseBody, originalBodyStream, d.MaxRespSize);
        }
        else
            await next.Invoke(context);
    }

    private async Task LoginRequest(HttpContext context, int maxSize)
    {
        context.Request.EnableBuffering();
        try
        {
            await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            await context.Request.Body.CopyToAsync(requestStream);
            var token = context.Request.Headers.FirstOrDefault(mm => mm.Key == "token");

            var req = new
            {
                Path = context.Request.Path.Value,
                Sid = token.Value.ToString(),
                QueryString = context.Request.QueryString.ToString(),
                Body = await ReadString(requestStream, maxSize)
            };
            logger.LogInformation("Req body: {Body}", req);
        }
        catch
        {
            // ignored
        }

        context.Request.Body.Position = 0;

    }


    private async Task LogResponse(HttpContext context, MemoryStream responseBody, Stream originalResponseBody,
        int maxSize)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var text = await ReadString(context.Response.Body, maxSize);
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        try
        {
            var req = new
            {
                Path = context.Request.Path.Value,
                Body = text
            };
            logger.LogInformation("Resp body: {Body}", req);
        }
        catch
        {
            // ignored
        }

        await responseBody.CopyToAsync(originalResponseBody);
    }

    async Task<string> ReadString(Stream stream, int maxSize = 4096)
    {
        stream.Seek(0, SeekOrigin.Begin);
        using var ms = new MemoryStream();
        var buffer = new byte[4096];
        int readCount;
        while ((readCount = await stream.ReadAsync(buffer)) > 0)
        {
            if (ms.Length + readCount > maxSize)
            {
                int remainingBytes = maxSize - (int)ms.Length;
                await ms.WriteAsync(buffer, 0, remainingBytes);
                break;
            }

            await ms.WriteAsync(buffer, 0, readCount);
        }

        return Encoding.UTF8.GetString(ms.ToArray());
    }
}