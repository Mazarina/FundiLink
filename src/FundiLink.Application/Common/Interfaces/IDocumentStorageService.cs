namespace FundiLink.Application.Common.Interfaces;

public interface IDocumentStorageService
{
    Task StoreAsync(Stream stream, string contentType, string storageKey, CancellationToken ct);
    Task<(Stream Stream, string ContentType)> GetAsync(string storageKey, CancellationToken ct);
    Task DeleteAsync(string storageKey, CancellationToken ct);
}
