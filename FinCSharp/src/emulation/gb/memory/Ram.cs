namespace fin.emulation.gb.memory {
  public class Ram : IRam {
    // TODO: Switch this out for a different approach; e.g. memory mapper.
    private readonly byte[] data_ = new byte[65535];

    // TODO: Do more complex logic here.
    public byte this[ushort address] {
      get => this.data_[address];
      set => this.data_[address] = value;
    }
  }
}
