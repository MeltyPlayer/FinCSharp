using simple.platformer.player;
using simple.platformer.world;

using CMath = System.Math;
using Math = fin.math.Math;

namespace simple.platformer {
  public class PlayerCollider {
    public PlayerStateMachine StateMachine { get; set; }
    public Rigidbody Rigidbody { get; set; }

    public void TickCollisions() {
      var level = LevelConstants.LEVEL;
      var blockSize = LevelConstants.SIZE;

      var playerWidth = PlayerConstants.HSIZE;
      var playerHeight = PlayerConstants.VSIZE;

      var (centerColumn, centerRow) =
          ((int) CMath.Floor(this.LeftX / blockSize),
           (int) CMath.Floor(this.BottomY / blockSize));

      for (var r = -2; r <= 1; ++r) {
        for (var c = -1; c <= 1; ++c) {
          var (column, row) = (centerColumn + c, centerRow + r);

          if (!level[column, row]) {
            continue;
          }

          var (blockX, blockY) = (column * blockSize, row * blockSize);
          var blockLeftX = blockX;
          var blockRightX = blockX + blockSize;
          var blockTopY = blockY;
          var blockBottomY = blockY + blockSize;

          // Checks for vertical collisions.
          if (this.RightX > blockLeftX && this.LeftX < blockRightX) {
            if ((this.PreviousTopY > blockBottomY &&
                 this.TopY <= blockBottomY) ||
                (this.PreviousTopY >= blockBottomY &&
                 this.TopY < blockBottomY)) {
              this.StateMachine.State = PlayerState.FALLING;

              this.TopY = blockBottomY;
              this.Rigidbody.YVelocity = 0;
            }

            if (Math.IsIncreasing(this.PreviousBottomY, blockY, this.BottomY)
            ) {
              if (this.StateMachine.IsInAir) {
                this.StateMachine.State = PlayerState.STANDING;
              }

              this.BottomY = blockTopY;
              this.Rigidbody.YVelocity = 0;
            }
          }

          // Checks for horizontal collisions.
          if (this.BottomY > blockTopY && this.TopY < blockBottomY) {
            if (Math.IsIncreasing(this.PreviousRightX,
                                  blockLeftX,
                                  this.RightX)) {
              this.StateMachine.State = PlayerState.STANDING;
              this.RightX = blockLeftX;
              this.Rigidbody.XVelocity = 0;
            }

            if (Math.IsIncreasing(this.LeftX,
                                  blockRightX,
                                  this.PreviousLeftX)) {
              this.StateMachine.State = PlayerState.STANDING;
              this.LeftX = blockRightX;
              this.Rigidbody.XVelocity = 0;
            }
          }
        }
      }
    }


    public double LeftX {
      get => this.X - PlayerConstants.HSIZE / 2;
      set => this.X = value + PlayerConstants.HSIZE / 2;
    }

    public double RightX {
      get => this.X + PlayerConstants.HSIZE / 2;
      set => this.X = value - PlayerConstants.HSIZE / 2;
    }

    public double BottomY {
      get => this.Y;
      set => this.Y = value;
    }

    public double TopY {
      get => this.Y - PlayerConstants.VSIZE;
      set => this.Y = value + PlayerConstants.VSIZE;
    }


    public double PreviousLeftX => this.PreviousX - PlayerConstants.HSIZE / 2;
    public double PreviousRightX => this.PreviousX + PlayerConstants.HSIZE / 2;
    public double PreviousBottomY => this.PreviousY;
    public double PreviousTopY => this.PreviousY - PlayerConstants.VSIZE;


    public double X {
      get => this.Rigidbody.X;
      set => this.Rigidbody.X = value;
    }

    // TODO: Remove 480 references.
    // TODO: Remove flipping.
    public double Y {
      get => 480 - this.Rigidbody.Y;
      set => this.Rigidbody.Y = 480 - value;
    }

    public double PreviousX => this.Rigidbody.PreviousX;
    public double PreviousY => 480 - this.Rigidbody.PreviousY;
  }
}