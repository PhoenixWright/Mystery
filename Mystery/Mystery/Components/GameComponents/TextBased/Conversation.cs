﻿using Microsoft.Xna.Framework;

namespace Mystery.Components.GameComponents.TextBased
{
    public class Conversation : Component
    {
        private AnimatedText currentText;
        private DialogBox dialogBox;

        public Conversation(Engine engine)
            : base(engine)
        {
            dialogBox = new DialogBox(engine, true);

            Engine.AddComponent(this);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (dialogBox.IsDone)
            {
                if (currentText == null)
                {
                    currentText = new AnimatedText(Engine, new Vector2(dialogBox.Size.Left + 5, dialogBox.Size.Top + 5), dialogBox.Size.Width - 10, "This is some test text.");
                }
                else
                {
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}