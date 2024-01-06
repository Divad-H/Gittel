using System.Text;

namespace Libgit2Bindings.Test;

public sealed class MessageTest
{
  [Fact]
  public void CanParseMessage()
  {
    const string message = "This is a commit message\n"
      + "\n"
      + "This is the body of the commit message\n"
      + "\n"
      + "Signed-off-by: John Doe <john@doe.de>\n"
      + "Signed-off-by: Doe John <doe@john.de>";

    using var libgit2 = new Libgit2();
    var trailers = libgit2.ParseGitMessageTrailers(Encoding.UTF8.GetBytes(message));

    Assert.Equal(2, trailers.Count);
    Assert.Equal("Signed-off-by", Encoding.UTF8.GetString(trailers[0].Key));
    Assert.Equal("John Doe <john@doe.de>", Encoding.UTF8.GetString(trailers[0].Value));
    Assert.Equal("Signed-off-by", Encoding.UTF8.GetString(trailers[1].Key));
    Assert.Equal("Doe John <doe@john.de>", Encoding.UTF8.GetString(trailers[1].Value));
  }
}

