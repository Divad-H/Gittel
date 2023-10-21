namespace Libgit2Bindings;

[Flags]
public enum GitCredentialType
{
  UserPassPlaintext = 1 << 0,
  SshKey = 1 << 1,
  SshCustom = 1 << 2,
  Default = 1 << 3,
  SshInteractive = 1 << 4,
  Username = 1 << 5,
  SshMemory = 1 << 6,
}

public interface IGitCredential : IDisposable
{
}
