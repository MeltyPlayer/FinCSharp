using fin.data.collections.set;

namespace fin.pointer.contract.impl {

  public partial class ContractFactory : IContractFactory {

    private abstract class ContractPointerImpl<T> : IContractPointer<T> {
      private readonly OrderedSet<IStrongContractOwner<T>> strongOwners_ = new OrderedSet<IStrongContractOwner<T>>();
      private readonly OrderedSet<IWeakContractOwner<T>> weakOwners_ = new OrderedSet<IWeakContractOwner<T>>();

      public ContractPointerImpl(T value, IContractOwner<T>[] owners) {
        this.Value = value;

        this.IsActive = owners.Length > 0;
        foreach (var owner in owners) {
          this.Join(owner);
        }
      }

      public T Value { get; private set; }

      public bool Join(IContractOwner<T> owner) {
        if (!this.IsActive) {
          return false;
        }

        // TODO: This is not great but I can't think of how else to do this.
        // TODO: Seriously, figure out a better way to do this at some point.
        // TODO: Please.
        if (owner is StrongContractSet<T> strongOwner) {
          if (this.strongOwners_.Add(strongOwner)) {
            strongOwner.JoinBackdoor(this);
            return true;
          }
        } else {
          var weakOwner = (owner as WeakContractSet<T>)!;
          if (this.weakOwners_.Add(weakOwner)) {
            weakOwner.JoinBackdoor(this);
            return true;
          }
        }

        return false;
      }

      public bool IsActive { get; private set; }

      public bool Break() {
        if (!this.IsActive) {
          return false;
        }

        this.IsActive = false;

        while (this.strongOwners_.Count > 0) {
          this.strongOwners_.Last.Break(this);
        }
        this.strongOwners_.Clear();

        while (this.weakOwners_.Count > 0) {
          this.weakOwners_.Last.Break(this);
        }
        this.weakOwners_.Clear();

        return true;
      }

      /// <summary>
      ///   Inaccessible from IContractPointer types.
      /// </summary>
      public void BreakWith(IContractOwner<T> owner) {
        bool removed;

        if (owner is IStrongContractOwner<T> strongOwner) {
          removed = this.strongOwners_.Remove(strongOwner);
        } else {
          var weakOwner = (owner as IWeakContractOwner<T>)!;
          removed = this.weakOwners_.Remove(weakOwner);
        }

        if (removed && this.strongOwners_.Count == 0) {
          this.Break();
        }
      }
    }
  }
}