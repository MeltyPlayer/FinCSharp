using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.src.events {

  [TestClass]
  public partial class EventTest {

    [TestMethod]
    public void TestEmitNoSubscriptions() {
      var passString = new EventType<string>();
      var emitter = new EventEmitter();

      string output = "";
      emitter.Emit(passString, "foobar");

      Assert.AreEqual(output, "");
    }

    [TestMethod]
    public void TestSimpleEmit() {
      var passString = new EventType<string>();
      var emitter = new EventEmitter();
      var listener = new EventListener();

      string output = "";
      emitter.Subscribe(listener, passString, (string s) => output = s);
      emitter.Emit(passString, "foobar");

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestMultiEmit() {
      var passString = new EventType<bool>();
      var emitter = new EventEmitter();
      var fooListener = new EventListener();
      var barListener = new EventListener();

      string output = "";
      emitter.Subscribe(fooListener, passString, (bool b) => output += "foo");
      emitter.Subscribe(barListener, passString, (bool b) => output += "bar");
      emitter.Emit(passString, true);

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestEmitAfterUnsubscribe() {
      var passString = new EventType<string>();
      var emitter = new EventEmitter();
      var listener = new EventListener();

      string output = "";
      var subscription = emitter.Subscribe(listener, passString, (string s) => output = s);
      subscription.Unsubscribe();
      emitter.Emit(passString, "foobar");

      Assert.AreEqual(output, "");
    }

    [TestMethod]
    public void TestEmitAfterUnsubscribeAll() {
      var passString = new EventType<string>();
      var emitter = new EventEmitter();
      var listener = new EventListener();

      string output = "";
      emitter.Subscribe(listener, passString, (string s) => output = s);
      emitter.UnsubscribeAll();
      emitter.Emit(passString, "foobar");

      Assert.AreEqual(output, "");
    }
  }
}