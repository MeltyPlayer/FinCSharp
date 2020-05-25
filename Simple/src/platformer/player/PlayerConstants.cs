using CMath = System.Math;

namespace simple.platformer.player {
  public static class PlayerConstants {
    public const double HSIZE = 32;
    public const double VSIZE = 64;

    public const double GROUND_REACTION_FRAC = .5;
    public const double GROUND_UPRIGHT_SLOW_XACC = .4;
    public const double GROUND_UPRIGHT_FAST_XACC = .6;

    public const double GROUND_FRICTION = .25;
    public const double GROUND_SLIDING_FRICTION = .1;

    public const double GROUND_DUCKED_SLOW_XACC = .6;
    public const double GROUND_DUCKED_FAST_XACC = .6;
    public const double DUCKED_MAX_SLOW_XSPD = 1;
    public const double DUCKED_MAX_FAST_XSPD = 2;

    public const double AIR_SLOW_XACC = .3;
    public const double AIR_FAST_XACC = .4;
    public const double AIR_FRICTION = .1;

    public const double UPRIGHT_MAX_SLOW_XSPD = 3;
    public const double UPRIGHT_MAX_FAST_XSPD = 6;

    public const double GRAVITY = 1.3; // -.9

    public const double JUMP_HEIGHT = VSIZE * 2;

    public static readonly double JUMP_SPEED =
        PlayerConstants.CalculateJumpSpeed_(JUMP_HEIGHT);

    public const double BACKFLIP_JUMP_HEIGHT = VSIZE * 3;

    public static readonly double BACKFLIP_JUMP_SPEED =
        PlayerConstants.CalculateJumpSpeed_(BACKFLIP_JUMP_HEIGHT);

    public const double LONGJUMP_HEIGHT = VSIZE * .75;

    public static readonly double LONGJUMP_SPEED =
        PlayerConstants.CalculateJumpSpeed_(LONGJUMP_HEIGHT);

    public const double LONGJUMP_MAX_XSPD = 12;

    private static double CalculateJumpSpeed_(double height) =>
        -CMath.Sqrt(2 * GRAVITY * height);
  }
}