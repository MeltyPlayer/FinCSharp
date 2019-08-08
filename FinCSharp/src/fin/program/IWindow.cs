using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fin.program {
  public interface IWindow {
    int width { get; set; }
    int height { get; set; }

    void Close();
  }
}
