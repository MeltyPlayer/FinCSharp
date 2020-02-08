/*using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.events {
  [TestClass]
  public partial class EventRelayerTest {
    [TestMethod]
    public void TestEmitNoSubscriptions() {
      var passStringEvent = new EventType<string>();
      var emitter = new EventEmitter();

      string output = "";
      emitter.Emit(passStringEvent, "foobar");

      Assert.AreEqual(output, "");
    }

    [TestMethod]
    public void TestSimpleEmit() {
      var passStringEvent = new EventType<string>();
      var emitter = new EventEmitter();
      var listener = new EventListener();

      string output = "";
      emitter.Subscribe(listener, passStringEvent, (string s) => output = s);
      emitter.Emit(passStringEvent, "foobar");

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestMultiEmit() {
      var voidEvent = new EventType();
      var emitter = new EventEmitter();
      var fooListener = new EventListener();
      var barListener = new EventListener();

      string output = "";
      emitter.Subscribe(fooListener, voidEvent, () => output += "foo");
      emitter.Subscribe(barListener, voidEvent, () => output += "bar");
      emitter.Emit(voidEvent);

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestEmitAfterUnsubscribe() {
      var passStringEvent = new EventType<string>();
      var emitter = new EventEmitter();
      var listener = new EventListener();

      string output = "";
      var subscription = emitter.Subscribe(listener, passStringEvent, (string s) => output = s);
      subscription.Unsubscribe();
      emitter.Emit(passStringEvent, "foobar");

      Assert.AreEqual(output, "");
    }

    [TestMethod]
    public void TestEmitAfterUnsubscribeAll() {
      var passStringEvent = new EventType<string>();
      var emitter = new EventEmitter();
      var listener = new EventListener();

      string output = "";
      emitter.Subscribe(listener, passStringEvent, (string s) => output = s);
      emitter.UnsubscribeAll();
      emitter.Emit(passStringEvent, "foobar");

      Assert.AreEqual(output, "");
    }
  }
}*/