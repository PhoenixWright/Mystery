using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mystery.Components.GameComponents.TextBased
{
  public class Dialogue
  {
    public string Text { get; private set; }

    public Dialogue(string text)
    {
      Text = text;
    }
  }
}
