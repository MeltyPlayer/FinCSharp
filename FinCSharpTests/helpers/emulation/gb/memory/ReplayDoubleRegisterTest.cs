using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.helpers.emulation.gb.memory {
  [TestClass]
  public class ReplayDoubleRegisterTest {
    public ReplayDoubleRegister New(
        ushort defaultValue,
        LockState defaultLockState)
      => new ReplayDoubleRegister("!", "?", defaultValue, defaultLockState);

    [TestMethod]
    public void TestReusesDefaultValueAfterReset() {
      var register = this.New(0xabcd, LockState.CAN_READ_AND_WRITE);
      register.Value = 0xbcde;
      Assert.AreEqual(0xbcde, register.Value);

      register.Reset();

      Assert.AreEqual(0xabcd, register.Value);
    }

    [TestMethod]
    public void TestReusesDefaultLockStateAfterReset() {
      var register = this.New(0, LockState.CAN_READ_AND_WRITE);
      register.LockState = LockState.LOCKED;
      Assert.AreEqual(LockState.LOCKED, register.LockState);

      register.Reset();

      Assert.AreEqual(LockState.CAN_READ_AND_WRITE, register.LockState);
    }

    [TestMethod]
    public void TestReadOnlyWhenAByteIsReadOnly() {
      var register = this.New(0, LockState.CAN_READ_AND_WRITE);
      register.Upper_R.LockState = LockState.CAN_READ;
      Assert.AreEqual(register.LockState, LockState.CAN_READ);
    }

    [TestMethod]
    public void TestLockedWhenAByteIsLockedForwards() {
      var register = this.New(0, LockState.CAN_READ_AND_WRITE);
      register.Upper_R.LockState = LockState.CAN_READ;
      register.Lower_R.LockState = LockState.LOCKED;
      Assert.AreEqual(register.LockState, LockState.LOCKED);
    }

    [TestMethod]
    public void TestLockedWhenAByteIsLockedBackwards() {
      var register = this.New(0, LockState.CAN_READ_AND_WRITE);
      register.Upper_R.LockState = LockState.LOCKED;
      register.Lower_R.LockState = LockState.CAN_READ;
      Assert.AreEqual(register.LockState, LockState.LOCKED);
    }
  }
}