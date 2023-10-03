namespace Libgit2Bindings;

public record GitOid(byte[] Id)
{
  public const int Size = 20;

  public byte[] Id { get; } = Id.Length == Size ? Id : throw new ArgumentException($"Length of {nameof(Id)} must be {Size}.");

  public string Sha => Convert.ToHexString(Id).ToLowerInvariant();
}
