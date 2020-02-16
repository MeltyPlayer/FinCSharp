using fin.app.events;
using fin.events;
using fin.type;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.app.node {

  [TestClass]
  public class AppNodeTest {
    private static readonly SafeType<PassStringEvent> PASS_STRING_EVENT_TYPE = new SafeType<PassStringEvent>();

    private class PassStringEvent : BEvent {
      public string Str { get; }

      public PassStringEvent(string str) {
        this.Str = str;
      }
    }

    private static readonly SafeType<VoidEvent> VOID_EVENT_TYPE = new SafeType<VoidEvent>();

    private class VoidEvent : BEvent { }

    private static readonly Log LOG = new Log();

    private class Log {
      private string actualText_ = "";

      public void Reset() => this.actualText_ = "";
      public void Write(string text) => this.actualText_ += text;

      public void AssertText(string expectedText) => Assert.AreEqual(expectedText, this.actualText_);
    }

    private class StringEvent : BEvent { }

    private class Foo : AppNodeInternal {
      public Foo(BAppNodeInternal parent) : base(parent) {
      }

      [OnTick]
      private void PrintToLog_(VoidEvent evt) => LOG.Write("Foo");

      [OnTick]
      private void PrintToLog_(PassStringEvent evt) => LOG.Write("Foo(" + evt.Str + ")");
    }

    private class Bar : AppNodeInternal {
      public Bar(BAppNodeInternal parent) : base(parent) {
      }

      [OnTick]
      private void PrintToLog_(VoidEvent evt) => LOG.Write("Bar");

      [OnTick]
      private void PrintToLog_(PassStringEvent evt) => LOG.Write("Bar(" + evt.Str + ")");
    }

    [TestInitialize]
    public void TestInitialize() => LOG.Reset();

    [TestMethod]
    public void TestHierarchy() {
      var root = new RootAppNodeInternal();
      var son = new AppNodeInternal(root);
      var daughter = new AppNodeInternal(root);
      var grandkid = new AppNodeInternal(son);

      Assert.AreEqual(root, son.Parent);
      Assert.AreEqual(root, daughter.Parent);
      Assert.AreEqual(son, grandkid.Parent);
    }

    [TestMethod]
    public void TestManualConfigPropagateVoid() {
      var root = new RootAppNodeInternal();
      var foo = new AppNodeInternal(root);
      var bar = new AppNodeInternal(foo);

      foo.OnTick(VOID_EVENT_TYPE, _ => LOG.Write("Foo"));
      bar.OnTick(VOID_EVENT_TYPE, _ => LOG.Write("Bar"));

      root.Emit(new VoidEvent());

      LOG.AssertText("FooBar");
    }

    [TestMethod]
    public void TestManualConfigPropagateT() {
      var root = new RootAppNodeInternal();
      var foo = new AppNodeInternal(root);
      var bar = new AppNodeInternal(foo);

      foo.OnTick(PASS_STRING_EVENT_TYPE, evt => LOG.Write("Foo(" + evt.Str + ")"));
      bar.OnTick(PASS_STRING_EVENT_TYPE, evt => LOG.Write("Bar(" + evt.Str + ")"));

      root.Emit(new PassStringEvent("_"));

      LOG.AssertText("Foo(_)Bar(_)");
    }

    [TestMethod]
    public void TestAutomaticConfigPropagateVoid() {
      var root = new RootAppNodeInternal();
      var foo = new Foo(root);
      _ = new Bar(foo);

      root.Emit(new VoidEvent());

      LOG.AssertText("FooBar");
    }

    [TestMethod]
    public void TestAutomaticConfigPropagateT() {
      var root = new RootAppNodeInternal();
      var foo = new Foo(root);
      _ = new Bar(foo);

      root.Emit(new PassStringEvent("_"));

      LOG.AssertText("Foo(_)Bar(_)");
    }
  }
}