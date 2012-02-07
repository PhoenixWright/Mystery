using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using Mystery;
using Mystery.Components.EngineComponents;
using Mystery.Components.GameComponents;
using Mystery.Components.GameComponents.TileBased;
using Mystery.Components.GraphicsComponents;
using Mystery.Components.PhysicsComponents;
using Mystery.GameComponents;
using Mystery.GameObjects.LightObjects;
using Mystery.ScreenManagement;
using Mystery.ScreenManagement.Screens;

namespace Mystery.GameObjects.TileBased
{
    public class Player : TileCharacter
    {
        // components
        public Light light { get; private set; }

        // audio listener
        public AudioListener AudioListener { get; private set; }

        public Player(Engine engine, Vector2 tilePosition)
            : base(engine, tilePosition)
        {
            DrawOrder = (int)Global.Layers.Player;

            AudioListener = new AudioListener();
            AudioListener.Position = new Vector3(Position, 0);

            light = new Light(engine);
            light.Color = Color.White;
            light.Fov = MathHelper.TwoPi;
            light.Position = Position;
            light.Range = 250;
            light.ShadowType = Krypton.Lights.ShadowType.Illuminated;

            Engine.AddComponent(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Moving && (Engine.Input.IsButtonDown(Global.Configuration.GetButtonConfig("GameControls", "TalkButton")) || Engine.Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "TalkKey"))))
            {
                // check if there is an NPC in the direction that the player is currently facing
                Vector2 tilePlayerIsFacing = Vector2.Zero;
                switch (Direction)
                {
                    case Global.Directions.Up:
                        tilePlayerIsFacing = new Vector2(TilePosition.X, TilePosition.Y - 1);
                        break;

                    case Global.Directions.Down:
                        tilePlayerIsFacing = new Vector2(TilePosition.X, TilePosition.Y + 1);
                        break;

                    case Global.Directions.Left:
                        tilePlayerIsFacing = new Vector2(TilePosition.X - 1, TilePosition.Y);
                        break;

                    case Global.Directions.Right:
                        tilePlayerIsFacing = new Vector2(TilePosition.X + 1, TilePosition.Y);
                        break;

                    default:
                        break;
                }

                foreach (NPC npc in Engine.NPCs)
                {
                    if (npc.TilePosition == tilePlayerIsFacing)
                    {
                        Engine.ScreenManager.AddScreen(new ConversationScreen(), PlayerIndex.One);
                    }
                }
            }

            if (Engine.Input.IsButtonDown(Global.Configuration.GetButtonConfig("GameControls", "UpButton")) || Engine.Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "UpKey")))
            {
                Move(Global.Directions.Up);
            }

            if (Engine.Input.IsButtonDown(Global.Configuration.GetButtonConfig("GameControls", "DownButton")) || Engine.Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "DownKey")))
            {
                Move(Global.Directions.Down);
            }

            if (!Moving && Engine.Input.IsButtonDown(Global.Configuration.GetButtonConfig("GameControls", "LeftButton")) || Engine.Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "LeftKey")))
            {
                Move(Global.Directions.Left);
            }

            if (!Moving && Engine.Input.IsButtonDown(Global.Configuration.GetButtonConfig("GameControls", "RightButton")) || Engine.Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "RightKey")))
            {
                Move(Global.Directions.Right);
            }

            light.Position = new Vector2(Position.X + Engine.Level.TileWidth / 2, Position.Y + Engine.Level.TileHeight / 2);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Engine.SpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Engine.Camera.CameraMatrix);
            Engine.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
