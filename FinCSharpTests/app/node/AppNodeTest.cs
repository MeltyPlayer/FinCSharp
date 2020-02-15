using fin.events;
using fin.type;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.app.node {

  [TestClass]
  public class AppNodeTest {

    private class PassStringEvent : Event<string> { }

    private class VoidEvent : Event { }

    private static readonly Log LOG = new Log();

    private class Log {
      private string actualText_ = "";

      public void Reset() => this.actualText_ = "";
      public void Write(string text) => this.actualText_ += text;

      public void AssertText(string expectedText) => Assert.AreEqual(expectedText, this.actualText_);
    }

    private class StringEvent : Event { }

    private class Foo : AppNode {
      public Foo(AppNodeImpl parent) : base(parent) {
      }

      [OnTick]
      public void PrintToLog(VoidEvent evt) => LOG.Write("Foo");

      [OnTick]
      public void PrintToLog(PassStringEvent evt, string value) => LOG.Write("Foo(" + value + ")");
    }

    private class Bar : AppNode {
      public Bar(AppNodeImpl parent) : base(parent) {
      }

      [OnTick]
      public void PrintToLog(VoidEvent evt) => LOG.Write("Bar");

      [OnTick]
      public void PrintToLog(PassStringEvent evt, string value) => LOG.Write("Bar(" + value + ")");
    }

    [TestInitialize]
    public void TestInitialize() => LOG.Reset();

    [TestMethod]
    public void TestHierarchy() {
      var root = new RootNode();
      var son = new AppNode(root);
      var daughter = new AppNode(root);
      var grandkid = new AppNode(son);

      Assert.AreEqual(root, son.Parent);
      Assert.AreEqual(root, daughter.Parent);
      Assert.AreEqual(son, grandkid.Parent);
    }

    [TestMethod]
    public void TestManualConfigPropagateVoid() {
      var root = new RootNode();
      var foo = new AppNode(root);
      var bar = new AppNode(foo);

      foo.OnTick(new SafeType<Event>(typeof(VoidEvent)), _ => LOG.Write("Foo"));
      bar.OnTick(new SafeType<Event>(typeof(VoidEvent)), _ => LOG.Write("Bar"));

      root.Emit(new VoidEvent());

      LOG.AssertText("FooBar");
    }

    [TestMethod]
    public void TestManualConfigPropagateT() {
      var root = new RootNode();
      var foo = new AppNode(root);
      var bar = new AppNode(foo);

      foo.OnTick(new SafeType<Event<string>>(typeof(PassStringEvent)), (_, s) => LOG.Write("Foo(" + s + ")"));
      bar.OnTick(new SafeType<Event<string>>(typeof(PassStringEvent)), (_, s) => LOG.Write("Bar(" + s + ")"));

      root.Emit(new PassStringEvent(), "_");

      LOG.AssertText("Foo(_)Bar(_)");
    }

    [TestMethod]
    public void TestAutomaticConfigPropagateVoid() {
      var root = new RootNode();
      var foo = new Foo(root);
      _ = new Bar(foo);

      root.Emit(new VoidEvent());

      LOG.AssertText("FooBar");
    }

    [TestMethod]
    public void TestAutomaticConfigPropagateT() {
      var root = new RootNode();
      var foo = new Foo(root);
      _ = new Bar(foo);

      root.Emit(new PassStringEvent(), "_");

      LOG.AssertText("Foo(_)Bar(_)");
    }
  }
}