using fin.generic;
using RSG;

namespace fin.file {
  public static class FileUtil {
    internal static readonly ConvergingTypedHandlerMap<string>
      readHandlers =
        new ConvergingTypedHandlerMap<string>();

    internal static readonly ConvergingTypedHandlerMap<IPromise<string>>
      asyncReadHandlers =
        new ConvergingTypedHandlerMap<IPromise<string>>();

    internal static readonly ConvergingTypedHandlerMap<string>
      writeHandlers =
        new ConvergingTypedHandlerMap<string>();

    // TODO: Use input/output streams instead of promises.
    public static string ReadText(IFile file)
      => FileUtil.readHandlers.Call(file);

    public static IPromise<string> ReadTextAsync(IFile file)
      => FileUtil.asyncReadHandlers.Call(file);

    public static void WriteText(IFile file, string text) {
      //return FileUtil.writeHandlers.Call(file, text);
    }
  }
}