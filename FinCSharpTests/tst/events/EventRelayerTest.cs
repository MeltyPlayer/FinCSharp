﻿using fin.type;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using fin.discardable;
using fin.helpers;

namespace fin.events {
  [TestClass]
  public class EventRelayerTest : BContractTestBase {
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
    public void TestEmptyEmit() {
      var emitter = FACTORY.NewEmitter(this.rootDiscardable_);
      var relay = FACTORY.NewRelay(this.rootDiscardable_);

      relay.AddRelaySource(emitter);
      string output = "";
      emitter.Emit(new PassStringEvent("foobar"));

      Assert.AreEqual(output, "");
    }

    [TestMethod]
    public void TestAddRelaySourceBeforeAddListener() {
      var emitter = FACTORY.NewEmitter(this.rootDiscardable_);
      var relay = FACTORY.NewRelay(this.rootDiscardable_);
      var listener = FACTORY.NewListener(this.rootDiscardable_);

      relay.AddRelaySource(emitter);
      string output = "";
      relay.AddListener(listener,
                        PASS_STRING_EVENT_TYPE,
                        evt => output += evt.Str);
      emitter.Emit(new PassStringEvent("foobar"));

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestAddRelaySourceAfterAddListener() {
      var emitter = FACTORY.NewEmitter(this.rootDiscardable_);
      var relay = FACTORY.NewRelay(this.rootDiscardable_);
      var listener = FACTORY.NewListener(this.rootDiscardable_);

      string output = "";
      relay.AddListener(listener,
                        PASS_STRING_EVENT_TYPE,
                        evt => output += evt.Str);
      relay.AddRelaySource(emitter);
      emitter.Emit(new PassStringEvent("foobar"));

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestDoubleAddRelaySource() {
      var emitter = FACTORY.NewEmitter(this.rootDiscardable_);
      var relay = FACTORY.NewRelay(this.rootDiscardable_);
      var listener = FACTORY.NewListener(this.rootDiscardable_);

      Assert.IsTrue(relay.AddRelaySource(emitter));
      string output = "";
      relay.AddListener(listener,
                        PASS_STRING_EVENT_TYPE,
                        evt => output += evt.Str);
      Assert.IsFalse(relay.AddRelaySource(emitter));
      emitter.Emit(new PassStringEvent("foobar"));

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestDoubleAddListener() {
      var emitter = FACTORY.NewEmitter(this.rootDiscardable_);
      var relay = FACTORY.NewRelay(this.rootDiscardable_);
      var listener = FACTORY.NewListener(this.rootDiscardable_);

      string output = "";
      Action<PassStringEvent> addToOutput = evt => output += evt.Str;
      var firstSubscription =
          relay.AddListener(listener, PASS_STRING_EVENT_TYPE, addToOutput);
      var secondSubscription =
          relay.AddListener(listener, PASS_STRING_EVENT_TYPE, addToOutput);
      relay.AddRelaySource(emitter);
      emitter.Emit(new PassStringEvent("foobar"));

      Assert.AreEqual(firstSubscription, secondSubscription);
      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestRemoveSourceKeepsListeners() {
      var emitter = FACTORY.NewEmitter(this.rootDiscardable_);
      var relay = FACTORY.NewRelay(this.rootDiscardable_);
      var listener = FACTORY.NewListener(this.rootDiscardable_);

      string output = "";
      relay.AddListener(listener,
                        PASS_STRING_EVENT_TYPE,
                        evt => output += evt.Str);
      relay.AddRelaySource(emitter);
      relay.RemoveRelaySource(emitter);
      relay.AddRelaySource(emitter);
      emitter.Emit(new PassStringEvent("foobar"));

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestMultipleListeners() {
      var emitter = FACTORY.NewEmitter(this.rootDiscardable_);
      var relay = FACTORY.NewRelay(this.rootDiscardable_);
      var fooListener = FACTORY.NewListener(this.rootDiscardable_);
      var barListener = FACTORY.NewListener(this.rootDiscardable_);

      string output = "";
      relay.AddListener(fooListener, VOID_EVENT_TYPE, _ => output += "foo");
      relay.AddListener(barListener, VOID_EVENT_TYPE, _ => output += "bar");
      relay.AddRelaySource(emitter);
      emitter.Emit(new VoidEvent());

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestRelayAddListenerThenEmit() {
      var relay = FACTORY.NewRelay(this.rootDiscardable_);
      var listener = FACTORY.NewListener(this.rootDiscardable_);

      string output = "";
      relay.AddListener(listener,
                        PASS_STRING_EVENT_TYPE,
                        evt => output += evt.Str);
      relay.Emit(new PassStringEvent("foobar"));

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestRelayAddSourceThenEmit() {
      var parent = FACTORY.NewRelay(this.rootDiscardable_);
      var relay = FACTORY.NewRelay(this.rootDiscardable_);
      var listener = FACTORY.NewListener(this.rootDiscardable_);

      string output = "";
      relay.AddRelaySource(parent);
      parent.AddListener(listener,
                         PASS_STRING_EVENT_TYPE,
                         evt => output += evt.Str);
      parent.Emit(new PassStringEvent("foobar"));

      Assert.AreEqual(output, "foobar");
    }

    [TestMethod]
    public void TestChainingRelays() {
      var top = FACTORY.NewRelay(this.rootDiscardable_);
      var middle = FACTORY.NewRelay(this.rootDiscardable_);
      var bottom = FACTORY.NewRelay(this.rootDiscardable_);
      var listener = FACTORY.NewListener(this.rootDiscardable_);

      string output = "";
      middle.AddRelaySource(top);
      bottom.AddRelaySource(middle);
      bottom.AddListener(listener,
                         PASS_STRING_EVENT_TYPE,
                         evt => output += evt.Str);
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