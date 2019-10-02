namespace fin.instantiable {
  /*public class Instantiation {
    public class Instantiator {
      private class InstantiatorKey {
        public InstantiatorKey() {
        }
      }

      public abstract class WithoutParams {
        protected WithoutParams(InstantiatorKey key) {
          this.OnInstantiation();
        }

        protected abstract void OnInstantiation();
      }

      public abstract class WithParams<TParams> {
        protected WithParams(InstantiatorKey key, TParams tParams) {
          this.OnInstantiation(tParams);
        }

        protected abstract void OnInstantiation(TParams tParams);
      }

      public TInstantiable Instantiate<TInstantiable>() where TInstantiable : Instantiable {
        // TODO: Memoize these constructors?
        var ctor = typeof(TInstantiable).GetConstructor(new Type[] { typeof(InstantiatorKey) });
        return (TInstantiable)ctor!.Invoke(new object[] { new InstantiatorKey() });
      }

      public TInstantiableWithParams Instantiate<TInstantiableWithParams, TParams>(TParams param) where TInstantiableWithParams : Instantiable<TParams> {
        var ctor = typeof(TInstantiableWithParams).GetConstructor(new Type[] { typeof(InstantiatorKey), typeof(TParams) });
        return (TInstantiableWithParams)ctor!.Invoke(new object[] { new InstantiatorKey(), param! });
      }
    }
  }*/
}