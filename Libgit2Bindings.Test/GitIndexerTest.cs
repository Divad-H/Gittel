using Libgit2Bindings.Test.Helpers;
using Libgit2Bindings.Test.TestData;

namespace Libgit2Bindings.Test;

public class GitIndexerTest
{
  [Fact]
  public void IndexerFixesThin()
  {
    byte[] baseObject = [ 0x07, 0x3E ];

    byte[] thinPack = [
      0x50, 0x41, 0x43, 0x4b, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x02,
      0x32, 0x78, 0x9c, 0x63, 0x67, 0x00, 0x00, 0x00, 0x10, 0x00, 0x08, 0x76,
      0xe6, 0x8f, 0xe8, 0x12, 0x9b, 0x54, 0x6b, 0x10, 0x1a, 0xee, 0x95, 0x10,
      0xc5, 0x32, 0x8e, 0x7f, 0x21, 0xca, 0x1d, 0x18, 0x78, 0x9c, 0x63, 0x62,
      0x66, 0x4e, 0xcb, 0xcf, 0x07, 0x00, 0x02, 0xac, 0x01, 0x4d, 0x42, 0x52,
      0x3a, 0x6f, 0x39, 0xd1, 0xfe, 0x66, 0x68, 0x6b, 0xa5, 0xe5, 0xe2, 0x97,
      0xac, 0x94, 0x6c, 0x76, 0x0b, 0x04
    ];

    using var tempDirectory = new TemporaryDirectory();
    using var libgit2 = new Libgit2();
    using var repo = libgit2.InitRepository(Path.Combine(tempDirectory.DirectoryPath, "thin.git"), true);
    using var odb = repo.GetOdb();

    // Store the missing base into your ODB so the indexer can fix the pack
    var oid = odb.Write(baseObject, GitObjectType.Blob);

    Assert.Equal("e68fe8129b546b101aee9510c5328e7f21ca1d18", oid.Sha);

    bool callbackCalled = false;

    using var indexer = libgit2.NewGitIndexer(tempDirectory.DirectoryPath, 0, odb, new()
    {
      Verify = true,
      ProgressCallback = (progress) => 
      {
        callbackCalled = true;
        return GitOperationContinuation.Continue;
      }
    });

    var appendStats = indexer.Append(thinPack);
    appendStats = indexer.Commit();

    Assert.Equal(2u, appendStats.TotalObjects);
    Assert.Equal(2u, appendStats.ReceivedObjects);
    Assert.Equal(2u, appendStats.IndexedObjects);
    Assert.Equal(1u, appendStats.LocalObjects);

    Assert.Equal("fefdb2d740a3a6b6c03a0c7d6ce431c6d5810e13", indexer.Name);

    Assert.True(callbackCalled);
  }
}
