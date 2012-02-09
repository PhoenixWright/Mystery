using Microsoft.Xna.Framework;

namespace Mystery.Components.GameComponents.TextBased
{
    public class Conversation : Component
    {
        private AnimatedText currentText;
        private DialogBox dialogBox;
        public bool IsDone { get; private set; }

        public Conversation(Engine engine)
            : base(engine)
        {
            dialogBox = new DialogBox(engine, true);
            IsDone = false;

            Engine.AddComponent(this);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (dialogBox.DialogState == DialogBox.State.Open)
            {
                if (currentText == null)
                {
                    currentText = new AnimatedText(Engine, new Vector2(dialogBox.Size.Left + 5, dialogBox.Size.Top + 5), dialogBox.Size.Width - 10, "This is some test text.");
                }
                else
                {
                    if (currentText.IsDone)
                    {
                        Engine.RemoveComponent(currentText);
                        dialogBox.Close();
                    }
                }
            }
            else if (dialogBox.DialogState == DialogBox.State.Closed)
            {
                IsDone = true;
            }

            base.Update(gameTime);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
