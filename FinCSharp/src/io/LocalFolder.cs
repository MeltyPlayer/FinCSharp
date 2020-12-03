using System.IO;

using fin.assert;

namespace fin.io {
  public class LocalFolder : IPath {
    public string Uri { get; }

    private LocalFolder(string uri) {
      Asserts.True(Path.IsPathFullyQualified(uri));
      this.Uri = uri;
    }

    public static LocalFolder At(string absoluteUri)
      => new LocalFolder(absoluteUri);

    IPath IPath.GetSubpath(string uri) => this.GetSubpath(uri);
    public LocalFolder GetSubpath(string uri)
      => LocalFolder.At(this.GetUriWithin_(uri));
    
    IFile IPath.GetFile(string uri) => this.GetFile(uri);
    public LocalFile GetFile(string uri)
      => LocalFile.At(this.GetUriWithin_(uri));
    
    private string GetUriWithin_(string uri)
      => Path.GetFullPath(this.Uri + uri);
  }
}