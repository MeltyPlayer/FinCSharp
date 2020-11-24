namespace fin.math {
  public interface IMath<TNumber> {
    // Operations
    TNumber _Mod(TNumber lhs, TNumber rhs);
    TNumber _Sqrt(TNumber value);
    TNumber _Pow(TNumber value, TNumber exponent);

    // Working w/ averages
    //TNumber Mean(params TNumber[] values);

    // Working w/ signs
    TNumber _Abs(TNumber value);
    int _Sign(TNumber value);
    
    bool _IsPositive(TNumber value);
    bool _IsNegative(TNumber value);

    // Checking number ranges
    /*bool _IsGreaterThan(TNumber lhs, TNumber rhs);
    bool _IsGreaterThanOrEqualTo(TNumber lhs, TNumber rhs);
    
    bool _IsLessThan(TNumber a, TNumber b);
    bool _IsLessThanOrEqualTo(TNumber a, TNumber b);*/
    
    bool _IsBetween(TNumber min, TNumber value, TNumber max);

    //bool IsIncreasing(params TNumber[] values);
    //bool IsDecreasing(params TNumber[] values);

    // Applying value ranges
    TNumber _Min(TNumber lhs, TNumber rhs);
    TNumber _Max(TNumber lhs, TNumber rhs);
    TNumber _Clamp(TNumber min, TNumber value, TNumber max);

    TNumber _Wrap(TNumber min, TNumber value, TNumber max);
    TNumber _AddTowards(TNumber start, TNumber end, TNumber inc);
  }
}