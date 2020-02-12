using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace fin.events {

  [TestClass]
  public partial class EventTest {
    private static readonly IEventFactory FACTORY = IEventFactory.Instance;
    private static readonly EventType<string> PASS_STRING = new EventType<string>();
    private static readonly EventType VOID = new EventType();

    [TestMethod]
    public void TestEmitNoSubscriptions() {
      var emitter = FACTORY.NewEmitter();

      string output = "";
      emitter.Emit(PASS_STRING, "foobar");

      Assert.AreEqual(output, "");
    }

    [TestMethod]
    public void TestSimpleEmitAfterAddListener() {
      var emitter = FACTORY.NewEmitter();
      var listener = FACTORY.NewListener();

      string output = "";
      emitter.AddListener(listener, PASS_STRING, (string s) => output += s);
      emitter.Emit(PASS_STRING, "foobar");

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestSimpleEmitAfterSubscribeTo() {
      var emitter = FACTORY.NewEmitter();
      var listener = FACTORY.NewListener();

      string output = "";
      listener.SubscribeTo(emitter, PASS_STRING, (string s) => output += s);
      emitter.Emit(PASS_STRING, "foobar");

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestSimpleEmitAfterAddListenerAndSubscribeTo() {
      var emitter = FACTORY.NewEmitter();
      var listener = FACTORY.NewListener();

      string output = "";
      Action<string> handler = s => output += s;

      var addListenerContract = emitter.AddListener(listener, PASS_STRING, handler);
      var subscribeToContract = listener.SubscribeTo(emitter, PASS_STRING, handler);
      emitter.Emit(PASS_STRING, "foobar");

      Assert.AreEqual(addListenerContract, subscribeToContract);
      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestMultiEmit() {
      var emitter = FACTORY.NewEmitter();
      var fooListener = FACTORY.NewListener();
      var barListener = FACTORY.NewListener();

      string output = "";
      emitter.AddListener(fooListener, VOID, () => output += "foo");
      emitter.AddListener(barListener, VOID, () => output += "bar");
      emitter.Emit(VOID);

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestEmitAfterUnsubscribe() {
      var emitter = FACTORY.NewEmitter();
      var listener = FACTORY.NewListener();

      string output = "";
      var subscription = emitter.AddListener(listener, PASS_STRING, (string s) => output = s);
      subscription.Unsubscribe();
      emitter.Emit(PASS_STRING, "foobar");

      Assert.AreEqual(output, "");
    }

    [TestMethod]
    public void TestEmitAfterUnsubscribeAll() {
      var emitter = FACTORY.NewEmitter();
      var listener = FACTORY.NewListener();

      string output = "";
      emitter.AddListener(listener, PASS_STRING, (string s) => output = s);
      emitter.RemoveAllListeners();
      emitter.Emit(PASS_STRING, "foobar");

      Assert.AreEqual(output, "");
    }
  }
}