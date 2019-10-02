using fin.input.impl.opentk;

namespace fin.input {

  public interface IInput {
    IController Controller { get; }
  }

  public class OpenTkInput : IInput {
    private readonly IKeyStateDictionary ksd_;
    private readonly IKeyButtonDictionary kbd_;

    public OpenTkInput() {
      this.ksd_ = new KeyStateDictionary();
      this.kbd_ = new OpenTkKeyButtonDictionary(this.ksd_);

      this.Controller = new KeyboardController(this.kbd_);
    }

    public IController Controller { get; }
  }
}