using System.IO;

using fin.assert;

namespace fin.file {
  public class LocalFile : IFile {
    public string uri { get; }

    private LocalFile(string uri) {
      Asserts.True(Path.IsPathFullyQualified(uri));

      this.uri = uri;
    }

    public static LocalFile At(string absolutePath) =>
        new LocalFile(absolutePath);

    // TODO: Use resources/ as base?
    public static LocalFile WithinResources(string relativePath) {
      var absolutePath =
          "R:/Documents/CSharpWorkspace/FinCSharp/Simple/resources/" +
          relativePath;
      var mergedFullPath = Path.GetFullPath(absolutePath);
      return LocalFile.At(mergedFullPath);
    }

    public bool Exists() => File.Exists(this.uri);
  }
}