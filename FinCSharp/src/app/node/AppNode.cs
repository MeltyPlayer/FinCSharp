using System;
using System.Linq;

using fin.assert;
using fin.data.graph;
using fin.events;
using fin.type;

namespace fin.app.node {

  public class RootNode : AppNodeImpl {
    public RootNode() : base(null) {
    }
  }

  public class AppNode : AppNodeImpl {
    public AppNode(AppNodeImpl parent) : base(parent) {
    }

    public AppNodeImpl Parent {
      get => this.ParentImpl!;
      set => this.ParentImpl = value;
    }
  }

  /// <summary>
  ///   Do NOT directly inherit from this.
  /// </summary>
  // TODO: Better way to enforce this???
  public abstract class AppNodeImpl {
    private readonly Node<AppNodeImpl> node_;

    private readonly IEventListener listener_ = IEventFactory.Instance.NewListener();

    private readonly IEventRelay downwardRelay_ = IEventFactory.Instance.NewRelay();

    public delegate void OnDestroyHandler();

    public event OnDestroyHandler OnDestroy = delegate { };

    public AppNodeImpl(AppNodeImpl? parent) {
      this.node_ = new Node<AppNodeImpl>(this);
      this.ParentImpl = parent;

      this.OnDestroy += this.UnsubscribeAll_;

      this.AddOnTickHandlers_();
    }

    private void AddOnTickHandlers_() {
      var type = this.GetType();
      var methods = type.GetMethods();

      var onTick = methods.Where(m => m.Name == "OnTick").Single();

      var onTickHandlers = methods
        .Where(m => m.GetCustomAttributes(typeof(OnTickAttribute), true).Length > 0)
        .ToArray();
      foreach (var onTickHandler in onTickHandlers) {
        var parameters = onTickHandler.GetParameters();
        var eventParameter = parameters[0];

        var eventParameterType = eventParameter.ParameterType;

        var safeTypeType = typeof(SafeType<>).MakeGenericType(eventParameterType!);
        var safeTypeConstructor = safeTypeType.GetConstructor(Array.Empty<Type>());
        var safeEventParameterType = safeTypeConstructor!.Invoke(Array.Empty<object>());

        var actionType = typeof(Action<>).MakeGenericType(eventParameterType);
        Action<dynamic> handler = evt => onTickHandler.CreateDelegate(actionType, this).DynamicInvoke(new[] { evt });

        onTick.MakeGenericMethod(eventParameterType).Invoke(this, new object[] { safeEventParameterType, handler });
      }
    }

    public bool IsDestroyed => this.node_.IncomingNodes.Count() == 0;

    // TODO: Schedule for destruction, handle this at a later time.
    public void Destroy() {
      Asserts.False(this.IsDestroyed);

      this.node_.RemoveAllIncoming();
      this.OnDestroy();

      this.ForChildren(child => child.Destroy());
    }

    // Hierarchy logic.
    protected AppNodeImpl? ParentImpl {
      get {
        var incoming = this.node_.IncomingNodes;

        if (incoming.Count() > 0) {
          return incoming.Single().Value;
        }
        return null;
      }
      set {
        var oldParent = this.ParentImpl;
        if (oldParent != null) {
          oldParent.node_.RemoveOutgoing(this.node_);
          this.downwardRelay_.RemoveRelaySource(oldParent.downwardRelay_);
        }

        var newParent = value;
        if (newParent != null) {
          Asserts.Different(this, newParent);
          newParent.node_.AddOutgoing(this.node_);
          this.downwardRelay_.AddRelaySource(newParent.downwardRelay_);
        }
      }
    }

    public void ForChildren(Action<AppNodeImpl> handler) {
      foreach (var childNode in this.node_.OutgoingNodes) {
        var child = childNode.Value;
        handler(child);
      }
    }

    // Event logic.
    public void Emit<TEvent>(TEvent evt) where TEvent : IEvent => this.downwardRelay_.Emit(evt);
    public IEventSubscription? OnTick<TEvent>(SafeType<TEvent> eventType, Action<TEvent> handler) where TEvent : IEvent {
      if (this.ParentImpl != null) {
        return this.ParentImpl.downwardRelay_.AddListener(this.listener_, eventType, handler);
      }
      return null;
    }

    private void UnsubscribeAll_() {
      this.listener_.UnsubscribeAll();
      this.downwardRelay_.Destroy();
    }
  }
}