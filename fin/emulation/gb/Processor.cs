using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGame.fin.emulation.gb {
  class Processor {

    private void init() {

      // 4. LD n,A
      op("LD", "A,A", "7F", 4);
      op("LD", "B,A", "47", 4);
      op("LD", "C,A", "4F", 4);
      op("LD", "D,A", "57", 4);
      op("LD", "E,A", "5F", 4);
      op("LD", "H,A", "67", 4);
      op("LD", "L,A", "6F", 4);
      op("LD", "(BC),A", "02", 8);
      op("LD", "(DE),A", "12", 8);
      op("LD", "(HL),A", "77", 8);
      op("LD", "(nn),A", "EA", 16);
    }

    private void op(string instruction, string parameters, string opcode, int cycles) {

    }
  }
}
