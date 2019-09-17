using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fin.graphics.common {
  public abstract class IScreen {
    public abstract IScreen Clear();
    public abstract IScreen Clear(Color color);
  }
}
