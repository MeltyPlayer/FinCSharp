using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

using fin.assert;
using fin.emulation.gb.memory;
using fin.emulation.gb.memory.io;
using fin.io;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.emulation.gb.blargg {
  [TestClass]
  public class BlarggTest {
    private const bool COMPARE_TO_BINJGB_ = false;
    private const bool LOG_TRACE_ = false;

    [TestMethod]
    public void TestInstrTiming() => this.TestFile_("instr_timing");
    
    [TestMethod]
    public void TestCpuInstrs() => this.TestFile_("cpu_instrs");

    [TestMethod]
    public void Test01Special() => this.TestFile_("01-special");

    [TestMethod]
    public void Test02Interrupts() => this.TestFile_("02-interrupts");

    [TestMethod]
    public void Test03OpSpHl() => this.TestFile_("03-op sp,hl");

    [TestMethod]
    public void Test04OpRImm() => this.TestFile_("04-op r,imm");

    [TestMethod]
    public void Test05OpRp() => this.TestFile_("05-op rp");

    [TestMethod]
    public void Test06LdRR() => this.TestFile_("06-ld r,r");

    [TestMethod]
    public void Test07Jump() => this.TestFile_("07-jr,jp,call,ret,rst");

    [TestMethod]
    public void Test08Misc() => this.TestFile_("08-misc instrs");

    [TestMethod]
    public void Test09OpRR() => this.TestFile_("09-op r,r");

    [TestMethod]
    public void Test10BitOps() => this.TestFile_("10-bit ops");

    [TestMethod]
    public void Test11OpAHl() => this.TestFile_("11-op a,(hl)");

    private class Expected {
      public ushort Af { get; }
      public ushort Bc { get; }
      public ushort De { get; }
      public ushort Hl { get; }
      public ushort Pc { get; }
      public int Cycles { get; }
      public int Scl { get; }
      public int PpuMode { get; }

      public Expected(
          ushort af,
          ushort bc,
          ushort de,
          ushort hl,
          ushort pc,
          int cycles,
          int scl,
          int ppuMode) {
        this.Af = af;
        this.Bc = bc;
        this.De = de;
        this.Hl = hl;
        this.Pc = pc;
        this.Cycles = cycles;
        this.Scl = scl;
        this.PpuMode = ppuMode;
      }
    }

    private void TestFile_(string fileName) {
      var lcd = new Lcd {Active = false};
      var serialBus = new SerialBus();
      var ioAddresses = new IoAddresses(serialBus);
      var memoryMap = new MemoryMap(ioAddresses);
      var registers = new Registers();
      var memory = new Mmu(memoryMap, registers);
      var cpu = new Cpu(lcd, memory, new Opcodes(memory));

      ushort initialPc = 0;
      ushort finalPc = 0;

      var maxCycleCount = 100000000;

      LinkedList<Expected?>? expecteds = null;
      if (BlarggTest.COMPARE_TO_BINJGB_) {
        var expectedTracePath =
            "R:/Documents/CSharpWorkspace/FinCSharp/FinCSharpTests/tst/emulation/gb/blargg/" +
            fileName +
            ".txt";
        expecteds = new LinkedList<Expected?>();
        try {
          using (var sr = new StreamReader(expectedTracePath)) {
            string line;
            while ((line = sr.ReadLine()) != null) {
              var words = line.Split(' ');

              var expectedAHex = words[0].Substring(2);
              var expectedA = byte.Parse(expectedAHex, NumberStyles.HexNumber);

              var expectedFText = words[1].Substring(2);
              var expectedF = 0;
              if (expectedFText[0] != '-') {
                expectedF |= 1 << 7;
              }
              if (expectedFText[1] != '-') {
                expectedF |= 1 << 6;
              }
              if (expectedFText[2] != '-') {
                expectedF |= 1 << 5;
              }
              if (expectedFText[3] != '-') {
                expectedF |= 1 << 4;
              }

              var expectedAf = ByteMath.MergeBytes(expectedA, (byte) expectedF);

              var expectedBcHex = words[2].Substring(3);
              var expectedBc = ushort.Parse(expectedBcHex,
                                            NumberStyles.HexNumber);

              var expectedDeHex = words[3].Substring(3);
              var expectedDe = ushort.Parse(expectedDeHex,
                                            NumberStyles.HexNumber);

              var expectedHlHex = words[4].Substring(3);
              var expectedHl = ushort.Parse(expectedHlHex,
                                            NumberStyles.HexNumber);

              var expectedPcHex = words[6].Substring(3);
              //var expectedPcHex = line.Substring(47, 4);
              var expectedPc = ushort.Parse(expectedPcHex,
                                            NumberStyles.HexNumber);

              var expectedCyclesText = line.Substring(57,
                                                      line.IndexOf(
                                                          ')',
                                                          57) -
                                                      57);
              var expectedCycles = int.Parse(expectedCyclesText);

              var expectedSclText = words[9].Substring(4);
              var expectedScl = int.Parse(expectedSclText);

              var expectedPpuModeText = words[12].Substring(5);
              var expectedPpuMode = int.Parse(expectedPpuModeText);

              expecteds.AddLast(new Expected(expectedAf,
                                             expectedBc,
                                             expectedDe,
                                             expectedHl,
                                             expectedPc,
                                             expectedCycles,
                                             expectedScl,
                                             expectedPpuMode));
            }
          }
        } catch (Exception e) {
          expecteds.Clear();
          expecteds = null;
        }
      }

      var romPath =
          "R:/Documents/CSharpWorkspace/FinCSharp/FinCSharpTests/tst/emulation/gb/blargg/" +
          fileName +
          ".gb";

      var romFile = LocalFile.At(romPath);
      var romData = LocalFileUtil.ReadBytes(romFile);
      memoryMap.Rom = new Rom(romData);

      var output = "";
      serialBus.Bytes.Subscribe(b => { output += Convert.ToChar(b); });

      var outputPath =
          "R:/Documents/CSharpWorkspace/FinCSharp/FinCSharpTests/tst/emulation/gb/blargg/output_" +
          fileName +
          ".txt";
      using var writer = (BlarggTest.LOG_TRACE_)
                             ? new StreamWriter(outputPath)
                             : null;

      var pc = registers.Pc;
      initialPc = 0;
      finalPc = 0;

      var shouldGetPpuMode =
          BlarggTest.LOG_TRACE_ || BlarggTest.COMPARE_TO_BINJGB_;

      var lcdc = ioAddresses.Lcdc;
      var ly = ioAddresses.Ly;

      var instruction = 0;
      var cycles = 0;
      //try {
        var enumerator = expecteds?.GetEnumerator();
        for (var i = 0; i < maxCycleCount; ++i) {
          var expected = enumerator?.Current;
          enumerator?.MoveNext();

          var ppuMode = shouldGetPpuMode ? cpu.PpuMode : 0;
          initialPc = pc.Value;

          if (BlarggTest.LOG_TRACE_) {
            StringBuilder line = new StringBuilder();

            line.AppendFormat("0x{0:x4}:  ", initialPc);
            line.AppendFormat("{0:x2} |", memoryMap[initialPc]);
            line.AppendFormat(" af={0:x4}", registers.Af.Value);
            line.AppendFormat(" bc={0:x4}", registers.Bc.Value);
            line.AppendFormat(" de={0:x4}", registers.De.Value);
            line.AppendFormat(" hl={0:x4}", registers.Hl.Value);
            line.AppendFormat(" sp={0:x4}", registers.Sp.Value);
            line.AppendFormat(" pc={0:x4} |", registers.Pc.Value);

            line.AppendFormat(" tot={0} |", cycles);

            line.AppendFormat(" scl={0}", cpu.UpwardScanlineCycleCounter);
            line.AppendFormat(" st={0}", cpu.PpuModeCycleCount);
            line.AppendFormat(" cnt={0} |", cpu.ScanlineCycleCounter / 2);

            line.AppendFormat(" lcdc={0:x2}", lcdc.Value);
            line.AppendFormat(" ly={0:x2}", ly.Value);
            line.AppendFormat(" ppu={0:x1} |", ppuMode);

            line.AppendFormat(" div={0:x2}", ioAddresses.Div);
            line.AppendFormat(" tima={0:x2}", ioAddresses.Tima);
            line.AppendFormat(" tma={0:x2}", ioAddresses.Tma);
            line.AppendFormat(" tac={0:x2}", ioAddresses.Tac);
            writer.WriteLine(line.ToString());
          }

          if (expected != null &&
              (expected.Pc != initialPc ||
               expected.Cycles != cycles ||
               expected.Af != registers.Af.Value ||
               expected.Bc != registers.Bc.Value ||
               expected.De != registers.De.Value ||
               expected.Hl != registers.Hl.Value ||
               expected.Scl != cpu.UpwardScanlineCycleCounter ||
               expected.PpuMode != ppuMode)) {
            var errorBuilder = new StringBuilder();
            errorBuilder.Append("Difference at instruction: " +
                                instruction +
                                "\n");
            errorBuilder.AppendFormat("Af: {0:x4}/{1:x4}\n",
                                      expected.Af,
                                      registers.Af.Value);
            errorBuilder.AppendFormat("Bc: {0:x4}/{1:x4}\n",
                                      expected.Bc,
                                      registers.Bc.Value);
            errorBuilder.AppendFormat("De: {0:x4}/{1:x4}\n",
                                      expected.De,
                                      registers.De.Value);
            errorBuilder.AppendFormat("Hl: {0:x4}/{1:x4}\n",
                                      expected.Hl,
                                      registers.Hl.Value);
            errorBuilder.AppendFormat("Pc: {0:x4}/{1:x4}\n",
                                      expected.Pc,
                                      initialPc);
            errorBuilder.AppendFormat("Cycles: {0}/{1}\n",
                                      expected.Cycles,
                                      cycles);
            errorBuilder.AppendFormat("Scl: {0}/{1}\n",
                                      expected.Scl,
                                      cpu.UpwardScanlineCycleCounter);
            errorBuilder.AppendFormat("Ppu Mode: {0}/{1}\n",
                                      expected.PpuMode,
                                      ppuMode);
            Assert.Fail(errorBuilder.ToString());
          }

          cycles += cpu.ExecuteCycles(1);

          instruction++;
          finalPc = pc.Value;

          if (initialPc == finalPc && memory.HaltState != HaltState.HALTED) {
            break;
          }
        }
      /*} catch (Exception e) {
        output = cycles + ", " + registers.Pc.Value + ": " + output;
        throw e;
      }*/

      output = cycles + ", " + registers.Pc.Value + ": " + output;

      if (!output.Contains("Passed")) {
        Asserts.Fail(output);
      }
    }
  }
}