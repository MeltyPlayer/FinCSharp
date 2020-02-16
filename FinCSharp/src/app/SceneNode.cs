/*using System;
using System.Collections.Generic;

using fin.app.phase;
using fin.data.collections.set;
using fin.discard;

namespace fin.app {
  public class DisposePhase {
  }

  public abstract class UiSceneNode : SceneNode<UiSceneNode> {
  }

  public abstract class TwoDSceneNode : SceneNode<TwoDSceneNode> {
  }

  public abstract class ThreeDSceneNode : SceneNode<ThreeDSceneNode> {
  }

  public abstract class SceneNode<TMe> : IPhaseHandler, IReflectivePhaseHandler, IReflectivePhaseHandler<DisposePhase> where TMe : SceneNode<TMe> {
    protected delegate void OnDisposeEventHandler();

    protected event OnDisposeEventHandler OnDisposeEvent = delegate { };

    private bool markedForDisposal_ = false;

    private readonly UnsafeDisposableDataNode<SceneNode<TMe>> disposableNodeimpl_;
    private readonly ReflectivePhaseManager reflectivePhaseManagerImpl_;
    private readonly TickHandler tickHandlerImpl_;

    // TODO: Switch out null for the parent based on current scope.
    public SceneNode(SceneNode<TMe>? parent = null) {
      this.disposableNodeimpl_ = new UnsafeDisposableDataNode<SceneNode<TMe>>(this, parent?.disposableNodeimpl_);
      this.disposableNodeimpl_.OnDiscardEvent += this.OnDispose_;

      this.reflectivePhaseManagerImpl_ = new ReflectivePhaseManager(this);

      this.tickHandlerImpl_ = new TickHandler();
      this.tickHandlerImpl_.AddHandler(this.reflectivePhaseManagerImpl_);
    }

    protected void TriggerDispose() => this.markedForDisposal_ = true;

    private void OnDispose_() => this.OnDisposeEvent();

    // TODO: Switch out null for the parent based on current scope.
    public SceneNode<TMe>? Parent => this.disposableNodeimpl_.Parent?.Data;

    public IEnumerable<SceneNode<TMe>> Children => this.disposableNodeimpl_.ChildData;

    // TODO: Remove tick handler when remove child.
    public SceneNode<TMe> Attach(params SceneNode<TMe>[] children) {
      this.tickHandlerImpl_.AddHandlers(children);
      Array.ForEach(children, (SceneNode<TMe> child) => this.disposableNodeimpl_.Attach(child.disposableNodeimpl_));
      return this;
    }

    public void Tick(params object[] phaseDatas) => this.tickHandlerImpl_.Tick(phaseDatas);

    public void OnPhase(object phaseData) {
      if (!this.disposableNodeimpl_.IsDiscarded) {
        this.tickHandlerImpl_.OnPhase(phaseData);
      }
    }

    public void OnPhase(DisposePhase phaseData) {
      if (this.markedForDisposal_) {
        this.disposableNodeimpl_.Discard();
      }
    }

    public IEnumerable<Type> HandledPhaseTypes => this.tickHandlerImpl_.HandledPhaseTypes;
  }
}*/