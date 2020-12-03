namespace fin.io {
  public interface IIoUnit {
    string Uri { get; }
  }

  public interface IFile : IIoUnit {
  }

  public interface IPath : IIoUnit {
    IPath GetSubpath(string uri);
    IFile GetFile(string uri);
  }
}