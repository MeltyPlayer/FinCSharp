namespace fin.app {
  public enum TickPhase {
    SCENE_INITIALIZATION = 1,
    ACTOR_MANAGEMENT = 2,
    RESOURCE_LOADING = 3,
    NET = 4,
    CONTROL = 5,
    // First apply velocity, then change in acceleration.
    PHYSICS = 6,
    COLLISION = 7,
    ANIMATION = 8,
    RENDER = 9,
  }
}
