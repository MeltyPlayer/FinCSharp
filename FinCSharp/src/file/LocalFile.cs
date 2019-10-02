﻿using System.IO;

namespace fin.file {
  public class LocalFile : IFile {
    public string uri { get; }

    private LocalFile(string uri) {
      // TODO: Verify formatting.
      this.uri = uri;
    }

    public static LocalFile At(string absolutePath) {
      return new LocalFile(absolutePath);
    }

    // TODO: Use resources/ as base?
    public static LocalFile WithinResources(string relativePath) {
      return null;
    }

    public bool Exists() {
      return File.Exists(this.uri);
    }
  }
}