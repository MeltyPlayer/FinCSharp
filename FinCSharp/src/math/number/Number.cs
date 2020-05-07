using System;

namespace fin.math.number {
  public interface INumber<TNumber> where TNumber : IComparable {
    TNumber Value { get; set; }
  }

  public sealed class Number<TNumber> : INumber<TNumber> where TNumber : IComparable {
    public TNumber Value { get; set; }

    public Number(TNumber initialValue) {
      this.Value = initialValue;
    }
  }
}