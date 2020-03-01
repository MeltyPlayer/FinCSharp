using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using fin.events;
using fin.type;

namespace fin.app.events {
  public interface IForOnTickMethod {
    void ForOnTickMethod<TEvent>(SafeType<TEvent> eventType,
                                 Action<TEvent> handler) where TEvent : IEvent;
  }

  [AttributeUsage(AttributeTargets.Method)]
  public class OnTickAttribute : Attribute {
    public OnTickAttribute() {}

    public delegate dynamic ForOnTickMethod();

    public static void SniffAndAddMethods(object onTickHandlerOwner,
                                          IForOnTickMethod target) {
      var forOnTickMethod = target.GetType().GetMethods()
                                  .Single(m => m.Name == "ForOnTickMethod");

      var type = onTickHandlerOwner.GetType();
      var methods = type.GetMethods(BindingFlags.Instance |
                                    BindingFlags.Static | BindingFlags.Public |
                                    BindingFlags.NonPublic);

      var onTickHandlers = methods
                           .Where(m =>
                                    m.GetCustomAttributes(
                                      typeof(OnTickAttribute),
                                      true).Length > 0)
                           .ToArray();
      foreach (var onTickHandler in onTickHandlers) {
        var parameters = onTickHandler.GetParameters();
        var eventParameter = parameters[0];

        var eventParameterType = eventParameter.ParameterType;

        var safeTypeType =
          typeof(SafeType<>).MakeGenericType(eventParameterType!);
        var safeTypeConstructor =
          safeTypeType.GetConstructor(Array.Empty<Type>());
        var safeEventParameterType =
          safeTypeConstructor!.Invoke(Array.Empty<object>());

        var actionType = typeof(Action<>).MakeGenericType(eventParameterType);
        var specificHandler =
          onTickHandler.CreateDelegate(actionType, onTickHandlerOwner);
        // TODO: Possible to remove this?
        Action<dynamic> genericHandler =
          evt => specificHandler.DynamicInvoke(evt);

        //var actionType = typeof(Action<>).MakeGenericType(eventParameterType);
        //Action<dynamic> handler = evt => onTickHandler.CreateDelegate(actionType, this).DynamicInvoke(new[] { evt });
        forOnTickMethod.MakeGenericMethod(eventParameterType).Invoke(target,
                                                                     new object
                                                                       [] {
                                                                         safeEventParameterType,
                                                                         genericHandler
                                                                       });

        //onTick.MakeGenericMethod(eventParameterType).Invoke(this, new object[] { safeEventParameterType, handler });
      }
    }
  }
}