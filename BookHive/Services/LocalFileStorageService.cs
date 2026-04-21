namespace BookHive.Services;

public class LocalFileStorageService(IWebHostEnvironment environment) : IFileStorageService
{
    public async Task<string?> SaveFileAsync(IFormFile? file, string folderName, CancellationToken cancellationToken = default)
    {
        if (file is null || file.Length == 0)
        {
            return null;
        }

        var uploadsRoot = Path.Combine(environment.WebRootPath, "uploads", folderName);
        Directory.CreateDirectory(uploadsRoot);

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var physicalPath = Path.Combine(uploadsRoot, fileName);

        await using var stream = new FileStream(physicalPath, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);

        return $"/uploads/{folderName}/{fileName}";
    }

    public void DeleteFile(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return;
        }

        var normalizedPath = relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var physicalPath = Path.Combine(environment.WebRootPath, normalizedPath);

        if (File.Exists(physicalPath))
        {
            File.Delete(physicalPath);
        }
    }
}
