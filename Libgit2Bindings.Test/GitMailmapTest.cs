using System.Text;

namespace Libgit2Bindings.Test;

public class GitMailmapTest
{
  [Fact]
  public void MailmapAddEntry()
  {
    using var libgit2 = new Libgit2();
    using var mailmap = libgit2.NewGitMailmap();

    mailmap.AddEntry("realName", "realEmail", "replaceName", "replaceEmail");

    var nameAndEmail = mailmap.Resolve("replaceName", "replaceEmail");

    Assert.Equal("realName", nameAndEmail.Name);
    Assert.Equal("realEmail", nameAndEmail.Email);
  }

  [Fact]
  public void CanResolveSignature()
  {
    using var libgit2 = new Libgit2();
    using var mailmap = libgit2.NewGitMailmap();

    mailmap.AddEntry("realName", "realEmail", "replaceName", "replaceEmail");

    using var signature = libgit2.CreateGitSignature("replaceName", "replaceEmail", DateTimeOffset.Now);

    var resolvedSignature = mailmap.ResolveSignature(signature);

    Assert.Equal("realName", resolvedSignature.Name);
    Assert.Equal("realEmail", resolvedSignature.Email);
  }

  [Fact]
  public void CanCreateMailmapFromBuffer()
  {
    const string buffer = @"
      Proper Name <proper@email.xx> Commit Name <commit@email.xx>";

    using var libgit2 = new Libgit2();
    using var mailmap = libgit2.NewGitMailmapFromBuffer(Encoding.UTF8.GetBytes(buffer));

    var nameAndEmail = mailmap.Resolve("Commit Name", "commit@email.xx");

    Assert.Equal("Proper Name", nameAndEmail.Name);
    Assert.Equal("proper@email.xx", nameAndEmail.Email);
  }
}
