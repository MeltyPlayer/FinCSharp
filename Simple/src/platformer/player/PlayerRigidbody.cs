namespace simple.platformer.player {
  public class PlayerRigidbody {
    public Rigidbody Rigidbody { get; set; }

    public double Width => PlayerConstants.HSIZE;
    public double Height => PlayerConstants.VSIZE;

    public double CenterX {
      get => this.X;
      set => this.X = value;
    }

    public double CenterY {
      get => this.Y - this.Height / 2;
      set => this.Y = value + this.Height / 2;
    }

    public double LeftX {
      get => this.X - this.Width / 2;
      set => this.X = value + this.Width / 2;
    }

    public double RightX {
      get => this.X + this.Width / 2;
      set => this.X = value - this.Width / 2;
    }

    public double BottomY {
      get => this.Y;
      set => this.Y = value;
    }

    public double TopY {
      get => this.Y - PlayerConstants.VSIZE;
      set => this.Y = value + PlayerConstants.VSIZE;
    }

    private double X {
      get => this.Rigidbody.X;
      set => this.Rigidbody.X = value;
    }

    private double Y {
      get => this.Rigidbody.Y;
      set => this.Rigidbody.Y = value;
    }

    public double XVelocity {
      get => this.Rigidbody.XVelocity;
      set => this.Rigidbody.XVelocity = value;
    }
    public double YVelocity {
      get => this.Rigidbody.YVelocity;
      set => this.Rigidbody.YVelocity = value;
    }
    public (double, double) Velocity {
      get => this.Rigidbody.Velocity;
      set => this.Rigidbody.Velocity = value;
    }

    public double XAcceleration {
      get => this.Rigidbody.XAcceleration;
      set => this.Rigidbody.XAcceleration = value;
    }
    public double YAcceleration {
      get => this.Rigidbody.YAcceleration;
      set => this.Rigidbody.YAcceleration = value;
    }
    public (double, double) Acceleration {
      get => this.Rigidbody.Acceleration;
      set => this.Rigidbody.Acceleration = value;
    }

  }
}
