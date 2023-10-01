using Libgit2Bindings.Test.Helpers;

namespace Libgit2Bindings.Test;

public class CommitTest
{
  [Fact]
  public void CanCreateCommit()
  {
    const string filename = "test.txt";

    using var libgit2 = new Libgit2();
    using var tempDirectory = new TemporaryDirectory();

    using var repo = libgit2.InitRepository(tempDirectory.DirectoryPath, false);

    using var signature = repo.GitSignatureNow("John Doe", "john@doe.de");

    var fileFullPath = Path.Combine(tempDirectory.DirectoryPath, filename);
    File.WriteAllLines(fileFullPath, Array.Empty<string>());

    using var index = repo.GetIndex();
    
    index.AddByPath(filename);
    var treeOid = index.WriteTree();
    index.Write();
    
    using var tree = repo.LookupTree(treeOid);
    
    var commitOid = repo.CreateCommit("HEAD", signature, signature, null, "Initial commit", tree, null);
    
    Assert.NotNull(commitOid);
  }
}
