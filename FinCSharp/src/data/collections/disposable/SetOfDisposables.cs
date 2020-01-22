using System;
using System.Collections.Generic;

using fin.discard;

namespace fin.data.collections.disposable {

  public class SetOfDisposables<T> where T : notnull, Discardable {
    private readonly ISet<T> impl_ = new HashSet<T>();
    private readonly ISet<T> toAdd_ = new HashSet<T>();
    private readonly ISet<T> toRemove_ = new HashSet<T>();

    private int iterationCount_ = 0;

    public void Add(T t) {
      if (!t.IsDiscarded) {
        if (this.iterationCount_ == 0) {
          this.impl_.Add(t);
        } else {
          this.toAdd_.Add(t);
        }
      }
    }

    public void Remove(T t) {
      if (!t.IsDiscarded) {
        if (this.iterationCount_ == 0) {
          this.impl_.Remove(t);
        } else {
          this.toRemove_.Add(t);
        }
      }
    }

    public void ForEach(Action<Discardable> handler) {
      ++this.iterationCount_;
      foreach (var t in this.impl_) {
        if (t.IsDiscarded) {
          this.toRemove_.Add(t);
        } else {
          handler(t);
        }
      }
      --this.iterationCount_;
      this.AddAndRemove_();
    }

    private void AddAndRemove_() {
      if (this.iterationCount_ > 0) {
        return;
      }

      foreach (var t in this.toRemove_) {
        this.impl_.Remove(t);
      }
      this.toRemove_.Clear();

      foreach (var t in this.toAdd_) {
        if (!t.IsDiscarded) {
          this.impl_.Add(t);
        }
      }
      this.toAdd_.Clear();
    }
  }

  public class SetOfDisposables : SetOfDisposables<Discardable> { }
}