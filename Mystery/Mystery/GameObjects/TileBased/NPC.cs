using Microsoft.Xna.Framework;

using Mystery.Components.GameComponents.TileBased;

namespace Mystery.GameObjects.TileBased
{
    public class NPC : TileCharacter
    {
        public NPC(Engine engine, Vector2 tilePosition)
            : base(engine, tilePosition)
        {
            DrawOrder = (int)Global.Layers.NPCs;

            Engine.AddComponent(this);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
