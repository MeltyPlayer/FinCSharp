﻿using fin.app.node;
using fin.app.window;

namespace fin.app.scene {

  public interface ISceneManager {
    /// <summary>
    ///   Schedules the manager to transition to the next scene at the soonest
    ///   available opportunity.
    /// </summary>
    void ScheduleGoToScene(IScene scene);

    // TODO: Maybe this should be a component instead?
    // TODO: I'm not liking these params...?
    void ExitSceneIfScheduled(IRootAppNode root);
    void EnterSceneIfScheduled(IRootAppNode root, IWindowManager windowManager, IInstantiator instantiator);
  }

  public class SceneManagerImpl : ISceneManager {
    private IAppNode? currentNode_;
    private IScene? pendingScene_;

    public void ScheduleGoToScene(IScene scene) => this.pendingScene_ = scene;

    public void ExitSceneIfScheduled(IRootAppNode root) {
      if (this.pendingScene_ == null || this.currentNode_ == null) {
        return;
      }
      root.Emit(new SceneEndTickEvent());
      this.currentNode_.Discard();
      this.currentNode_ = null;
    }

    public void EnterSceneIfScheduled(IRootAppNode root, IWindowManager windowManager, IInstantiator instantiator) {
      if (this.pendingScene_ == null) {
        return;
      }
      instantiator.NewChild(root, this.pendingScene_);
      root.Emit(new SceneInitTickEvent(windowManager));
      this.pendingScene_ = null;
    }
  }
}