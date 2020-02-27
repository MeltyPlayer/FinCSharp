using RSG;

namespace fin.resource {
  public abstract class Resource {
    public delegate Promise<T> LoaderFunction<T>(ResourceUri uri);

    /*private static Dictionary<Type, LoaderFunction<Resource>>
      loaderFuncs_ =
        new Dictionary<Type, LoaderFunction<Resource>>();

    /*public static void DefineLoader<T>(LoaderFunction<T> loaderFunc) {
      Resource.loaderFuncs_[typeof(T)] = loaderFunc;
    }

    public static Resource<T> Load<T>(ResourceUri uri) {
      // TODO: Cache values that are already loaded.
      var valuePromise = Resource.loaderFuncs_[typeof(T)](uri);
      return new Resource<T>(valuePromise);
    }*/
  }

  public class Resource<T> : Resource {
    // TODO: Keep track of progress.
    /*private T value_;

    public Resource(T value) {
      this.value_ = value;
    }

    public Resource(IPromise<T> valuePromise) {
      valuePromise.Then((value) => this.value_ = value);
    }*/

    // TODO: Allow default values.
  }
}