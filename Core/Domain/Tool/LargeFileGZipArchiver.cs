using System.IO.Compression;

namespace Domain.Tool;

public class LargeFileGZipArchiver
{
    private const int BufferSize = 1024 * 1024; // 1 MB

    public async Task ArchiveToGzipAsync(string dir, string archiveDir, string extension, long toKeepMinute,
        CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(archiveDir))
            Directory.CreateDirectory(archiveDir);

        var files = Directory.GetFiles(dir, $"*.{extension}");
        foreach (var file in files)
        {
            var lastWrite = File.GetLastWriteTime(file);
            if ((DateTime.Now - lastWrite).Minutes > toKeepMinute)
            {
                string fileName = Path.GetFileName(file);
                string gzipFile = Path.Combine(archiveDir, $"{fileName}.gz");

                await using (var sourceStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read,
                                 BufferSize, useAsync: true))
                {
                    await using (var destinationStream = new FileStream(gzipFile, FileMode.Create, FileAccess.Write,
                                     FileShare.None, BufferSize, useAsync: true))
                    {
                        await using (var gzipStream = new GZipStream(destinationStream, CompressionLevel.Optimal))
                        {
                            await sourceStream.CopyToAsync(gzipStream, BufferSize, cancellationToken);
                        }
                    }
                }

                File.Delete(file);
            }
        }
    }
}