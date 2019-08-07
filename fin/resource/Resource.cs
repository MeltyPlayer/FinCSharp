using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fin.resource {
  public class Resource<DATA_TYPE> {
    private DATA_TYPE data;

    public static void From(DATA_TYPE data) {
      this.data = data;
    }
  }
}
