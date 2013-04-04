using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mystery.Components.EngineComponents
{
  public class Camera3D : Component
  {
    public Vector3 Position { get; set; }
    public Vector3 Target { get; set; }
    public Matrix ViewMatrix { get; set; }
    public Matrix ProjectionMatrix { get; set; }

    public Camera3D(Engine engine)
      : base(engine)
    {
      ResetCamera();

      Engine.AddComponent(this);
    }

    public override void Update(GameTime gameTime)
    {
      UpdateViewMatrix();

      base.Update(gameTime);
    }

    private void UpdateViewMatrix()
    {
      ViewMatrix = Matrix.CreateLookAt(Position, Target, Vector3.Up);
    }

    public void ResetCamera()
    {
      Position = Vector3.Zero;
      Target = Vector3.Zero;
      ViewMatrix = Matrix.Identity;
      ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), Engine.Video.GraphicsDevice.Viewport.AspectRatio, 0.5f, 500f);
    }
  }
}
