namespace BookHive.Services;

public interface IFileStorageService
{
    Task<string?> SaveFileAsync(IFormFile? file, string folderName, CancellationToken cancellationToken = default);

    void DeleteFile(string? relativePath);
}
