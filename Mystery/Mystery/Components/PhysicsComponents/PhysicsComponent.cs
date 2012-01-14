﻿using System.Collections.Generic;

using Microsoft.Xna.Framework;

using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;

using Mystery.Components.EngineComponents;
using Mystery.ScreenManagement;
using Mystery.ScreenManagement.Screens;

namespace Mystery.Components.PhysicsComponents
{
    public class PhysicsComponent : Component
    {
        public Fixture MainFixture { get; protected set; }
        public float Angle { get { return MainFixture.Body.Rotation; } }
        public List<Body> Bodies { get; private set; }
        public Vector2 Position
        {
            get
            {
                return Engine.Physics.PositionToGameWorld(MainFixture.Body.Position);
            }
        }

        public PhysicsComponent(Engine engine) : base(engine)
        {
            Bodies = new List<Body>();

            Engine.AddComponent(this);
        }

        public override void Dispose(bool disposing)
        {
            foreach (Body body in Bodies)
            {
                Engine.Physics.World.RemoveBody(body);
            }

            base.Dispose(disposing);
        }
    }
}
