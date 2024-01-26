namespace Libgit2Bindings;

public enum GitOidType
{
  Sha1 = 1,
  // Sha256 = 2,
}

public sealed record GitOid(byte[] Id)
{
  public const int Size = 20;
  public const int HexSize = Size * 2;

  public byte[] Id { get; } = Id.Length == Size ? Id : throw new ArgumentException($"Length of {nameof(Id)} must be {Size}.");

  public string Sha => Convert.ToHexString(Id).ToLowerInvariant();

  public bool IsZero => Id.All(b => b == 0);

  public bool Equals(GitOid? other)
  {
    if (other is null)
    {
      return false;
    }

    if (ReferenceEquals(this, other))
    {
      return true;
    }

    return Id.SequenceEqual(other.Id);
  }

  public override int GetHashCode()
  {
    return new System.Numerics.BigInteger(Id).GetHashCode();
  }
}
