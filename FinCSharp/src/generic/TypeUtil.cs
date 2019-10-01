using System;
using System.Collections.Generic;
using System.Linq;

using fin.function;

namespace fin.generic {

  public static class TypeUtil {

    public static Type[] GetAllBaseTypes(object instance) => GetAllBaseTypes(instance.GetType());

    public static Type[] GetAllBaseTypes(Type t) => GetAllBaseTypesImpl_(t);

    private static Func<Type, Type[]> GetAllBaseTypesImpl_ { get; } = Memoization.Memoize((Type t) => {
      var baseTypes = new List<Type>();

      var baseType = t.BaseType;
      while (baseType != typeof(object)) {
        baseTypes.Add(baseType!);
        baseType = baseType!.BaseType;
      }

      baseTypes.Reverse();
      return baseTypes.ToArray();
    });

    public static Type[] GetAllInterfaces(object instance) => GetAllInterfaces(instance.GetType());

    public static Type[] GetAllInterfaces(Type t) => GetAllInterfacesImpl_(t);

    private static Func<Type, Type[]> GetAllInterfacesImpl_ { get; } = Memoization.Memoize((Type t) => t.GetInterfaces());

    public static Type[] GetImplementationsOfGenericInterface(object instance, Type iface) => GetImplementationsOfGenericInterface(instance.GetType(), iface);

    public static Type[] GetImplementationsOfGenericInterface(Type t, Type iface) => GetImplementationsOfGenericInterfaceImpl_(t, iface);

    private static Func<Type, Type, Type[]> GetImplementationsOfGenericInterfaceImpl_ { get; } = Memoization.Memoize((Type t, Type iface) => {
      var ifaces = TypeUtil.GetAllInterfaces(t);
      return ifaces.Where(t => t.IsGenericType && t.Name == iface.Name).ToArray();
    });
  }
}