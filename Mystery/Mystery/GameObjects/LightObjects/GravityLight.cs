using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using Mystery;
using Mystery.Components.EngineComponents;
using Mystery.Components.GraphicsComponents;
using Mystery.ScreenManagement;
using Mystery.ScreenManagement.Screens;

namespace Mystery.GameObjects.LightObjects
{
  public class GravityLight : EffectLight
  {
    private float gravityValue { get; set; }
    private Vector2 gravityVector { get; set; }

    public GravityLight(Engine engine, float gravityValue)
      : base(engine)
    {
      this.gravityValue = gravityValue;
      gravityVector = new Vector2(0.0f, gravityValue);
    }

    public override void Update(GameTime gameTime)
    {
      if(EffectActive) {
        foreach(Fixture fixture in AffectedFixtures) {
          fixture.Body.ApplyForce(gravityVector);
        }
      }

      base.Update(gameTime);
    }
  }
}
