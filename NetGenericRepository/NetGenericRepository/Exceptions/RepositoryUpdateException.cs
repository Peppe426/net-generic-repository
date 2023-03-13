namespace NetGenericRepository.Exceptions;

/// <summary>
/// Upate exception, incuding Rowversion
/// </summary>
public class RepositoryUpdateException : DbUpdateConcurrencyException
{
    public RepositoryUpdateException(string message, string rowVersion, Exception? innerException) : base(message, innerException)
    {
        Rowversion = rowVersion;
    }

    public string Rowversion { get; set; } = string.Empty;
}