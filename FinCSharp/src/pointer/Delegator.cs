using System.Collections.Generic;

using fin.data.collections.set;

namespace fin.pointer {
  // TODO: Rename this at some point.
  // TODO: Add tests.
  public interface IDelegator<T> {
    void Clear();

    bool Add(T value);

    bool Remove(T value);

    IEnumerable<T> All { get; }
    T First { get; }
    T Last { get; }
  }

  public class Delegator<T> : IDelegator<T> {
    private readonly OrderedSet<T> impl_ = new OrderedSet<T>();

    public void Clear() => this.impl_.Clear();

    public bool Add(T value) => this.impl_.Add(value);

    public bool Remove(T value) => this.impl_.Remove(value);

    public IEnumerable<T> All => this.impl_;
    public T First => this.impl_.First;
    public T Last => this.impl_.Last;
  }
}