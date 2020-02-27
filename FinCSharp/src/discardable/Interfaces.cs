namespace fin.discardable {
  public interface IDiscardable {
    bool IsDiscarded { get; }
  }

  public interface IPubliclyDiscardable : IDiscardable {
    bool Discard();
  }

  public interface IEventDiscardable : IDiscardable {
    public delegate void OnDiscardHandler(IEventDiscardable discardable);

    event OnDiscardHandler OnDiscard;
  }

  public interface IDependentDiscardable : IDiscardable {
    bool AddParent(IEventDiscardable parent);
    bool RemoveParent(IEventDiscardable parent);
  }
}