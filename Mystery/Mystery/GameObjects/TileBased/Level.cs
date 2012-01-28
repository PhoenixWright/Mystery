using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

using TiledLib;

using Krypton;

using Mystery;
using Mystery.Components.EngineComponents;
using Mystery.Components.GraphicsComponents;
using Mystery.Components.PhysicsComponents;
using Mystery.GameObjects;
using Mystery.GameObjects.LightObjects;
using Mystery.ScreenManagement;
using Mystery.ScreenManagement.Screens;

namespace Mystery.GameObjects.TileBased
{
    public class Level : Component
    {
        public List<EffectLight> Lights { get; private set; }

        private string mapFilePath;
        private Map map;
        private List<ParticleEffectComponent> levelParticleEffects;

        public int TileWidth { get { return map.TileWidth; } }
        public int TileHeight { get { return map.TileHeight; } }

        /// <summary>
        /// Loads and manages maps.
        /// </summary>
        /// <param name="game">Game reference.</param>
        /// <param name="mapPath">The relative path to the map.</param>
        public Level(Engine engine, string mapPath)
            : base(engine)
        {
            mapFilePath = mapPath;
            Lights = new List<EffectLight>();
            levelParticleEffects = new List<ParticleEffectComponent>();

            DrawOrder = (int)Global.Layers.Background;

            Engine.AddComponent(this);
        }

        public override void LoadContent()
        {
            map = Engine.Content.Load<Map>(mapFilePath);

            // loop through the map properties and handle them
            foreach (Property property in map.Properties)
            {
                switch (property.Name)
                {
                    case "BackgroundTrack":
                        Cue backgroundTrack = Engine.Audio.GetCue("Rain");
                        backgroundTrack.Play();
                        break;
                    case "ParticleEffect":
                        CreateParticleEffect(property.RawValue);
                        break;
                    default:
                        throw new Exception("Map property " + property.Name + " not recognized in map file " + mapFilePath);
                }
            }

            // loop through the collision objects and create physics fixtures for them
            //MapObjectLayer collisionLayer = map.GetLayer("CollisionObjects") as MapObjectLayer;
            //foreach (MapObject collisionObject in collisionLayer.Objects)
            //{
            //    // I don't fully understand why using Vector2.Zero works here. It must have something to do with the Bounds already containing the position information
            //    CreateCollisionRectangle(collisionObject.Bounds, Engine.Physics.PositionToPhysicsWorld(Vector2.Zero));

            //    // create the shadow hull for the platform
            //    CreateRectangleShadowHull(new Vector2(collisionObject.Bounds.Center.X, collisionObject.Bounds.Center.Y), collisionObject.Bounds);
            //}

            //// loop through the collision tiles and create physics fixtures for them
            //TileLayer tileLayer = map.GetLayer("CollisionTiles") as TileLayer;
            //if (Global.Configuration.GetBooleanConfig("Debug", "ShowDebugCollisionTiles"))
            //{
            //    tileLayer.Opacity = 100;
            //}
            //else
            //{
            //    tileLayer.Opacity = 0;
            //}
            //for (int y = 0; y < tileLayer.Height; ++y)
            //{
            //   for (int x = 0; x < tileLayer.Width; ++x)
            //   {
            //        if (tileLayer.Tiles[x, y] != null)
            //        {
            //            // create the physics object for the tile
            //            CreateCollisionRectangle(tileLayer.Tiles[x, y].Source, Engine.Physics.PositionToPhysicsWorld(new Vector2(x * map.TileWidth, y * map.TileHeight)));

            //            // create the shadow for the tile new Vector2(x * map.TileWidth, y * map.TileHeight)
            //            CreateRectangleShadowHull(new Vector2(x * map.TileWidth + tileLayer.Tiles[x, y].Source.Width / 2,
            //                                                  y * map.TileHeight + tileLayer.Tiles[x, y].Source.Height / 2),
            //                                      tileLayer.Tiles[x, y].Source);
            //        }
            //    }
            //}

            //// loop through the light objects and create lights in the game world for them
            //MapObjectLayer lightLayer = map.GetLayer("LightObjects") as MapObjectLayer;
            //foreach (MapObject lightObject in lightLayer.Objects)
            //{
            //    CreateLight(lightObject);
            //}
        }

        public override void Update(GameTime gameTime)
        {
            foreach (ParticleEffectComponent particleEffectComponent in levelParticleEffects)
            {
                particleEffectComponent.Position = new Vector2(Engine.Camera.Position.X, 0.0f);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This draw function comes from TiledLib, and is re-implemented to add parallax scrolling.
        /// The scrolling is based player position and a scroll value in a layer's properties if IsParallax is true.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            Rectangle worldArea = Engine.Camera.VisibleArea;

            int minX;
            int maxX;

            int minY;
            int maxY;

            Engine.SpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Engine.Camera.CameraMatrix);

            foreach (Layer layer in map.Layers)
            {
                if (layer.Name.Contains("Object"))
                    continue;

                TileLayer tileLayer = layer as TileLayer;

                if (!tileLayer.Visible)
                    continue;

                // figure out the min and max tile indices to draw
                minX = Math.Max((int)Math.Floor((float)worldArea.Left / map.TileWidth), 0);
                maxX = Math.Min((int)Math.Ceiling((float)worldArea.Right / map.TileWidth), map.Width);

                minY = Math.Max((int)Math.Floor((float)worldArea.Top / map.TileHeight), 0);
                maxY = Math.Min((int)Math.Ceiling((float)worldArea.Bottom / map.TileHeight), map.Height);

                for (int x = minX; x < maxX; x++)
                {
                    for (int y = minY; y < maxY; y++)
                    {
                        Tile tile = tileLayer.Tiles[x, y];

                        if (tile == null)
                            continue;

                        Rectangle r = new Rectangle(x * map.TileWidth, y * map.TileHeight - tile.Source.Height + map.TileHeight, tile.Source.Width, tile.Source.Height);

                        tile.DrawOrthographic(Engine.SpriteBatch, r, tileLayer.Opacity, tileLayer.LayerDepth);
                    }
                }
            }

            Engine.SpriteBatch.End();
        }

        public bool CheckMove(int x, int y)
        {
            bool valid = false;

            // check the boundary tiles
            TileLayer boundaryLayer = map.GetLayer("Boundaries") as TileLayer;
            if ((x > 0 && x < boundaryLayer.Tiles.Width) && (y > 0 && y < boundaryLayer.Tiles.Height))
            {
                valid = boundaryLayer.Tiles[x, y] == null;
            }

            // check npcs...

            return valid;
        }

        public Vector2 GetTilePosition(int x, int y)
        {
            TileLayer backgroundLayer = map.GetLayer("Background") as TileLayer;
            Rectangle tileRectangle = backgroundLayer.Tiles[x, y].Source;
            return new Vector2(tileRectangle.Left, tileRectangle.Top);
        }

        #region CreateFunctions
        public void CreateCollisionRectangle(Rectangle rectangle, Vector2 position)
        {
            Vertices vertices = new Vertices();
            vertices.Add(Engine.Physics.PositionToPhysicsWorld(new Vector2(rectangle.Left, rectangle.Top)));
            vertices.Add(Engine.Physics.PositionToPhysicsWorld(new Vector2(rectangle.Right, rectangle.Top)));
            vertices.Add(Engine.Physics.PositionToPhysicsWorld(new Vector2(rectangle.Right, rectangle.Bottom)));
            vertices.Add(Engine.Physics.PositionToPhysicsWorld(new Vector2(rectangle.Left, rectangle.Bottom)));

            Fixture fixture = FixtureFactory.CreatePolygon(Engine.Physics.World, vertices, 1.0f);
            fixture.Body.Position = position;
            fixture.Body.BodyType = BodyType.Static;
            fixture.Restitution = 0.0f;
            fixture.CollisionFilter.CollisionCategories = (Category)Global.CollisionCategories.Structure;
        }

        public void CreateLight(MapObject lightObject)
        {
            Vector2 position = new Vector2(lightObject.Bounds.Center.X, lightObject.Bounds.Center.Y);

            Property lightAngle;
            if (lightObject.Properties.TryGetValue("LightAngle", out lightAngle) == false)
            {
                lightAngle = new Property("LightAngle", "3.14");
            }
            float lightAngleValue = float.Parse(lightAngle.RawValue);

            Property lightColorString;
            if (lightObject.Properties.TryGetValue("LightColor", out lightColorString) == false)
            {
                throw new Exception("Failed to retrieve light color from " + lightObject.Name + " in map " + mapFilePath);
            }
            // use reflection to get a Color from a string
            PropertyInfo colorProperty = typeof(Color).GetProperty(lightColorString.RawValue);
            Color lightColor = (Color)colorProperty.GetValue(null, null);

            Property lightFov;
            if (lightObject.Properties.TryGetValue("LightFov", out lightFov) == false)
            {
                lightFov = new Property("LightFov", "6.28");
            }
            float lightFovValue = float.Parse(lightFov.RawValue);

            Property lightMotion;
            if (lightObject.Properties.TryGetValue("LightMotion", out lightMotion) == false)
            {
                throw new Exception("Failed to retrieve light motion from " + lightObject.Name + " in map " + mapFilePath);
            }

            Property lightRange;
            if (lightObject.Properties.TryGetValue("LightRange", out lightRange) == false)
            {
                throw new Exception("Failed to retrieve light range from " + lightObject.Name + " in map " + mapFilePath);
            }
            float lightRangeValue = float.Parse(lightRange.RawValue);

            Property lightType;
            if (lightObject.Properties.TryGetValue("LightType", out lightType) == false)
            {
                throw new Exception("Failed to retrieve light type from " + lightObject.Name + " in map " + mapFilePath);
            }

            EffectLight effectLight;

            switch (lightType.RawValue)
            {
                case "Gravity":
                    // get gravity value
                    Property gravityValueProperty;
                    if (lightObject.Properties.TryGetValue("GravityValue", out gravityValueProperty) == false)
                    {
                        throw new Exception("Failed to retrieve gravity value from " + lightObject.Name + " in map " + mapFilePath);
                    }
                    float gravityValue = float.Parse(gravityValueProperty.RawValue);

                    effectLight = new GravityLight(Engine, gravityValue);
                    break;

                case "Null":
                    effectLight = new NullLight(Engine, Lights);
                    break;

                case "Velocity":
                    // get velocity value
                    Property velocityValueProperty;
                    if (lightObject.Properties.TryGetValue("VelocityValue", out velocityValueProperty) == false)
                    {
                        throw new Exception("Failed to retrieve velocity value from " + lightObject.Name + " in map " + mapFilePath);
                    }
                    float velocityValue = float.Parse(velocityValueProperty.RawValue);

                    effectLight = new VelocityLight(Engine, velocityValue);
                    break;

                default:
                    throw new Exception("Failed to instantiate light with type of " + lightObject.Name + " in map " + mapFilePath);
            }

            effectLight.Angle = lightAngleValue;
            effectLight.Color = lightColor;
            effectLight.Fov = lightFovValue;
            effectLight.Intensity = 1.0f;
            effectLight.Position = position;
            effectLight.Range = lightRangeValue;
            effectLight.Activate( lightMotion.RawValue);

            Lights.Add(effectLight);
        }

        public void CreateParticleEffect(string particleEffectName)
        {
            levelParticleEffects.Add(new ParticleEffectComponent(Engine, particleEffectName, new Vector2(Engine.Video.GraphicsDevice.Viewport.Width, 0)));
        }

        public void CreatePlatform(Vector2 position, Rectangle rectangle, bool castsShadow)
        {
            // create the shadow hull for the platform
            CreateRectangleShadowHull(position, rectangle);
        }

        public void CreateRectangleShadowHull(Vector2 position, Rectangle rectangle)
        {
            ShadowHull shadowHull = ShadowHull.CreateRectangle(new Vector2(rectangle.Width, rectangle.Height));
            shadowHull.Position = position;

            Engine.Lighting.Krypton.Hulls.Add(shadowHull);
        }
        #endregion
    }
}