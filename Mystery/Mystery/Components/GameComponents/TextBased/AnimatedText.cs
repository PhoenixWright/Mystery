using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mystery.Components.GameComponents.TextBased
{
  public class AnimatedText : Component
  {
    public Color Color { get; set; }
    public float CPS { get; set; }
    public SpriteFont Font { get; set; }
    public bool IsDone { get; private set; }
    public int MaxLineWidth { get; set; }
    public Vector2 Position { get; set; }
    public string Text { get; set; }
    public int Width { get; set; }

    // state management
    private double startTime;
    private string currentString;
    bool spedUp;

    public AnimatedText(Engine engine, Vector2 position, int width, string text)
      : base(engine)
    {
      Color = Color.White;
      CPS = Global.Configuration.GetFloatConfig("TextBasedVariables", "CharactersPerSecond");
      DrawOrder = (int)Global.Layers.Text;
      Font = Engine.Content.Load<SpriteFont>(@"Fonts\DialogueFont");
      IsDone = false;
      MaxLineWidth = 0;
      Position = position;
      Text = text;

      startTime = 0.0;
      currentString = "";
      spedUp = false;

      if(Font.MeasureString(Text).Length() > width) {
        string line = string.Empty;
        Text = string.Empty;
        string[] words = text.Split(' ');

        foreach(string word in words) {
          if(Font.MeasureString(line + word).Length() > width) {
            Text = Text + line + '\n';
            line = string.Empty;
          }

          line = line + word + ' ';
        }
      }

      Engine.AddComponent(this);
    }

    public override void Update(GameTime gameTime)
    {
      if(IsDone) {
        return;
      }

      if(startTime == 0.0) {
        startTime = gameTime.TotalGameTime.TotalMilliseconds;
      }

      double currentTime = gameTime.TotalGameTime.TotalMilliseconds;

      // calculate how many characters should be displayed
      double secondsPassed = (currentTime - startTime) / 1000;

      int charactersToDisplay = (int)(secondsPassed * CPS);
      if(charactersToDisplay >= Text.Length) {
        IsDone = true;
        currentString = Text;
      }
      else {
        currentString = Text.Substring(0, charactersToDisplay);
      }

      if(Engine.Input.IsButtonDown(Global.Configuration.GetButtonConfig("GameControls", "TalkButton")) || Engine.Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "TalkKey"))) {
        if(!spedUp) {
          spedUp = true;
          CPS += 50;
        }
      }

      base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
      Engine.SpriteBatch.Begin(SpriteSortMode.Immediate, null);

      if(MaxLineWidth == 0) {
        Engine.SpriteBatch.DrawString(Font, currentString, Position, Color);
      }
      else {
        // TODO: wrap text
      }

      Engine.SpriteBatch.End();

      base.Draw(gameTime);
    }
  }
}
