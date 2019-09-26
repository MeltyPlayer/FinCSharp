using System;
using System.Collections.Generic;
using System.Linq;

namespace fin.generic {

  public static class TypeUtil {

    public static Type[] GetAllBaseTypes(object instance) => GetAllBaseTypes(instance.GetType());

    public static Type[] GetAllBaseTypes(Type t) {
      var baseTypes = new List<Type>();

      var baseType = t.BaseType;
      while (baseType != typeof(object)) {
        baseTypes.Add(baseType!);
        baseType = baseType!.BaseType;
      }

      baseTypes.Reverse();
      return baseTypes.ToArray();
    }

    public static Type[] GetAllInterfaces(object instance) => GetAllInterfaces(instance.GetType());

    public static Type[] GetAllInterfaces(Type t) => t.GetInterfaces();

    public static Type[] GetImplementationsOfGenericInterface(object instance, Type iface) {
      var ifaces = TypeUtil.GetAllInterfaces(instance);
      return ifaces.Where(t => t.IsGenericType && t.Name == iface.Name).ToArray();
    }
  }
}