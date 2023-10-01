namespace Libgit2Bindings.Test;

public sealed class GitSignatureTest
{
  [Fact]
  public void CanCreateSignature()
  {
    using var libgit2 = new Libgit2();
    var time = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.FromMinutes(60));
    using var signature = libgit2.CreateGitSignature("John Doe 💕", "john@doe.de", time);
    Assert.NotNull(signature);
    Assert.Equal("John Doe 💕", signature.Name);
    Assert.Equal("john@doe.de", signature.Email);
    Assert.Equal(time, signature.When);
  }

  [Fact]
  public void CanCreateSignatureFromString()
  {
    using var libgit2 = new Libgit2();
    var time = new DateTimeOffset(2021, 1, 1, 5, 25, 10, TimeSpan.FromMinutes(60));
    using var signature = libgit2.CreateGitSignature(
      $"John Doe 💕 <john@doe.de> {time.ToUnixTimeSeconds()} 0100");
    Assert.NotNull(signature);
    Assert.Equal("John Doe 💕", signature.Name);
    Assert.Equal("john@doe.de", signature.Email);
    Assert.Equal(time, signature.When);
  }
}
