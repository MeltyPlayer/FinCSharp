using System;

using fin.events;

namespace fin.app.node {

  [AttributeUsage(AttributeTargets.Method)]
  public class OnTickAttribute : Attribute {

    public OnTickAttribute() {
    }
  }
}