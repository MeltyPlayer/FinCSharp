using fin.generic;

using RSG;

namespace fin.file {
  public static class FileUtil {
    internal static readonly ConvergingTypedHandlerMap<byte[]>
        readBytesHandlers =
            new ConvergingTypedHandlerMap<byte[]>();

    internal static readonly ConvergingTypedHandlerMap<string>
        readTextHandlers =
            new ConvergingTypedHandlerMap<string>();

    internal static readonly ConvergingTypedHandlerMap<IPromise<string>>
        asyncReadHandlers =
            new ConvergingTypedHandlerMap<IPromise<string>>();

    internal static readonly ConvergingTypedHandlerMap<string>
        writeHandlers =
            new ConvergingTypedHandlerMap<string>();

    static FileUtil() {
      FileUtil.readBytesHandlers.DefineHandler<LocalFile>(
          LocalFileUtil.ReadBytes);
      FileUtil.readTextHandlers
              .DefineHandler<LocalFile>(LocalFileUtil.ReadText);
      FileUtil.asyncReadHandlers.DefineHandler<LocalFile>(LocalFileUtil
                                                              .ReadTextAsync);
      //FileUtil.writeHandlers.DefineHandler<LocalFile>(WriteText);
    }

    // TODO: Use input/output streams instead of promises.
    public static byte[] ReadBytes(IFile file)
      => FileUtil.readBytesHandlers.Call(file);

    public static string ReadText(IFile file)
      => FileUtil.readTextHandlers.Call(file);

    public static IPromise<string> ReadTextAsync(IFile file)
      => FileUtil.asyncReadHandlers.Call(file);

    public static void WriteText(IFile file, string text) {
      //return FileUtil.writeHandlers.Call(file, text);
    }
  }
}