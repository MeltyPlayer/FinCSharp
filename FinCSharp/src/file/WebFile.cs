namespace fin.file {

  public class WebFile : IFile {
    public string uri { get; }

    private WebFile(string uri) {
      // TODO: Verify formatting.
      this.uri = uri;
    }

    public static WebFile At(string url) {
      return new WebFile(url);
    }
  }
}