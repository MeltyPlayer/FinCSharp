namespace fin.instantiable {
  public class Instantiation {
    public class InstantiatorKey {
      private InstantiatorKey() {}
    }

    public class Instantiator {
      /*public IInstantiable<TParams> Instantiate<TParams>(TParams tParams)
        where TParams : IInstantiableParams {
        var key = new InstantiatorKey();
      }*/
    }

    public interface IInstantiableParams {}

    public abstract class IInstantiable<TParams>
      where TParams : IInstantiableParams {
      protected delegate void OnInstantiationEventHandler(TParams tParams);

      protected event OnInstantiationEventHandler OnInstantiationEvent;
        
      protected IInstantiable(InstantiatorKey key, TParams tParams) {
        this.OnInstantiationEvent?.Invoke(tParams);
      }
    }
  }

  //public class A : Instantiation.IInstantiable<> {}
}