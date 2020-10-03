using fin.assert;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.generic {
  [TestClass]
  public class TypeUtilTest {
    private interface IABaseGenericInterface<T> { }

    private interface IAInterface1 : IABaseGenericInterface<int> { }

    private abstract class ALowestBase : IAInterface1 { }

    private interface IAInterface2 : IABaseGenericInterface<string> { }

    private abstract class AHighestBase : ALowestBase, IAInterface2 { }

    private interface IAInterface3 { }

    private class A : AHighestBase, IAInterface3 { }

    [TestMethod]
    public void TestAllBaseTypes() {
      Asserts.Equal(new[] {typeof(ALowestBase), typeof(AHighestBase)},
        TypeUtil.GetAllBaseTypes(new A()));
    }

    [TestMethod]
    public void TestGetAllInterfaces() {
      Asserts.Equal(new[] {
          typeof(IAInterface1), typeof(IABaseGenericInterface<int>),
          typeof(IAInterface2), typeof(IABaseGenericInterface<string>),
          typeof(IAInterface3)
        },
        TypeUtil.GetAllInterfaces(new A()));
    }

    [TestMethod]
    public void TestGetImplementationsOfGenericInterface() {
      var expectedTypes = new[] {
        typeof(IABaseGenericInterface<int>),
        typeof(IABaseGenericInterface<string>)
      };
      var actualTypes =
        TypeUtil.GetImplementationsOfGenericInterface(new A(),
          typeof(IABaseGenericInterface<>));

      Asserts.Equal(expectedTypes, actualTypes);
    }
  }
}