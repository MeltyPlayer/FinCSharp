using System.Reactive.Subjects;

namespace fin.emulation.gb.memory {
  public interface ISerialBus {
    ISubject<byte> Bytes { get; }
  }

  public class SerialBus : ISerialBus {
    public ISubject<byte> Bytes { get; } = new Subject<byte>();
  }
}