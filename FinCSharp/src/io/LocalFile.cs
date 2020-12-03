using System.IO;

using fin.assert;

namespace fin.io {
  public class LocalFile : IFile {
    public string Uri { get; }

    private LocalFile(string uri) {
      Asserts.True(Path.IsPathFullyQualified(uri));
      this.Uri = uri;
    }

    public static LocalFile At(string absoluteUri) =>
        new LocalFile(absoluteUri);

    public bool Exists() => File.Exists(this.Uri);
  }
}