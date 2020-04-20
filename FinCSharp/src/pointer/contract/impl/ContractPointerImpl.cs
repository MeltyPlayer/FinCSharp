using System.Linq;

using fin.data.collections.set;

namespace fin.pointer.contract.impl {
  public sealed partial class ContractFactory {
    private abstract partial class ContractPointerOwnerImpl<T> {
      private abstract class ContractPointerImpl : IContractPointer<T> {
        private readonly IFinSet<IStrongContractPointerOwner<T>>
            strongOwners_ = new FinOrderedSet<IStrongContractPointerOwner<T>>();

        private readonly IFinSet<IWeakContractPointerOwner<T>> weakOwners_ =
            new FinOrderedSet<IWeakContractPointerOwner<T>>();

        public T Value { get; }

        public ContractPointerImpl(T value, IContractPointerOwner<T>[] owners) {
          this.Value = value;

          this.IsActive = owners.Length > 0;
          foreach (var owner in owners) {
            this.Join(owner);
          }
        }

        public bool Join(IContractPointerOwner<T> owner) {
          if (!this.IsActive) {
            return false;
          }

          // TODO: This is not great but I can't think of how else to do this.
          // TODO: Seriously, figure out a better way to do this at some point.
          // TODO: Please.
          if (owner is StrongContractPointerOwner<T> strongOwner) {
            if (this.strongOwners_.Add(strongOwner)) {
              strongOwner.JoinBackdoor_(this);
              return true;
            }
          }
          else {
            var weakOwner = (owner as WeakContractPointerOwner<T>)!;
            if (this.weakOwners_.Add(weakOwner)) {
              weakOwner.JoinBackdoor_(this);
              return true;
            }
          }

          return false;
        }

        public bool IsActive { get; private set; }

        public event IContract.OnBreakHandler OnBreak = delegate {};

        public bool Break() {
          if (!this.IsActive) {
            return false;
          }

          this.IsActive = false;

          while (this.strongOwners_.Count > 0) {
            this.strongOwners_.Last().Break(this);
          }

          this.strongOwners_.Clear();

          while (this.weakOwners_.Count > 0) {
            this.weakOwners_.Last().Break(this);
          }

          this.weakOwners_.Clear();

          this.OnBreak(this);

          return true;
        }

        /// <summary>
        ///   Inaccessible from IContractPointer types.
        /// </summary>
        public void BreakWith(IContractPointerOwner<T> owner) {
          bool removed;

          if (owner is IStrongContractPointerOwner<T> strongOwner) {
            removed = this.strongOwners_.Remove(strongOwner);
          }
          else {
            var weakOwner = (owner as IWeakContractPointerOwner<T>)!;
            removed = this.weakOwners_.Remove(weakOwner);
          }

          if (removed && this.strongOwners_.Count == 0) {
            this.Break();
          }
        }
      }
    }
  }
}