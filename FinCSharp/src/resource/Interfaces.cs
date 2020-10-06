using System;
using System.Collections.Generic;
using System.Text;

namespace fin.resource {

  public interface IResource {

  }

  public interface IResource<T> : IResource {

  }

  /*public interface I {

  }

  public interface IResourceManager {
    public IResource<> Get(string id) {
    }

    public IResourcePredicate<T> Add(string id) {
    }

    public IResourcePredicate<T> GetOrAdd(string id) {
    }
  }*/
}
