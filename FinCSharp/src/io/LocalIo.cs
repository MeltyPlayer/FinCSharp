namespace fin.io {
  public class LocalIo {
    // TODO: Use resources/ as base?
    public static LocalFolder Resources { get; } =
      LocalFolder.At(
          "R:/Documents/CSharpWorkspace/FinCSharp/Simple/resources/");
  }
}