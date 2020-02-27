using System.Collections.Generic;

namespace fin.generic {
  public class DivergingSubclassHandlerMap<TBase> {
    private readonly ISet<TBase> callers_ = new HashSet<TBase>();

    public void Add<TChild>(TChild child) where TChild : TBase { }

    public void Call() { }
  }
}