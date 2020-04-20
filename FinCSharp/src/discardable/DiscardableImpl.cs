using System;
using System.Linq;

using fin.data.collections.set;

namespace fin.discardable {
  public class DiscardableImpl : IPubliclyDiscardable, IEventDiscardable,
    IDependentDiscardable {
    private readonly IFinSet<IEventDiscardable> parents_ =
      new FinHashSet<IEventDiscardable>();

    public event IEventDiscardable.OnDiscardHandler? OnDiscard;

    public bool IsDiscarded { get; private set; }

    ~DiscardableImpl() {
      this.Discard_(false);
    }

    public bool Discard() {
      if (this.IsDiscarded) {
        return false;
      }

      this.Discard_(true);
      GC.SuppressFinalize(this);
      return true;
    }

    private void VoidDiscard_(IEventDiscardable _) => this.Discard();

    private void Discard_(bool discarding) {
      if (this.IsDiscarded) {
        return;
      }

      this.IsDiscarded = true;

      if (!discarding) {
        // TODO: Free unmanaged resources here?
        return;
      }

      this.OnDiscard?.Invoke(this);
      while (this.parents_.Count > 0) {
        this.RemoveParent(this.parents_.First());
      }
    }

    public bool AddParent(IEventDiscardable parent) {
      if (this.IsDiscarded || !this.parents_.Add(parent)) {
        return false;
      }

      parent.OnDiscard += this.VoidDiscard_;
      return true;
    }

    public bool RemoveParent(IEventDiscardable parent) {
      if (!this.parents_.Remove(parent)) {
        return false;
      }

      parent.OnDiscard -= this.VoidDiscard_;
      return true;
    }
  }
}