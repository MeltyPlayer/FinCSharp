using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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