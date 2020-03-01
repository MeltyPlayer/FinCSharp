using fin.app.events;
using fin.app.node.impl;
using fin.events;
using fin.type;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.app.node {
  [TestClass]
  public class AppNodeTest {
    private static readonly IInstantiator INSTANTIATOR = new InstantiatorImpl();

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

    private static readonly Log LOG = new Log();

    private class Log {
      private string actualText_ = string.Empty;

      public void Reset() => this.actualText_ = string.Empty;

      public void Write(string text) => this.actualText_ += text;

      public void AssertText(string expectedText) =>
        Assert.AreEqual(expectedText, this.actualText_);
    }

    private class StringEvent : BEvent {}

    private class FooComponent : BComponent {
      protected override void Discard() {}

      [OnTick]
      private void PrintToLog_(VoidEvent evt) => LOG.Write("Foo");

      [OnTick]
      private void PrintToLog_(PassStringEvent evt) =>
        LOG.Write("Foo(" + evt.Str + ")");
    }

    private class BarComponent : BComponent {
      protected override void Discard() {}

      [OnTick]
      private void PrintToLog_(VoidEvent evt) => LOG.Write("Bar");

      [OnTick]
      private void PrintToLog_(PassStringEvent evt) =>
        LOG.Write("Bar(" + evt.Str + ")");
    }

    [TestInitialize]
    public void TestInitialize() => LOG.Reset();

    [TestMethod]
    public void TestHierarchy() {
      var root = INSTANTIATOR.NewRoot();
      var son = INSTANTIATOR.NewChild(root);
      var daughter = INSTANTIATOR.NewChild(root);
      var grandkid = INSTANTIATOR.NewChild(son);

      Assert.AreEqual(root, son.Parent);
      Assert.AreEqual(root, daughter.Parent);
      Assert.AreEqual(son, grandkid.Parent);
    }

    [TestMethod]
    public void TestManualConfigPropagateVoid() {
      var root = INSTANTIATOR.NewRoot();
      var foo = INSTANTIATOR.NewChild(root);
      var bar = INSTANTIATOR.NewChild(foo);

      foo.OnTick(VOID_EVENT_TYPE, _ => LOG.Write("Foo"));
      bar.OnTick(VOID_EVENT_TYPE, _ => LOG.Write("Bar"));

      root.Emit(new VoidEvent());

      LOG.AssertText("FooBar");
    }

    [TestMethod]
    public void TestManualConfigPropagateT() {
      var root = INSTANTIATOR.NewRoot();
      var foo = INSTANTIATOR.NewChild(root);
      var bar = INSTANTIATOR.NewChild(foo);

      foo.OnTick(
        PASS_STRING_EVENT_TYPE,
        evt => LOG.Write("Foo(" + evt.Str + ")"));
      bar.OnTick(
        PASS_STRING_EVENT_TYPE,
        evt => LOG.Write("Bar(" + evt.Str + ")"));

      root.Emit(new PassStringEvent("_"));

      LOG.AssertText("Foo(_)Bar(_)");
    }

    [TestMethod]
    public void TestAutomaticConfigPropagateVoid() {
      var root = INSTANTIATOR.NewRoot();
      var foo = INSTANTIATOR.NewChild(root, new FooComponent());
      _ = INSTANTIATOR.NewChild(foo, new BarComponent());

      root.Emit(new VoidEvent());

      LOG.AssertText("FooBar");
    }

    [TestMethod]
    public void TestAutomaticConfigPropagateT() {
      var root = INSTANTIATOR.NewRoot();
      var foo = INSTANTIATOR.NewChild(root, new FooComponent());
      _ = INSTANTIATOR.NewChild(foo, new BarComponent());

      root.Emit(new PassStringEvent("_"));

      LOG.AssertText("Foo(_)Bar(_)");
    }
  }
}