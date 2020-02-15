using System;
using System.Globalization;
using System.Reflection;

namespace fin.type {

  // TODO: Add tests.
  public static class SafeType {
    public static SafeType<T> TypeOf<T>(T _) {
      return new SafeType<T>(typeof(T));
    }
  }

  internal interface ISafeType {
    Type Value { get; }
  }

  /// <summary>
  ///   A safer version of the common C# class Type. Wraps around a type that
  ///   must inherit from another given type.
  /// </summary>
  public sealed class SafeType<T> : Type, ISafeType {
    public Type Value { get; }

    public SafeType() {
      this.Value = typeof(T);
    }

    public SafeType(Type value) {
      this.Value = value;
    }

    public override int GetHashCode() => this.Value.GetHashCode();
    public override bool Equals(object? o) {
      if (o is ISafeType) {
        return this.Value.Equals((o as ISafeType)!.Value);
      } else if (o is Type) {
        return this.Value.Equals(o);
      }
      return false;
    }

    public override Assembly Assembly => this.Value.Assembly;
    public override string? AssemblyQualifiedName => this.Value.AssemblyQualifiedName;
    public override Type? BaseType => this.Value.BaseType;
    public override string? FullName => this.Value.FullName;
    public override Guid GUID => this.Value.GUID;
    public override Module Module => this.Value.Module;
    public override string? Namespace => this.Value.Namespace;
    public override Type UnderlyingSystemType => this.Value.UnderlyingSystemType;
    public override string Name => this.Value.Name;
    public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr) => this.Value.GetConstructors(bindingAttr);
    public override object[] GetCustomAttributes(bool inherit) => this.Value.GetCustomAttributes(inherit);
    public override object[] GetCustomAttributes(Type attributeType, bool inherit) => this.Value.GetCustomAttributes(attributeType, inherit);
    public override Type? GetElementType() => this.Value.GetElementType();
    public override EventInfo? GetEvent(string name, BindingFlags bindingAttr) => this.Value.GetEvent(name, bindingAttr);
    public override EventInfo[] GetEvents(BindingFlags bindingAttr) => this.Value.GetEvents(bindingAttr);
    public override FieldInfo? GetField(string name, BindingFlags bindingAttr) => this.Value.GetField(name, bindingAttr);
    public override FieldInfo[] GetFields(BindingFlags bindingAttr) => this.Value.GetFields(bindingAttr);

    public override Type? GetInterface(string name, bool ignoreCase) {
      throw new NotImplementedException();
    }

    public override Type[] GetInterfaces() {
      throw new NotImplementedException();
    }

    public override MemberInfo[] GetMembers(BindingFlags bindingAttr) {
      throw new NotImplementedException();
    }

    public override MethodInfo[] GetMethods(BindingFlags bindingAttr) {
      throw new NotImplementedException();
    }

    public override Type? GetNestedType(string name, BindingFlags bindingAttr) {
      throw new NotImplementedException();
    }

    public override Type[] GetNestedTypes(BindingFlags bindingAttr) {
      throw new NotImplementedException();
    }
    public override PropertyInfo[] GetProperties(BindingFlags bindingAttr) => this.Value.GetProperties(bindingAttr);
    public override object? InvokeMember(string name, BindingFlags invokeAttr, Binder? binder, object? target, object?[]? args, ParameterModifier[]? modifiers, CultureInfo? culture, string[]? namedParameters) {
      throw new NotImplementedException();
    }

    public override bool IsDefined(Type attributeType, bool inherit) => this.Value.IsDefined(attributeType, inherit);
    protected override TypeAttributes GetAttributeFlagsImpl() => throw new NotImplementedException();
    protected override ConstructorInfo? GetConstructorImpl(BindingFlags bindingAttr, Binder? binder, CallingConventions callConvention, Type[] types, ParameterModifier[]? modifiers) {
      throw new NotImplementedException();
    }

    protected override MethodInfo? GetMethodImpl(string name, BindingFlags bindingAttr, Binder? binder, CallingConventions callConvention, Type[]? types, ParameterModifier[]? modifiers) {
      throw new NotImplementedException();
    }

    protected override PropertyInfo? GetPropertyImpl(string name, BindingFlags bindingAttr, Binder? binder, Type? returnType, Type[]? types, ParameterModifier[]? modifiers) {
      throw new NotImplementedException();
    }

    protected override bool HasElementTypeImpl() {
      throw new NotImplementedException();
    }

    protected override bool IsArrayImpl() => this.IsArrayImpl();

    protected override bool IsByRefImpl() => throw new NotImplementedException();
    protected override bool IsCOMObjectImpl() => throw new NotImplementedException();
    protected override bool IsPointerImpl() {
      throw new NotImplementedException();
    }
    protected override bool IsPrimitiveImpl() {
      throw new NotImplementedException();
    }
  }
}