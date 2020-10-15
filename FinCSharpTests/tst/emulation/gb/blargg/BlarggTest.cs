using System;
using System.IO;
using System.Text;

using fin.emulation.gb.memory;
using fin.file;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.emulation.gb.blargg {
  [TestClass]
  public class BlarggTest {
    [TestMethod]
    public void TestAll() => this.TestFile_("cpu_instrs.gb");

    [TestMethod]
    public void Test01Special() => this.TestFile_("01-special.gb");

    [TestMethod]
    public void Test08Misc() => this.TestFile_("08-misc instrs.gb");

    private void TestFile_(string fileName) {
      var lcd = new Lcd();
      var serialBus = new SerialBus();
      var ioAddresses = new IoAddresses(serialBus);
      var memoryMap = new MemoryMap(ioAddresses);
      var registers = new Registers();
      var memory = new Memory(memoryMap, registers);
      var cpu = new Cpu(lcd, memory, new Opcodes(memory));

      //var romPath =
      //    "R:/Documents/CSharpWorkspace/FinCSharp/FinCSharpTests/tst/emulation/gb/blargg/cpu_instrs.gb";
      var romPath =
          "R:/Documents/CSharpWorkspace/FinCSharp/FinCSharpTests/tst/emulation/gb/blargg/" +
          fileName;

      var romFile = LocalFile.At(romPath);
      var romData = LocalFileUtil.ReadBytes(romFile);
      memoryMap.Rom = new Rom(romData);

      var output = "";
      var wrapper = new byte[1];
      serialBus.Bytes.Subscribe(b => {
        output += Convert.ToChar(b);
      });

      var outputPath = "R:/Documents/CSharpWorkspace/FinCSharp/FinCSharpTests/tst/emulation/gb/blargg/GB-Disassembler-master/output.txt";
      using var writer = new StreamWriter(outputPath);
      var pc = registers.Pc;

      var cycles = 0;
      try {
        for (int i = 0; i < 100000; ++i) {
          StringBuilder line = new StringBuilder();

          var initialPc = pc.Value;
          line.AppendFormat("0x{0:x4}:  ", initialPc);
          line.AppendFormat("{0:x2} |", memoryMap[initialPc]);
          line.AppendFormat(" af={0:x4}", registers.Af.Value);
          line.AppendFormat(" bc={0:x4}", registers.Bc.Value);
          line.AppendFormat(" de={0:x4}", registers.De.Value);
          line.AppendFormat(" hl={0:x4}", registers.Hl.Value);
          line.AppendFormat(" sp={0:x4}", registers.Sp.Value);
          line.AppendFormat(" pc={0:x4} |", registers.Pc.Value);

          line.AppendFormat(" cnt={0}", cpu.ScanlineCycleCounter/2);
          line.AppendFormat(" lcdc={0:x2}", ioAddresses.Lcdc);
          line.AppendFormat(" ly={0:x2}", ioAddresses.Ly);
          writer.WriteLine(line.ToString());
          
          cycles += cpu.ExecuteCycles(1);
        }
      }
      catch (Exception e) {
        output = cycles + ", " + registers.Pc.Value + ": " + output;
        throw new Exception(output, e);
      }

      output = cycles + ", " + registers.Pc.Value + ": " + output;

      throw new Exception(output);
    }
  }
}