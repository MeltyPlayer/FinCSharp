namespace fin.io {
  public class WebFile : IFile {
    public string Uri { get; }

    private WebFile(string uri) {
      // TODO: Verify formatting.
      this.Uri = uri;
    }

    public static WebFile At(string url) =>
        new WebFile(url);
  }
}