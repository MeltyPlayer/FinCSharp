namespace fin.input {
  public interface IAxis {
    float Value { get; }
  }

  public class MutableAxis : IAxis {
    public float Value { get; set; }
  }
}