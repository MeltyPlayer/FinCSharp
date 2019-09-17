using System.Collections.Generic;

/*namespace fin.dispose {
  public class SafeDisposalScope : IDisposableParent {
    private static readonly Stack<SafeDisposalScope> scopes_;

    public static SafeDisposalScope GetCurrentScope() {
      return SafeDisposalScope.scopes_.Peek();
    }

    public static SafeDisposalScope EnterScope() {
      SafeDisposalScope.scopes_.Push(new SafeDisposalScope());
    }

    static SafeDisposalScope() {
      SafeDisposalScope.scopes_ = new Stack<SafeDisposalScope>();
      SafeDisposalScope.EnterScope();
    }

    private SafeDisposalScope() {
      
    }
  }
}*/