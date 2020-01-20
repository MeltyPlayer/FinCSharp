using System;
using fin.discard;

namespace fin.events {

  public interface IEvent {

    void Add(IEventHandler handler, Action action);

    void Remove(IEventHandler handler, Action action);

    void Invoke();
  }

  public class Event {

    public void Add(IEventHandler handler, Action action) {
    }

    public void Remove(IEventHandler handler, Action action) {
    }

    public void Invoke() {
    }
  }

  public interface IEvent<T> where T : notnull {

    void Add(IEventHandler handler, Action<T> action);

    void Remove(IEventHandler handler, Action<T> action);
  }

  public class Event<T> : IEvent<T> where T : notnull {

    public void Add(IEventHandler handler, Action<T> action) {
    }

    public void Remove(IEventHandler handler, Action<T> action) {
    }
  }

  public interface IEventHandler {

    void Handle(IEvent ev, Action action);
  }

  public class DiscardableEventHandler : IDiscardable, IEventHandler {

    public void Handle(IEvent ev, Action action) {
      ev.Add(this, action);

      //this.OnDiscardEvent += ;
    }
  }
}