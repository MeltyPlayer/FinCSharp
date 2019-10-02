﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fin.graphics.common {
  public struct Color {
    public uint I { get; }

    private Color(uint i) {
      this.I = i;
    }

    public static Color FromRgba(uint i) => new Color(i);

    public static Color FromRgbaB(byte r, byte g, byte b, byte a) {
      var rr = (uint) ((r & 0xFF) << 24);
      var gg = (uint) ((g & 0xFF) << 16);
      var bb = (uint) ((b & 0xFF) << 8);
      var aa = (uint) (a & 0xFF);
      return Color.FromRgba(rr + gg + bb + aa);
    }

    public static Color FromRgbB(byte r, byte g, byte b)
      => Color.FromRgbaB(r, g, b, 255);

    public static Color FromRgbaF(float r, float g, float b, float a) {
      return Color.FromRgbaB((byte) (r * 255),
        (byte) (g * 255),
        (byte) (b * 255),
        (byte) (a * 255));
    }

    public static Color FromRgbF(float r, float g, float b) =>
      Color.FromRgbaF(r, g, b, 1);

    public byte Rb => (byte) (this.I >> 24 & 0xff);
    public byte Gb => (byte) (this.I >> 16 & 0xff);
    public byte Bb => (byte) (this.I >> 8 & 0xff);
    public byte Ab => (byte) (this.I & 0xff);

    public float Rf => this.Rb / 255f;
    public float Gf => this.Gb / 255f;
    public float Bf => this.Bb / 255f;
    public float Af => this.Ab / 255f;
  }
}