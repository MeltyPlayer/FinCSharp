using fin.type;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using fin.discardable;
using fin.helpers;

namespace fin.events {
  [TestClass]
  public partial class EventTest : BContractTestBase {
    private IDiscardableNode rootDiscardable_ = default;

    private static readonly IEventFactory FACTORY = IEventFactory.INSTANCE;

    protected override void OnRootDiscardableCreated(
        IDiscardableNode rootDiscardable)
      => this.rootDiscardable_ = rootDiscardable;

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

    private class VoidEvent : BEvent {}

    [TestMethod]
    public void TestEmitNoSubscriptions() {
      var emitter = FACTORY.NewEmitter(this.rootDiscardable_);

      string output = "";
      emitter.Emit(new PassStringEvent("foobar"));

      Assert.AreEqual(output, "");
    }

    [TestMethod]
    public void TestSimpleEmitAfterAddListener() {
      var emitter = FACTORY.NewEmitter(this.rootDiscardable_);
      var listener = FACTORY.NewListener(this.rootDiscardable_);

      string output = "";
      emitter.AddListener(listener,
                          PASS_STRING_EVENT_TYPE,
                          evt => output += evt.Str);
      emitter.Emit(new PassStringEvent("foobar"));

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestSimpleEmitAfterSubscribeTo() {
      var emitter = FACTORY.NewEmitter(this.rootDiscardable_);
      var listener = FACTORY.NewListener(this.rootDiscardable_);

      string output = "";
      listener.SubscribeTo(emitter,
                           PASS_STRING_EVENT_TYPE,
                           evt => output += evt.Str);
      emitter.Emit(new PassStringEvent("foobar"));

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestSimpleEmitAfterAddListenerAndSubscribeTo() {
      var emitter = FACTORY.NewEmitter(this.rootDiscardable_);
      var listener = FACTORY.NewListener(this.rootDiscardable_);

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
      var emitter = FACTORY.NewEmitter(this.rootDiscardable_);
      var fooListener = FACTORY.NewListener(this.rootDiscardable_);
      var barListener = FACTORY.NewListener(this.rootDiscardable_);

      string output = "";
      emitter.AddListener(fooListener, VOID_EVENT_TYPE, _ => output += "foo");
      emitter.AddListener(barListener, VOID_EVENT_TYPE, _ => output += "bar");
      emitter.Emit(new VoidEvent());

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestEmitAfterUnsubscribe() {
      var emitter = FACTORY.NewEmitter(this.rootDiscardable_);
      var listener = FACTORY.NewListener(this.rootDiscardable_);

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
      var emitter = FACTORY.NewEmitter(this.rootDiscardable_);
      var listener = FACTORY.NewListener(this.rootDiscardable_);

      string output = "";
      emitter.AddListener(listener,
                          PASS_STRING_EVENT_TYPE,
                          evt => output += evt.Str);
      emitter.RemoveAllListeners();
      emitter.Emit(new PassStringEvent("foobar"));

      Assert.AreEqual(output, "");
    }

    [TestMethod]
    public void TestCompileEmit() {
      var emitter = FACTORY.NewEmitter(this.rootDiscardable_);
      var emitVoid = emitter.CompileEmit<VoidEvent>();

      string output = "";

      var fooListener = FACTORY.NewListener(this.rootDiscardable_);
      emitter.AddListener(fooListener, VOID_EVENT_TYPE, _ => output += "foo");
      emitVoid(new VoidEvent());

      Assert.AreEqual(output, "foo");
    }

    [TestMethod]
    public void TestCompiledEmitWorksAfterEventIsEmpty() {
      var emitter = FACTORY.NewEmitter(this.rootDiscardable_);
      var emitVoid = emitter.CompileEmit<VoidEvent>();

      string output = "";

      var fooListener = FACTORY.NewListener(this.rootDiscardable_);
      emitter.AddListener(fooListener, VOID_EVENT_TYPE, _ => output += "foo");
      emitVoid(new VoidEvent());

      Assert.AreEqual(output, "foo");

      // Event is now empty, but emitVoid() should still work!
      fooListener.UnsubscribeAll();
      
      var barListener = FACTORY.NewListener(this.rootDiscardable_);
      emitter.AddListener(barListener, VOID_EVENT_TYPE, _ => output += "bar");
      emitter.Emit(new VoidEvent());

      Assert.AreEqual(output, "foobar");
    }
  }
}