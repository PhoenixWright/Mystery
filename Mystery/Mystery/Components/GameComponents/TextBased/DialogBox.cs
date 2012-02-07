using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mystery.Components.GameComponents.TextBased
{
    public class DialogBox : Component
    {
        private bool Animated;

        // text
        AnimatedText animatedText;

        // Size
        public bool IsDone { get; private set; }
        public Rectangle Size { get; set; }
        private Rectangle CurrentSize;
        private double animationSeconds;
        private double startTime;

        // Colors
        public Color BackgroundColor { get; set; }
        public Color OutlineColor { get; set; }

        // Internal Textures
        private Texture2D BackgroundTexture;
        private Texture2D OutlineTexture;

        public DialogBox(Engine engine, bool animated)
            : base(engine)
        {
            DrawOrder = (int)Global.Layers.TextBackground;
            Animated = animated;
            animatedText = null;

            // default size of the rectangle is a 4:3 formatted box at the bottom of the screen
            int width = engine.Video.GraphicsDevice.Viewport.Width / 3 * 2;
            int x = (engine.Video.GraphicsDevice.Viewport.Width - width) / 2;

            int y = engine.Video.GraphicsDevice.Viewport.Bounds.Bottom - engine.Video.GraphicsDevice.Viewport.Height / 3;
            int height = engine.Video.GraphicsDevice.Viewport.Bounds.Bottom - y;

            IsDone = !Animated;

            if (IsDone)
            {
                CurrentSize = Size;
            }
            else
            {
                CurrentSize = new Rectangle(x + width / 2, y + height / 2, 1, 1);
            }
            Size = new Rectangle(x, y, width, height);
            startTime = 0.0;
            animationSeconds = Global.Configuration.GetFloatConfig("TextBasedVariables", "DialogExpansionSeconds");

            BackgroundColor = Color.Gray;
            OutlineColor = Color.White;

            BackgroundTexture = new Texture2D(engine.Video.GraphicsDevice, 1, 1);
            BackgroundTexture.SetData(new Color[] { BackgroundColor });
            OutlineTexture = new Texture2D(engine.Video.GraphicsDevice, 1, 1);
            OutlineTexture.SetData(new Color[] { OutlineColor });

            Engine.AddComponent(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (Animated && !IsDone)
            {
                if (startTime == 0.0)
                {
                    startTime = gameTime.TotalGameTime.TotalMilliseconds;
                }

                double currentTime = gameTime.TotalGameTime.TotalMilliseconds;
                double progressPercentage = ((currentTime - startTime) / 1000) / animationSeconds;
                if (progressPercentage > 1)
                {
                    CurrentSize = Size;
                    IsDone = true;
                }
                else
                {
                    // calculate current size
                    CurrentSize.X = (int)(Size.X + Size.Width / 2 - (Size.Width / 2 * progressPercentage));
                    CurrentSize.Y = (int)(Size.Y + Size.Height / 2 - (Size.Height / 2 * progressPercentage));
                    CurrentSize.Width = (int)(Size.Width * progressPercentage);
                    CurrentSize.Height = (int)(Size.Height * progressPercentage);
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Engine.SpriteBatch.Begin(SpriteSortMode.Immediate, null);
            Engine.SpriteBatch.Draw(BackgroundTexture, CurrentSize, BackgroundColor);
            Engine.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
