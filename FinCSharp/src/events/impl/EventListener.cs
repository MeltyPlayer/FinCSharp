namespace fin.events.impl {

  public sealed partial class EventFactory : IEventFactory {

    public IEventListener NewListener() => new EventListener();

    private class EventListener : ContractEventOwner, IEventListener {
    }
  }
}