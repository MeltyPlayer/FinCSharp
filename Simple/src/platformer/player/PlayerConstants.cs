using CMath = System.Math;

namespace simple.platformer.player {
  public static class PlayerConstants {
    public const double HSIZE = 32;
    public const double VSIZE = 64;

    public const double GROUND_REACTION_FRAC = .5;
    public const double GROUND_SLOW_XACC = .4;
    public const double GROUND_FAST_XACC = .6;
    public const double GROUND_FRICTION = .25;
    public const double GROUND_SLIDING_FRICTION = .1;

    public const double AIR_SLOW_XACC = .3;
    public const double AIR_FAST_XACC = .4;
    public const double AIR_FRICTION = .1;

    public const double MAX_SLOW_XSPD = 3;
    public const double MAX_FAST_XSPD = 6;

    public const double GRAVITY = -1.3; // -.9

    public const double JUMP_HEIGHT = VSIZE * 2;

    public static readonly double JUMP_SPEED =
        PlayerConstants.CalculateJumpSpeed_(JUMP_HEIGHT);

    public const double BACKFLIP_JUMP_HEIGHT = VSIZE * 3;

    public static readonly double BACKFLIP_JUMP_SPEED =
        PlayerConstants.CalculateJumpSpeed_(BACKFLIP_JUMP_HEIGHT);

    private static double CalculateJumpSpeed_(double height) =>
        CMath.Sqrt(-2 * GRAVITY * height);
  }
}