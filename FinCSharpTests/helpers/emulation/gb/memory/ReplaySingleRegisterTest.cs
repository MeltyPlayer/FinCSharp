using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.helpers.emulation.gb.memory {
  [TestClass]
  public class ReplaySingleRegisterTest {
    public ReplaySingleRegister New(
        byte defaultValue,
        LockState defaultLockState)
      => new ReplaySingleRegister("$", defaultValue, defaultLockState);

    [TestMethod]
    public void TestReusesDefaultValueAfterReset() {
      var register = this.New(0, LockState.CAN_READ_AND_WRITE);
      register.Value = 1;
      Assert.AreEqual(1, register.Value);

      register.Reset();

      Assert.AreEqual(0, register.Value);
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
    public void TestReadOnlyWhenABitIsReadOnly() {
      var register = this.New(0, LockState.CAN_READ_AND_WRITE);
      register.BitLockStates[0] = LockState.CAN_READ;
      Assert.AreEqual(register.LockState, LockState.CAN_READ);
    }

    [TestMethod]
    public void TestLockedWhenABitIsLockedForwards() {
      var register = this.New(0, LockState.CAN_READ_AND_WRITE);
      register.BitLockStates[0] = LockState.CAN_READ;
      register.BitLockStates[1] = LockState.LOCKED;
      Assert.AreEqual(register.LockState, LockState.LOCKED);
    }

    [TestMethod]
    public void TestLockedWhenABitIsLockedBackwards() {
      var register = this.New(0, LockState.CAN_READ_AND_WRITE);
      register.BitLockStates[0] = LockState.LOCKED;
      register.BitLockStates[1] = LockState.CAN_READ;
      Assert.AreEqual(register.LockState, LockState.LOCKED);
    }
  }
}