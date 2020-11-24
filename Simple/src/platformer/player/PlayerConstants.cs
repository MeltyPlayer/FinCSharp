using fin.math;

using CMath = System.Math;

namespace simple.platformer.player {
  public static class PlayerConstants {
    public const float HSIZE = 32;
    public const float VSIZE = 64;

    public const float GROUND_REACTION_FRAC = .5f;
    public const float GROUND_UPRIGHT_SLOW_XACC = .4f;
    public const float GROUND_UPRIGHT_FAST_XACC = .6f;

    public const float GROUND_FRICTION = .25f;
    public const float GROUND_SLIDING_FRICTION = .1f;

    public const float GROUND_DUCKED_SLOW_XACC = .6f;
    public const float GROUND_DUCKED_FAST_XACC = .6f;
    public const float DUCKED_MAX_SLOW_XSPD = 1;
    public const float DUCKED_MAX_FAST_XSPD = 2;

    public const float AIR_SLOW_XACC = .3f;
    public const float AIR_FAST_XACC = .4f;
    public const float AIR_FRICTION = .1f;

    public const float UPRIGHT_MAX_SLOW_XSPD = 7; // 5
    public const float UPRIGHT_MAX_FAST_XSPD = 7;

    public const float GRAVITY = 1.3f; // -.9

    public const float WALL_SLIDING_XSPD = 5;
    public const float WALL_SLIDING_GRAVITY = .8f;

    public const float JUMP_HEIGHT = VSIZE * 2;

    public static readonly float JUMP_SPEED =
        PlayerConstants.CalculateJumpSpeed_(JUMP_HEIGHT);

    public const float BACKFLIP_JUMP_HEIGHT = VSIZE * 3;

    public static readonly float BACKFLIP_JUMP_SPEED =
        PlayerConstants.CalculateJumpSpeed_(BACKFLIP_JUMP_HEIGHT);

    public const float LONGJUMP_HEIGHT = VSIZE * .75f;

    public static readonly float LONGJUMP_SPEED =
        PlayerConstants.CalculateJumpSpeed_(LONGJUMP_HEIGHT);

    public const float LONGJUMP_MAX_XSPD = 12;

    private static float CalculateJumpSpeed_(float height) =>
        -FinMath.Sqrt(2 * GRAVITY * height);
  }
}