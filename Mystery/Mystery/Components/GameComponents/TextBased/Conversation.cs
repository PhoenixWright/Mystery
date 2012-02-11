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
                    currentText = new AnimatedText(Engine, new Vector2(dialogBox.Size.Left + 5, dialogBox.Size.Top + 5), dialogBox.Size.Width - 10, "It actually fucking works a little bit. Thank you for the email.  Our 6 Month Unlimited Package is usually priced at $600 but i would be willing to decrease that to $550 for you since you will only be in town for half the year.  Let me know if this is something that you are still interested in. Have a great week and I hope to see you at the studio soon!");
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
