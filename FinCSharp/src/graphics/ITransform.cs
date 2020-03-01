namespace fin.graphics {
  internal enum MatrixType {
    PROJECTION,
    MODELVIEW,
  };

  public interface ITransform {
    /*public abstract void set_target_matrix(MatrixType matrixType);

    public abstract void print_matrix();

    public abstract void push_matrix();
    public abstract void pop_matrix();

    public abstract void Identity();

    public abstract void translate(const double x, const double y);
    public abstract void translate(const double x, const double y, const double z);
    public void translate(const math::Point3d& point) {
      translate(point.x(), point.y(), point.z());
    }

    public abstract void scale(const double x, const double y);
  public abstract void scale(const double x, const double y, const double z);

    public abstract void rotate_x(const double deg);
    public abstract void rotate_y(const double deg);
    public abstract void rotate_z(const double deg);
    public abstract void rotate_around_axis(const math::INormal3d& axis,
                                    const double deg);

    public abstract void perspective(double fov, double aspect, double nearValue,
                             double farValue);
    public abstract void ortho(double left, double right, double bottom, double top,
                       double near, double far);
    public abstract void look_at(Vertex3d from, Vertex3d to,
                         const math::INormal3d& up);*/
  }
}