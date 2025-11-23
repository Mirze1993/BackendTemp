using Domain.Tool;
using Microsoft.Extensions.Logging;
using Quartz;

namespace QuartzScheduler.CustomJobs;

public class FileToArchiveJob(ILogger<FileToArchiveJob> logger) : IJob
{
    public static readonly JobKey Key = new JobKey("FileToArchiveJob");
    
    public static readonly string DirKey= "dirPath";
    public static readonly string ArchiveDirKey= "archiveDirPath";
    public static readonly string ExtensionKey= "extension";
    public static readonly string ToKeepMinuteKey= "toKeepMinute";
    
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("FileToArchiveJob started");
        JobDataMap dataMap = context.JobDetail.JobDataMap;
        
        string  logDir = dataMap.GetString(DirKey)??"";
        string archiveDir = dataMap.GetString(ArchiveDirKey)??"";
        string extension = dataMap.GetString(ExtensionKey)??"";
        long toKeepMinute = dataMap.GetLongValue(ToKeepMinuteKey);
        if(string.IsNullOrEmpty(logDir)||string.IsNullOrEmpty(archiveDir)||string.IsNullOrEmpty(extension))
            return;
       
        var archiver = new LargeFileGZipArchiver();
        await archiver.ArchiveToGzipAsync(logDir,archiveDir,extension,toKeepMinute);
        logger.LogInformation("FileToArchiveJob ended");
    }
}