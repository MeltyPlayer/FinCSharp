namespace simple.platformer.player {
  public class PlayerRigidbody {
    public Rigidbody Rigidbody { get; set; }

    public float Width { get; set; } = PlayerConstants.HSIZE;
    public float Height { get; set; } = PlayerConstants.VSIZE;

    public float CenterX {
      get => this.X;
      set => this.X = value;
    }

    public float CenterY {
      get => this.Y - this.Height / 2;
      set => this.Y = value + this.Height / 2;
    }

    public float LeftX {
      get => this.X - this.Width / 2;
      set => this.X = value + this.Width / 2;
    }

    public float RightX {
      get => this.X + this.Width / 2;
      set => this.X = value - this.Width / 2;
    }

    public float BottomY {
      get => this.Y;
      set => this.Y = value;
    }

    public float TopY {
      get => this.Y - this.Height;
      set => this.Y = value + this.Height;
    }


    public float PreviousBottomY => this.PreviousY;
    public float PreviousTopY => this.PreviousY - this.Height;
    private float PreviousY => this.Rigidbody.PreviousY;


    private float X {
      get => this.Rigidbody.X;
      set => this.Rigidbody.X = value;
    }

    private float Y {
      get => this.Rigidbody.Y;
      set => this.Rigidbody.Y = value;
    }

    public float XVelocity {
      get => this.Rigidbody.XVelocity;
      set => this.Rigidbody.XVelocity = value;
    }

    public float YVelocity {
      get => this.Rigidbody.YVelocity;
      set => this.Rigidbody.YVelocity = value;
    }

    public (float, float) Velocity {
      get => this.Rigidbody.Velocity;
      set => this.Rigidbody.Velocity = value;
    }

    public float? TargetXVelocity {
      get => this.Rigidbody.TargetXVelocity;
      set => this.Rigidbody.TargetXVelocity = value;
    }

    public float XAcceleration {
      get => this.Rigidbody.XAcceleration;
      set => this.Rigidbody.XAcceleration = value;
    }

    public float YAcceleration {
      get => this.Rigidbody.YAcceleration;
      set => this.Rigidbody.YAcceleration = value;
    }
  }
}