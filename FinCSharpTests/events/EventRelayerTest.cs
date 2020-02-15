using fin.type;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace fin.events {

  [TestClass]
  public class EventRelayerTest {
    private static readonly IEventFactory FACTORY = IEventFactory.Instance;

    private static readonly SafeType<PassStringEvent> PASS_STRING_EVENT_TYPE = new SafeType<PassStringEvent>();

    private class PassStringEvent : EventImpl {
      public string Str { get; }

      public PassStringEvent(string str) {
        this.Str = str;
      }
    }

    private static readonly SafeType<VoidEvent> VOID_EVENT_TYPE = new SafeType<VoidEvent>();

    private class VoidEvent : EventImpl { }

    [TestMethod]
    public void TestEmptyEmit() {
      var emitter = FACTORY.NewEmitter();
      var relay = FACTORY.NewRelay();

      relay.AddRelaySource(emitter);
      string output = "";
      emitter.Emit(new PassStringEvent("foobar"));

      Assert.AreEqual(output, "");
    }

    [TestMethod]
    public void TestAddRelaySourceBeforeAddListener() {
      var emitter = FACTORY.NewEmitter();
      var relay = FACTORY.NewRelay();
      var listener = FACTORY.NewListener();

      relay.AddRelaySource(emitter);
      string output = "";
      relay.AddListener(listener, PASS_STRING_EVENT_TYPE, evt => output += evt.Str);
      emitter.Emit(new PassStringEvent("foobar"));

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestAddRelaySourceAfterAddListener() {
      var emitter = FACTORY.NewEmitter();
      var relay = FACTORY.NewRelay();
      var listener = FACTORY.NewListener();

      string output = "";
      relay.AddListener(listener, PASS_STRING_EVENT_TYPE, evt => output += evt.Str);
      relay.AddRelaySource(emitter);
      emitter.Emit(new PassStringEvent("foobar"));

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestDoubleAddRelaySource() {
      var emitter = FACTORY.NewEmitter();
      var relay = FACTORY.NewRelay();
      var listener = FACTORY.NewListener();

      Assert.IsTrue(relay.AddRelaySource(emitter));
      string output = "";
      relay.AddListener(listener, PASS_STRING_EVENT_TYPE, evt => output += evt.Str);
      Assert.IsFalse(relay.AddRelaySource(emitter));
      emitter.Emit(new PassStringEvent("foobar"));

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestDoubleAddListener() {
      var emitter = FACTORY.NewEmitter();
      var relay = FACTORY.NewRelay();
      var listener = FACTORY.NewListener();

      string output = "";
      Action<PassStringEvent> addToOutput = evt => output += evt.Str;
      var firstSubscription = relay.AddListener(listener, PASS_STRING_EVENT_TYPE, addToOutput);
      var secondSubscription = relay.AddListener(listener, PASS_STRING_EVENT_TYPE, addToOutput);
      relay.AddRelaySource(emitter);
      emitter.Emit(new PassStringEvent("foobar"));

      Assert.AreEqual(firstSubscription, secondSubscription);
      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestRemoveSourceKeepsListeners() {
      var emitter = FACTORY.NewEmitter();
      var relay = FACTORY.NewRelay();
      var listener = FACTORY.NewListener();

      string output = "";
      relay.AddListener(listener, PASS_STRING_EVENT_TYPE, evt => output += evt.Str);
      relay.AddRelaySource(emitter);
      relay.RemoveRelaySource(emitter);
      relay.AddRelaySource(emitter);
      emitter.Emit(new PassStringEvent("foobar"));

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestMultipleListeners() {
      var emitter = FACTORY.NewEmitter();
      var relay = FACTORY.NewRelay();
      var fooListener = FACTORY.NewListener();
      var barListener = FACTORY.NewListener();

      string output = "";
      relay.AddListener(fooListener, VOID_EVENT_TYPE, _ => output += "foo");
      relay.AddListener(barListener, VOID_EVENT_TYPE, _ => output += "bar");
      relay.AddRelaySource(emitter);
      emitter.Emit(new VoidEvent());

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestRelayAddListenerThenEmit() {
      var relay = FACTORY.NewRelay();
      var listener = FACTORY.NewListener();

      string output = "";
      relay.AddListener(listener, PASS_STRING_EVENT_TYPE, evt => output += evt.Str);
      relay.Emit(new PassStringEvent("foobar"));

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestRelayAddSourceThenEmit() {
      var parent = FACTORY.NewRelay();
      var relay = FACTORY.NewRelay();
      var listener = FACTORY.NewListener();

      string output = "";
      relay.AddRelaySource(parent);
      parent.AddListener(listener, PASS_STRING_EVENT_TYPE, evt => output += evt.Str);
      parent.Emit(new PassStringEvent("foobar"));

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestChainingRelays() {
      var top = FACTORY.NewRelay();
      var middle = FACTORY.NewRelay();
      var bottom = FACTORY.NewRelay();
      var listener = FACTORY.NewListener();

      string output = "";
      middle.AddRelaySource(top);
      bottom.AddRelaySource(middle);
      bottom.AddListener(listener, PASS_STRING_EVENT_TYPE, evt => output += evt.Str);
      top.Emit(new PassStringEvent("foobar"));

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