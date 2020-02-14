using fin.type;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace fin.events {

  [TestClass]
  public partial class EventTest {
    private static readonly IEventFactory FACTORY = IEventFactory.Instance;

    private class PassStringEvent : Event<string> { }

    private SafeType<Event<string>> passStringEventType_ = new SafeType<Event<string>>(typeof(PassStringEvent));

    private class VoidEvent : Event { }

    private SafeType<Event> voidEventType_ = new SafeType<Event>(typeof(VoidEvent));

    [TestMethod]
    public void TestEmitNoSubscriptions() {
      var emitter = FACTORY.NewEmitter();

      string output = "";
      emitter.Emit(this.passStringEventType_, new PassStringEvent(), "foobar");

      Assert.AreEqual(output, "");
    }

    [TestMethod]
    public void TestSimpleEmitAfterAddListener() {
      var emitter = FACTORY.NewEmitter();
      var listener = FACTORY.NewListener();

      string output = "";
      emitter.AddListener(listener, this.passStringEventType_, (_, s) => output += s);
      emitter.Emit(this.passStringEventType_, new PassStringEvent(), "foobar");

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestSimpleEmitAfterSubscribeTo() {
      var emitter = FACTORY.NewEmitter();
      var listener = FACTORY.NewListener();

      string output = "";
      listener.SubscribeTo(emitter, this.passStringEventType_, (_, s) => output += s);
      emitter.Emit(this.passStringEventType_, new PassStringEvent(), "foobar");

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestSimpleEmitAfterAddListenerAndSubscribeTo() {
      var emitter = FACTORY.NewEmitter();
      var listener = FACTORY.NewListener();

      string output = "";
      Action<Event<string>, string> handler = (_, s) => output += s;

      var addListenerContract = emitter.AddListener(listener, this.passStringEventType_, handler);
      var subscribeToContract = listener.SubscribeTo(emitter, this.passStringEventType_, handler);
      emitter.Emit(this.passStringEventType_, new PassStringEvent(), "foobar");

      Assert.AreEqual(addListenerContract, subscribeToContract);
      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestMultiEmit() {
      var emitter = FACTORY.NewEmitter();
      var fooListener = FACTORY.NewListener();
      var barListener = FACTORY.NewListener();

      string output = "";
      emitter.AddListener(fooListener, this.voidEventType_, _ => output += "foo");
      emitter.AddListener(barListener, this.voidEventType_, _ => output += "bar");
      emitter.Emit(this.voidEventType_, new VoidEvent());

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestEmitAfterUnsubscribe() {
      var emitter = FACTORY.NewEmitter();
      var listener = FACTORY.NewListener();

      string output = "";
      var subscription = emitter.AddListener(listener, this.passStringEventType_, (_, s) => output = s);
      subscription.Unsubscribe();
      emitter.Emit(this.passStringEventType_, new PassStringEvent(), "foobar");

      Assert.AreEqual(output, "");
    }

    [TestMethod]
    public void TestEmitAfterUnsubscribeAll() {
      var emitter = FACTORY.NewEmitter();
      var listener = FACTORY.NewListener();

      string output = "";
      emitter.AddListener(listener, this.passStringEventType_, (_, s) => output = s);
      emitter.RemoveAllListeners();
      emitter.Emit(this.passStringEventType_, new PassStringEvent(), "foobar");

      Assert.AreEqual(output, "");
    }
  }
}