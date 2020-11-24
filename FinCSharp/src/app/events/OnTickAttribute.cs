using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using fin.events;
using fin.type;

namespace fin.app.events {
  public interface IForOnTickMethod {
    void ForOnTickMethod<TEvent>(
        SafeType<TEvent> eventType,
        Action<TEvent> handler) where TEvent : IEvent;
  }

  [AttributeUsage(AttributeTargets.Method)]
  public class OnTickAttribute : Attribute {
    public static void SniffAndAddMethods(
        object onTickHandlerOwner,
        IForOnTickMethod target) {
      var forOnTickMethod = target.GetType()
                                  .GetMethods()
                                  .Single(m => m.Name == "ForOnTickMethod");

      // Gets list of all methods in the owner with OnTickAttribute. 
      var type = onTickHandlerOwner.GetType();
      var onTickHandlers = type.GetMethods(BindingFlags.Instance |
                                           BindingFlags.Static |
                                           BindingFlags.Public |
                                           BindingFlags.NonPublic)
                               .Where(m =>
                                          m.GetCustomAttributes(
                                               typeof(OnTickAttribute),
                                               true)
                                           .Length >
                                          0)
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

        forOnTickMethod.MakeGenericMethod(eventParameterType)
                       .Invoke(target,
                               // TODO: Boxes to object, worth fixing?
                               new[] {
                                   safeEventParameterType,
                                   specificHandler
                               });
      }
    }
  }
}