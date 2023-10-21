namespace Libgit2Bindings.Mappers;

internal static class PackbuilderStageMapper
{
  public static PackbuilderStage ToManaged(this libgit2.GitPackbuilderStageT nativeStage)
  {
    return nativeStage switch 
    { 
      libgit2.GitPackbuilderStageT.GIT_PACKBUILDER_ADDING_OBJECTS => PackbuilderStage.AddingObjects,
      libgit2.GitPackbuilderStageT.GIT_PACKBUILDER_DELTAFICATION => PackbuilderStage.Deltafication, 
      _ => throw new ArgumentOutOfRangeException(nameof(nativeStage), nativeStage, null) };
  }
}
