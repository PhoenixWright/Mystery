using System.Collections.Generic;

using Microsoft.Xna.Framework;

using FarseerPhysics.Dynamics;

using Mystery;
using Mystery.Components.PhysicsComponents;
using Mystery.ScreenManagement;

namespace Mystery.GameComponents.PhysicsComponents
{
  public class MovingPlatformPhysicsComponent : PhysicsComponent
  {
    public List<Vector2> Positions { get; private set; }

    public MovingPlatformPhysicsComponent(Engine engine, List<Vector2> gameWorldPositionList, float speed)
      : base(engine)
    {
    }

    public override void Update(GameTime gameTime)
    {
    }
  }
}
