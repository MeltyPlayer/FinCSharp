using System;

namespace fin.app.scene {
  using node;

  using window;

  public interface ISceneManager {
    /// <summary>
    ///   Schedules the manager to transition to the next scene at the soonest
    ///   available opportunity.
    /// </summary>
    void ScheduleGoToScene(IScene scene);

    // TODO: Maybe this should be a component instead?
    // TODO: I'm not liking these params...?
    void ExitSceneIfScheduled(IAppNode root);

    // TODO: Does it make sense to have this much power???
    void EnterSceneIfScheduled(IAppNode root, IApp app);
  }

  public class SceneManagerImpl : ISceneManager {
    private IAppNode? currentNode_;
    private IScene? pendingScene_;

    public void ScheduleGoToScene(IScene scene) {
      this.pendingScene_ = scene;
    }

    public void ExitSceneIfScheduled(IAppNode root) {
      if (this.pendingScene_ == null || this.currentNode_ == null) {
        return;
      }

      root.CompileEmit<SceneEndTickEvent>()(new SceneEndTickEvent());
      //this.currentNode_.Discard();
      this.currentNode_ = null;
    }

    public void EnterSceneIfScheduled(IAppNode root, IApp app) {
      if (this.pendingScene_ == null) {
        return;
      }

      app.Instantiator.NewChild(root, this.pendingScene_);
      root.CompileEmit<SceneInitTickEvent>()(new SceneInitTickEvent(app));
      this.pendingScene_ = null;
    }
  }
}