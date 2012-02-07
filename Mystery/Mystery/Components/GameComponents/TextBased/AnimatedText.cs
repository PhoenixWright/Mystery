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

            Engine.AddComponent(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (IsDone)
            {
                return;
            }

            if (startTime == 0.0)
            {
                startTime = gameTime.TotalGameTime.TotalMilliseconds;
            }

            double currentTime = gameTime.TotalGameTime.TotalMilliseconds;

            // calculate how many characters should be displayed
            double secondsPassed = (currentTime - startTime) / 1000;

            int charactersToDisplay = (int)(secondsPassed * CPS);
            if (charactersToDisplay >= Text.Length)
            {
                IsDone = true;
                currentString = Text;
            }
            else
            {
                currentString = Text.Substring(0, charactersToDisplay);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Engine.SpriteBatch.Begin(SpriteSortMode.Immediate, null);

            if (MaxLineWidth == 0)
            {
                Engine.SpriteBatch.DrawString(Font, currentString, Position, Color);
            }
            else
            {
                // TODO: wrap text
            }

            Engine.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
