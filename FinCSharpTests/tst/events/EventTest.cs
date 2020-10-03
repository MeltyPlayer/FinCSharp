using fin.type;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace fin.events {
  [TestClass]
  public partial class EventTest {
    private static readonly IEventFactory FACTORY = IEventFactory.INSTANCE;

    private static readonly SafeType<PassStringEvent> PASS_STRING_EVENT_TYPE =
      new SafeType<PassStringEvent>();

    private class PassStringEvent : BEvent {
      public string Str { get; }

      public PassStringEvent(string str) {
        this.Str = str;
      }
    }

    private static readonly SafeType<VoidEvent> VOID_EVENT_TYPE =
      new SafeType<VoidEvent>();

    private class VoidEvent : BEvent { }

    [TestMethod]
    public void TestEmitNoSubscriptions() {
      var emitter = FACTORY.NewEmitter();

      string output = "";
      emitter.Emit(new PassStringEvent("foobar"));

      Assert.AreEqual(output, "");
    }

    [TestMethod]
    public void TestSimpleEmitAfterAddListener() {
      var emitter = FACTORY.NewEmitter();
      var listener = FACTORY.NewListener();

      string output = "";
      emitter.AddListener(listener,
        PASS_STRING_EVENT_TYPE,
        evt => output += evt.Str);
      emitter.Emit(new PassStringEvent("foobar"));

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestSimpleEmitAfterSubscribeTo() {
      var emitter = FACTORY.NewEmitter();
      var listener = FACTORY.NewListener();

      string output = "";
      listener.SubscribeTo(emitter,
        PASS_STRING_EVENT_TYPE,
        evt => output += evt.Str);
      emitter.Emit(new PassStringEvent("foobar"));

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestSimpleEmitAfterAddListenerAndSubscribeTo() {
      var emitter = FACTORY.NewEmitter();
      var listener = FACTORY.NewListener();

      string output = "";
      Action<PassStringEvent> handler = evt => output += evt.Str;

      var addListenerContract =
        emitter.AddListener(listener, PASS_STRING_EVENT_TYPE, handler);
      var subscribeToContract =
        listener.SubscribeTo(emitter, PASS_STRING_EVENT_TYPE, handler);
      emitter.Emit(new PassStringEvent("foobar"));

      Assert.AreEqual(addListenerContract, subscribeToContract);
      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestMultiEmit() {
      var emitter = FACTORY.NewEmitter();
      var fooListener = FACTORY.NewListener();
      var barListener = FACTORY.NewListener();

      string output = "";
      emitter.AddListener(fooListener, VOID_EVENT_TYPE, _ => output += "foo");
      emitter.AddListener(barListener, VOID_EVENT_TYPE, _ => output += "bar");
      emitter.Emit(new VoidEvent());

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestEmitAfterUnsubscribe() {
      var emitter = FACTORY.NewEmitter();
      var listener = FACTORY.NewListener();

      string output = "";
      var subscription = emitter.AddListener(listener,
        PASS_STRING_EVENT_TYPE,
        evt => output += evt.Str);
      subscription.Unsubscribe();
      emitter.Emit(new PassStringEvent("foobar"));

      Assert.AreEqual(output, "");
    }

    [TestMethod]
    public void TestEmitAfterUnsubscribeAll() {
      var emitter = FACTORY.NewEmitter();
      var listener = FACTORY.NewListener();

      string output = "";
      emitter.AddListener(listener,
        PASS_STRING_EVENT_TYPE,
        evt => output += evt.Str);
      emitter.RemoveAllListeners();
      emitter.Emit(new PassStringEvent("foobar"));

      Assert.AreEqual(output, "");
    }
  }
}