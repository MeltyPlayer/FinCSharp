using System;
using System.Collections.Generic;
using System.Text;
using fin.discard;

namespace fin.data.collections.disposable {

  public class MapOfDisposables<TValue> {
    private readonly IDictionary<IDiscardable, TValue> impl_ = new Dictionary<IDiscardable, TValue>();
    private readonly IDictionary<IDiscardable, TValue> toAdd_ = new Dictionary<IDiscardable, TValue>();
    private readonly IDictionary<IDiscardable, TValue> toRemove_ = new Dictionary<IDiscardable, TValue>();

    private int iterationCount_ = 0;

    public void Add(IDiscardable disposable) {
      if (!disposable.IsDiscarded) {
        /*if (this.iterationCount_ == 0) {
          this.impl_.Add(disposable);
        } else {
          this.toAdd_.Add(disposable);
        }*/
      }
    }

    public void Remove(IDiscardable disposable) {
      if (!disposable.IsDiscarded) {
        /*if (this.iterationCount_ == 0) {
          this.impl_.Remove(disposable);
        } else {
          this.toRemove_.Add(disposable);
        }*/
      }
    }

    public void ForEach(Action<IDiscardable> handler) {
      ++this.iterationCount_;
      foreach (var disposable in this.impl_) {
        /*if (disposable.IsDisposed) {
          this.toRemove_.Add(disposable);
        } else {
          handler(disposable);
        }*/
      }
      --this.iterationCount_;
      this.AddAndRemove_();
    }

    private void AddAndRemove_() {
      if (this.iterationCount_ > 0) {
        return;
      }

      foreach (var disposable in this.toRemove_) {
        this.impl_.Remove(disposable);
      }
      this.toRemove_.Clear();

      foreach (var disposable in this.toAdd_) {
        /*if (!disposable.IsDisposed) {
          this.impl_.Add(disposable);
        }*/
      }
      this.toAdd_.Clear();
    }
  }

  public class EventForDisposables {
    private readonly MapOfDisposables<Action> handlers_ = new MapOfDisposables<Action>();

    public void Invoke() {
      /*this.handlers_.ForEach((IDiscardable disposable, Action handler) => {
        handler();
      });*/
    }
  }
}