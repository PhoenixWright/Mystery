using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mystery.Components.GameComponents
{
  public class Sprite : Component
  {
    private Texture2D sprite;
    private Vector2 spriteOrigin;

    public float Angle { get; set; }
    public Vector2 Position { get; set; }
    public SpriteEffects SpriteEffect { get; set; }

    public Sprite(Engine engine, string spriteFilepath)
      : base(engine)
    {
      sprite = engine.Content.Load<Texture2D>(spriteFilepath);
      spriteOrigin = new Vector2(0, 0);

      Angle = 0.0f;
      Position = new Vector2();
      SpriteEffect = SpriteEffects.None;
      Visible = true;
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Draw(GameTime gameTime)
    {
      if(Visible) {
        Engine.SpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Engine.Camera.CameraMatrix);
        Engine.SpriteBatch.Draw(sprite, Position, sprite.Bounds, Color.White, Angle, spriteOrigin, 1.0f, SpriteEffect, 1.0f);
        Engine.SpriteBatch.End();
      }
    }

    public override void Dispose(bool disposing)
    {
      sprite = null;

      base.Dispose(disposing);
    }
  }
}
