using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace fin.events {

  [TestClass]
  public class EventRelayerTest {
    private static readonly IEventFactory FACTORY = IEventFactory.Instance;
    private static readonly EventType<string> PASS_STRING = new EventType<string>();
    private static readonly EventType VOID = new EventType();

    [TestMethod]
    public void TestEmptyEmit() {
      var emitter = FACTORY.NewEmitter();
      var relay = FACTORY.NewRelay();

      relay.AddRelaySource(emitter);
      string output = "";
      emitter.Emit(PASS_STRING, "foobar");

      Assert.AreEqual(output, "");
    }

    [TestMethod]
    public void TestAddRelaySourceBeforeAddListener() {
      var emitter = FACTORY.NewEmitter();
      var relay = FACTORY.NewRelay();
      var listener = FACTORY.NewListener();

      relay.AddRelaySource(emitter);
      string output = "";
      relay.AddListener(listener, PASS_STRING, s => output += s);
      emitter.Emit(PASS_STRING, "foobar");

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestAddRelaySourceAfterAddListener() {
      var emitter = FACTORY.NewEmitter();
      var relay = FACTORY.NewRelay();
      var listener = FACTORY.NewListener();

      string output = "";
      relay.AddListener(listener, PASS_STRING, s => output += s);
      relay.AddRelaySource(emitter);
      emitter.Emit(PASS_STRING, "foobar");

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestDoubleAddRelaySource() {
      var emitter = FACTORY.NewEmitter();
      var relay = FACTORY.NewRelay();
      var listener = FACTORY.NewListener();

      Assert.IsTrue(relay.AddRelaySource(emitter));
      string output = "";
      relay.AddListener(listener, PASS_STRING, s => output += s);
      Assert.IsFalse(relay.AddRelaySource(emitter));
      emitter.Emit(PASS_STRING, "foobar");

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestDoubleAddListener() {
      var emitter = FACTORY.NewEmitter();
      var relay = FACTORY.NewRelay();
      var listener = FACTORY.NewListener();

      string output = "";
      Action<string> addToOutput = s => output += s;
      var firstSubscription = relay.AddListener(listener, PASS_STRING, addToOutput);
      var secondSubscription = relay.AddListener(listener, PASS_STRING, addToOutput);
      relay.AddRelaySource(emitter);
      emitter.Emit(PASS_STRING, "foobar");

      Assert.AreEqual(firstSubscription, secondSubscription);
      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestRemoveSourceKeepsListeners() {
      var emitter = FACTORY.NewEmitter();
      var relay = FACTORY.NewRelay();
      var listener = FACTORY.NewListener();

      string output = "";
      relay.AddListener(listener, PASS_STRING, s => output += s);
      relay.AddRelaySource(emitter);
      relay.RemoveRelaySource(emitter);
      relay.AddRelaySource(emitter);
      emitter.Emit(PASS_STRING, "foobar");

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestMultipleListeners() {
      var emitter = FACTORY.NewEmitter();
      var relay = FACTORY.NewRelay();
      var fooListener = FACTORY.NewListener();
      var barListener = FACTORY.NewListener();

      string output = "";
      relay.AddListener(fooListener, VOID, () => output += "foo");
      relay.AddListener(barListener, VOID, () => output += "bar");
      relay.AddRelaySource(emitter);
      emitter.Emit(VOID);

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestEmitFromRelay() {
      var relay = FACTORY.NewRelay();
      var listener = FACTORY.NewListener();

      string output = "";
      relay.AddListener(listener, PASS_STRING, s => output += s);
      relay.Emit(PASS_STRING, "foobar");

      Assert.AreEqual(output, "foobar");
    }
  }
}

/*[TestMethod]
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
}*/