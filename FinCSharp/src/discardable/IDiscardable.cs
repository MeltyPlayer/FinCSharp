using System;

namespace fin.discard {

  public abstract class IDiscardable {

    public delegate void OnDiscardEventHandler();

    public event OnDiscardEventHandler OnDiscardEvent = delegate { };

    public bool IsDiscarded { get; private set; }

    ~IDiscardable() {
      this.Discard_(false);
    }

    public void Discard() {
      this.Discard_(true);
      GC.SuppressFinalize(this);
    }

    private void Discard_(bool discarding) {
      if (this.IsDiscarded) {
        return;
      }
      this.IsDiscarded = true;

      if (!discarding) {
        // TODO: Free unmanaged resources here?
        return;
      }

      this.OnDiscardEvent();
    }
  }
}