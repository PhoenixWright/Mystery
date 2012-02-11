using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Mystery.Components.GameComponents.TextBased
{
    public class Conversation : Component
    {
        int dialogueIndex;
        List<Dialogue> dialogue;

        private AnimatedText currentText;
        private DialogBox dialogBox;
        public bool IsDone { get; private set; }
        public bool WaitingForInput { get; private set; }

        // state management
        double waitStartTime;

        public Conversation(Engine engine, List<Dialogue> conversation)
            : base(engine)
        {
            dialogue = conversation;
            dialogueIndex = 0;

            dialogBox = new DialogBox(engine, true);
            IsDone = false;

            waitStartTime = 0.0;

            Engine.AddComponent(this);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (dialogBox.DialogState == DialogBox.State.Open)
            {
                if (currentText == null)
                {
                    if (dialogueIndex == dialogue.Count)
                    {
                        dialogBox.Close();
                    }
                    else
                    {
                        currentText = new AnimatedText(Engine, new Vector2(dialogBox.Size.Left + 5, dialogBox.Size.Top + 5), dialogBox.Size.Width - 10, dialogue[dialogueIndex].Text);
                        ++dialogueIndex;
                    }
                }
                else
                {
                    if (currentText.IsDone)
                    {
                        WaitingForInput = true;
                    }
                }
            }
            else if (dialogBox.DialogState == DialogBox.State.Closed)
            {
                IsDone = true;
            }

            if (WaitingForInput)
            {
                // TODO: display an animation at the bottom right corner of the dialog box
                // signaling that the user should press something

                // set a timer to make sure we don't go through this process too fast
                if (waitStartTime == 0.0)
                {
                    waitStartTime = gameTime.TotalGameTime.TotalMilliseconds;
                }
                else
                {
                    double currentSeconds = (gameTime.TotalGameTime.TotalMilliseconds - waitStartTime) / 1000;

                    if (currentSeconds > Global.Configuration.GetFloatConfig("TextBasedVariables", "AfterTextFinishedDisplayTime") && (Engine.Input.IsButtonDown(Global.Configuration.GetButtonConfig("GameControls", "TalkButton")) || Engine.Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "TalkKey"))))
                    {
                        WaitingForInput = false;
                        Engine.RemoveComponent(currentText);
                        currentText = null;
                        waitStartTime = 0.0;
                    }
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
