using System;
using System.Collections.Generic;

namespace fin.generic {
  public class TypedDictionary<T> {
    private readonly Dictionary<Type, ISet<T>> data_;

    public TypedDictionary() {
      this.data_ = new Dictionary<Type, ISet<T>>();
    }

    // TODO: Dispose

    // TODO: Make this class super-smart and automatically analyze the type.

    public ISet<T> Get(Type t) => this.data_[t];

    public bool Add(Type t, T instance) {
      var set = this.data_[t] ?? (this.data_[t] = new HashSet<T>());
      return set.Add(instance);
    }

    // TODO: Remove based on instance
  }
}