namespace fin.input.impl.opentk {
  using fin.input;

  public class InputOpenTk : IInput {
    private readonly IKeyStateDictionary ksd_;
    private readonly IKeyButtonDictionary kbd_;

    public InputOpenTk() {
      this.ksd_ = new KeyStateDictionary();
      this.kbd_ = new OpenTkKeyButtonDictionary(this.ksd_);

      this.Controller = new KeyboardController(this.kbd_);
      this.Cursor = new CursorOpenTk();
    }

    public IController Controller { get; }

    ICursor IInput.Cursor => this.Cursor;
    public CursorOpenTk Cursor { get; } = new CursorOpenTk();
  }
}
