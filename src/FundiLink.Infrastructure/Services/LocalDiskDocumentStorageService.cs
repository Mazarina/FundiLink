using FundiLink.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FundiLink.Infrastructure.Services;

public class LocalDiskDocumentStorageService : IDocumentStorageService
{
    private readonly string _rootPath;

    public LocalDiskDocumentStorageService(IConfiguration config)
    {
        _rootPath = config["DocumentStorage:RootPath"] ?? "document-store";
    }

    public async Task StoreAsync(Stream stream, string contentType, string storageKey, CancellationToken ct)
    {
        var safePath = SanitiseKey(storageKey);
        var fullPath = Path.Combine(_rootPath, safePath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        using var fs = File.Create(fullPath);
        await stream.CopyToAsync(fs, ct);
    }

    public Task<(Stream Stream, string ContentType)> GetAsync(string storageKey, CancellationToken ct)
    {
        var safePath = SanitiseKey(storageKey);
        var fullPath = Path.Combine(_rootPath, safePath);
        Stream fs = File.OpenRead(fullPath);
        return Task.FromResult((fs, "application/octet-stream"));
    }

    public Task DeleteAsync(string storageKey, CancellationToken ct)
    {
        var safePath = SanitiseKey(storageKey);
        var fullPath = Path.Combine(_rootPath, safePath);
        if (File.Exists(fullPath)) File.Delete(fullPath);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Prevents path traversal by stripping any ".." or "." segments from the storage key.
    /// </summary>
    private static string SanitiseKey(string key)
    {
        var parts = key.Replace('\\', '/').Split('/');
        var safe = parts.Where(p => p != ".." && p != "." && !string.IsNullOrEmpty(p)).ToArray();
        return Path.Combine(safe);
    }
}
