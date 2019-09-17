namespace fin.pipeline {
  public interface IConverter<in INPUT_TYPE, out OUTPUT_TYPE> {
    OUTPUT_TYPE Convert(INPUT_TYPE inputValue);
  }
}