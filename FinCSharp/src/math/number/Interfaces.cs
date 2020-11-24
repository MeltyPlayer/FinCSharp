using System;

namespace fin.math.number {
  public interface INumber<TNumber> where TNumber : IComparable {
    TNumber Value { get; set; }
  }

  public interface IInt : INumber<int> {
  }

  public interface IFloat : INumber<float> {
  }


  public interface IRangedNumber<TNumber> : INumber<TNumber> where TNumber : IComparable {
    TNumber Min { get; }
    TNumber Max { get; }
  }

  public interface IRangedInt : IRangedNumber<int> {}

  public interface IRangedFloat : IRangedNumber<float> {}
}