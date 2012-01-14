using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Krypton;
using Krypton.Lights;

using Mystery.Components.EngineComponents;
using Mystery.Components.GraphicsComponents;
using Mystery.GameObjects;
using Mystery.ScreenManagement;
using Mystery.ScreenManagement.Screens;

namespace Mystery
{
    public class Engine
    {
        List<Component> components;
        List<Component> componentsToBeAdded;
        List<Component> componentsToBeRemoved;

        public ContentManager Content { get; private set; }

        public Audio Audio { get; private set; }
        public Camera2D Camera { get; private set; }
        public InputState Input { get; private set; }
        public Level Level { get; set; }
        public Lighting Lighting { get; private set; }
        public Physics Physics { get; private set; }
        public Player Player { get; set; }
        public Video Video { get; private set; }

        public SpriteBatch SpriteBatch { get; private set; }

        public bool Updating { get; private set; }

        // components
        public BloomComponent BloomComponent;

        public ScreenManager ScreenManager;

        public Engine(ContentManager content, ScreenManager screenManager)
        {
            this.ScreenManager = screenManager;

            components = new List<Component>();
            componentsToBeAdded = new List<Component>();
            componentsToBeRemoved = new List<Component>();

            Content = content;

            Audio = new Audio(this);
            Input = new InputState();
            Physics = new Physics(this);
            Video = new Video(this);

            // these things require video
            Camera = new Camera2D(this);

            // lighting needs to know the camera matrix
            Lighting = new Lighting(this);

            SpriteBatch = new SpriteBatch(Global.GraphicsDeviceManager.GraphicsDevice);

            BloomComponent = new BloomComponent(this);
            BloomComponent.DrawOrder = int.MaxValue;
            BloomComponent.LoadContent();

            Updating = false;
        }

        public void LoadContent(Game game)
        {
            foreach (Component c in components)
                c.LoadContent();
        }

        public void UnloadContent()
        {
            foreach (Component c in components)
                c.UnloadContent();
        }

        public void Pause()
        {
            Audio.Pause();
        }

        public void Update(GameTime gameTime)
        {
            Input.Update(gameTime);
            Physics.Update(gameTime);

            if (Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "CameraUpKey")))
            {
                Camera.Position += new Vector2(0.0f, -10.0f);
            }

            if (Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "CameraDownKey")))
            {
                Camera.Position += new Vector2(0.0f, 10.0f);
            }

            if (Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "CameraLeftKey")))
            {
                Camera.Position += new Vector2(-10.0f, 0.0f);
            }

            if (Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "CameraRightKey")))
            {
                Camera.Position += new Vector2(10.0f, 0.0f);
            }

            if (Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "ZoomInKey")))
            {
                Camera.Zoom += 0.01f;
            }

            if (Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "ZoomOutKey")))
            {
                Camera.Zoom -= 0.01f;
            }

            // flag that we're updating
            Updating = true;
            foreach (Component c in components)
            {
                if (c.Disposed)
                {
                    RemoveComponent(c);
                }

                c.Update(gameTime);
            }
            // remove updating flag
            Updating = false;

            // handle new components generated during update
            foreach (Component c in componentsToBeAdded)
            {
                AddComponent(c);
            }
            componentsToBeAdded.Clear();

            // handle components removed during update
            foreach (Component c in componentsToBeRemoved)
            {
                RemoveComponent(c);
            }
            componentsToBeRemoved.Clear();
        }

        public void Draw(GameTime gameTime)
        {
            Lighting.Krypton.LightMapPrepare();

            BloomComponent.BeginDraw();

            Video.GraphicsDevice.Clear(Color.CornflowerBlue);

            // this is placeholder code for testing stuff
            SpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Camera.CameraMatrix);
            SpriteBatch.End();

            foreach (Component c in components)
            {
                c.Draw(gameTime);
            }

            BloomComponent.Draw(gameTime);

            DebugDraw(gameTime);
        }

        public void DebugDraw(GameTime gameTime)
        {
            if (Global.Configuration.GetBooleanConfig("Debug", "ShowDebugView"))
            {
                Lighting.DebugDraw();

                // need this to allow the farseer debug view transparency to work
                Video.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                Physics.Draw(gameTime);
            }
        }

        public void AddComponent(Component Component)
        {
            if (Updating)
            {
                componentsToBeAdded.Add(Component);
                return;
            }

            if (!components.Contains(Component))
            {
                components.Add(Component);
                Component.Initialize();
                Component.LoadContent();
                PutComponentInOrder(Component);
            }
        }

        // The components are stored in their draw order, so it is easy to loop 
        // through them and draw them in the correct order without having to sort
        // them every time they are drawn
        public void PutComponentInOrder(Component component)
        {
            if (components.Contains(component))
            {
                components.Remove(component);

                int i = 0;

                // Iterate through the components in order until we find one with
                // a higher or equal draw order, and insert the component at that
                // position.
                for (i = 0; i < components.Count; i++)
                    if (components[i].DrawOrder >= component.DrawOrder)
                        break;

                components.Insert(i, component);
            }
        }

        public void RemoveComponent(Component Component)
        {
            if (Component != null && components.Contains(Component))
            {
                if (Updating)
                {
                    componentsToBeRemoved.Add(Component);
                }
                else
                {
                    components.Remove(Component);
                }
            }
        }
    }
}