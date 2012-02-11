using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Mystery.Components.GameComponents.TextBased;

namespace Mystery.ScreenManagement.Screens
{
    public class ConversationScreen : GameScreen
    {
        float pauseAlpha;

        ContentManager content;

        Engine Engine;
        Conversation Conversation;

        /// <summary>
        /// Constructor. // TODO: add appropriate constructor to display the proper conversation
        /// </summary>
        public ConversationScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            Engine = new Engine(content, ScreenManager);

            List<Dialogue> dialogue = new List<Dialogue>();
            dialogue.Add(new Dialogue("It actually fucking works a little bit. Thank you for the email.  Our 6 Month Unlimited Package is usually priced at $600 but i would be willing to decrease that to $550 for you since you will only be in town for half the year.  Let me know if this is something that you are still interested in. Have a great week and I hope to see you at the studio soon!"));
            Conversation = new Conversation(Engine, dialogue);

            base.LoadContent();
        }

        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                Engine.Update(gameTime);

                if (Conversation.IsDone)
                {
                    ExitScreen();
                }
            }
            else
            {
                Engine.Pause();
            }
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
        }

        /// <summary>
        /// Draws the conversation screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            Engine.Draw(gameTime);

            // If the game is transitioning on or off, fade it out to black.
            // If the game is transitioning on or off, fade it out to black.
            if (pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }
    }
}
