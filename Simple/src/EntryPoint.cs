﻿namespace simple {
  using fin.app.impl.opentk;

  public class EntryPoint {
    public static void Main() {
      var app = new AppOpenTk();
      //app.Launch(new TestScene());
      app.Launch(new AudioScene());
    }
  }
}