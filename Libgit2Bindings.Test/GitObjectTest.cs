﻿using Libgit2Bindings.Test.TestData;
using System.Text;

namespace Libgit2Bindings.Test;

public class GitObjectTest
{
  [Fact]
  public void CanLookupObject()
  {
    using var repo = new EmptyRepo();
    var oid = repo.Repo.CreateBlob(Encoding.UTF8.GetBytes("my content"));
    using var blob = repo.Repo.LookupObject(oid, GitObjectType.Blob);
    Assert.Equal(oid, blob.Id);
  }

  [Fact]
  public void CanDuplicateObject()
  {
    using var repo = new EmptyRepo();
    var oid = repo.Repo.CreateBlob(Encoding.UTF8.GetBytes("my content"));
    using var blob = repo.Repo.LookupObject(oid, GitObjectType.Blob);
    using var duplicate = blob.Duplicate();
    Assert.Equal(oid, duplicate.Id);
  }
}
