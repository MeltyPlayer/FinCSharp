using System;
using fin.app.events;
using fin.events;
using fin.type;

namespace fin.app.node {
  // TODO: Rather than extending these classes, is it possible to have an
  // interface that plugs in?
  // TODO: This is quite important, as the current implementation doesn't
  // fulfill the requirement of limiting instantiation.
  public abstract class BRootAppNode : BAppNode {
  }

  public abstract class BChildAppNode : BAppNode {
    public BChildAppNode(BAppNode parent) : base(parent) {
    }

    protected IEventSubscription OnTick<TEvent>(SafeType<TEvent> eventType, Action<TEvent> handler) where TEvent : IEvent
      => this.impl.OnTick(eventType, handler)!;
  }

  /// <summary>
  ///   DO NOT directly implement this class.
  /// </summary>
  // TODO: Better way to enforce this?
  public abstract class BAppNode {
    protected readonly BAppNodeInternal impl;

    public BAppNode() {
      this.impl = new RootAppNodeInternal();
      this.Init_();
    }

    public BAppNode(BAppNode parent) {
      this.impl = new AppNodeInternal(parent.impl);
      this.Init_();
    }

    private void Init_() {
      OnTickAttribute.SniffAndAddMethods(this, this.impl);
    }

    // TODO: Implement IDiscardable?
    protected void Discard() => this.impl.Destroy();

    protected void Emit<TEvent>(TEvent evt) where TEvent : IEvent
      => this.impl.Emit(evt);
  }
}