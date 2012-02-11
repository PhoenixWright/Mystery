using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mystery.Components.GameComponents.TextBased
{
    public class DialogBox : Component
    {
        public enum State
        {
            Opening,
            Open,
            Closing,
            Closed
        };

        private bool Animated;

        public State DialogState { get; private set; }

        // text
        AnimatedText animatedText;

        // Size
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

            if (!animated)
            {
                CurrentSize = Size;
                DialogState = State.Open;
            }
            else
            {
                CurrentSize = new Rectangle(x + width / 2, y + height / 2, 1, 1);
                DialogState = State.Opening;
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
            if (Animated && (DialogState == State.Opening || DialogState == State.Closing))
            {
                if (startTime == 0.0)
                {
                    startTime = gameTime.TotalGameTime.TotalMilliseconds;
                }

                double currentTime = gameTime.TotalGameTime.TotalMilliseconds;
                double progressPercentage = ((currentTime - startTime) / 1000) / animationSeconds;
                if (progressPercentage > 1)
                {
                    if (DialogState == State.Opening)
                    {
                        DialogState = State.Open;
                        CurrentSize = Size;
                    }
                    else
                    {
                        DialogState = State.Closed;
                        CurrentSize = Rectangle.Empty;
                    }
                }
                else
                {
                    // calculate current size
                    if (DialogState == State.Opening)
                    {
                        CurrentSize.X = (int)(Size.X + Size.Width / 2 - (Size.Width / 2 * progressPercentage));
                        CurrentSize.Y = (int)(Size.Y + Size.Height / 2 - (Size.Height / 2 * progressPercentage));
                        CurrentSize.Width = (int)(Size.Width * progressPercentage);
                        CurrentSize.Height = (int)(Size.Height * progressPercentage);
                    }
                    else if (DialogState == State.Closing)
                    {
                        CurrentSize.X = (int)(Size.X + (Size.Width / 2 * progressPercentage));
                        CurrentSize.Y = (int)(Size.Y + (Size.Height / 2 * progressPercentage));
                        CurrentSize.Width = (int)(Size.Width * (1 - progressPercentage));
                        CurrentSize.Height = (int)(Size.Height * (1 - progressPercentage));
                    }
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

        public void Close()
        {
            DialogState = State.Closing;
            startTime = 0.0f;
        }
    }
}
