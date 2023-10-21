namespace Libgit2Bindings.Mappers;

internal static class GitCredentialTypeMapper
{
  public static libgit2.GitCredentialT ToNative(this GitCredentialType managedType)
  {
    var nativeType = new libgit2.GitCredentialT();

    if ((managedType & GitCredentialType.UserPassPlaintext) != 0)
      nativeType |= libgit2.GitCredentialT.GIT_CREDENTIAL_USERPASS_PLAINTEXT;
    if ((managedType & GitCredentialType.SshKey) != 0)
      nativeType |= libgit2.GitCredentialT.GIT_CREDENTIAL_SSH_KEY;
    if ((managedType & GitCredentialType.SshCustom) != 0)
      nativeType |= libgit2.GitCredentialT.GIT_CREDENTIAL_SSH_CUSTOM;
    if ((managedType & GitCredentialType.Default) != 0)
      nativeType |= libgit2.GitCredentialT.GIT_CREDENTIAL_DEFAULT;
    if ((managedType & GitCredentialType.SshInteractive) != 0)
      nativeType |= libgit2.GitCredentialT.GIT_CREDENTIAL_SSH_INTERACTIVE;
    if ((managedType & GitCredentialType.Username) != 0)
      nativeType |= libgit2.GitCredentialT.GIT_CREDENTIAL_USERNAME;
    if ((managedType & GitCredentialType.SshMemory) != 0)
      nativeType |= libgit2.GitCredentialT.GIT_CREDENTIAL_SSH_MEMORY;

    return nativeType;
  }

  public static GitCredentialType ToManaged(this libgit2.GitCredentialT nativeType)
  {
    var managedType = new GitCredentialType();

    if ((nativeType & libgit2.GitCredentialT.GIT_CREDENTIAL_USERPASS_PLAINTEXT) != 0)
      managedType |= GitCredentialType.UserPassPlaintext;
    if ((nativeType & libgit2.GitCredentialT.GIT_CREDENTIAL_SSH_KEY) != 0)
      managedType |= GitCredentialType.SshKey;
    if ((nativeType & libgit2.GitCredentialT.GIT_CREDENTIAL_SSH_CUSTOM) != 0)
      managedType |= GitCredentialType.SshCustom;
    if ((nativeType & libgit2.GitCredentialT.GIT_CREDENTIAL_DEFAULT) != 0)
      managedType |= GitCredentialType.Default;
    if ((nativeType & libgit2.GitCredentialT.GIT_CREDENTIAL_SSH_INTERACTIVE) != 0)
      managedType |= GitCredentialType.SshInteractive;
    if ((nativeType & libgit2.GitCredentialT.GIT_CREDENTIAL_USERNAME) != 0)
      managedType |= GitCredentialType.Username;
    if ((nativeType & libgit2.GitCredentialT.GIT_CREDENTIAL_SSH_MEMORY) != 0)
      managedType |= GitCredentialType.SshMemory;

    return managedType;
  }
}
