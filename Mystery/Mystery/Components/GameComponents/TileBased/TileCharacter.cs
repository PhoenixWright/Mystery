using Microsoft.Xna.Framework;

namespace Mystery.Components.GameComponents.TileBased
{
    public class TileCharacter : Component
    {
        // State Management
        public Vector2 Position { get; private set; }
        public Vector2 TilePosition { get; private set; }

        // used in calculation of movement speed
        public Vector2 OldPosition { get; private set; }

        public bool Moving { get; private set; }

        /// <summary>
        /// Time in seconds it takes to move from one tile to the next for a standard move operation.
        /// </summary>
        private float MovementSpeed;
        private double MoveStartTime;

        Sprite testArt;

        public TileCharacter(Engine engine, Vector2 tilePosition)
            : base(engine)
        {
            TilePosition = tilePosition;
            Position = new Vector2(TilePosition.X * Engine.Level.TileWidth, TilePosition.Y * Engine.Level.TileHeight);
            Moving = false;

            testArt = new Sprite(Engine, @"Characters\TestSquare");
            testArt.Position = Position;

            MovementSpeed = Global.Configuration.GetFloatConfig("TileCharacterVariables", "MovementSpeed");
            MoveStartTime = 0.0f;

            Engine.AddComponent(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (Moving)
            {
                UpdateMove(gameTime);
            }

            // update artwork
            testArt.Position = Position;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            testArt.Draw(gameTime);

            base.Draw(gameTime);
        }

        // Movement Tracking Variables
        Vector2 TargetPosition;
        
        public bool Move(Global.Directions direction)
        {
            if (Moving)
            {
                return false;
            }

            Vector2 NewTilePosition = new Vector2();
            switch (direction)
            {
                case Global.Directions.Up:
                    NewTilePosition = new Vector2((int)TilePosition.X, (int)TilePosition.Y - 1);
                    break;
                case Global.Directions.Down:
                    NewTilePosition = new Vector2((int)TilePosition.X, (int)TilePosition.Y + 1);
                    break;
                case Global.Directions.Left:
                    NewTilePosition = new Vector2((int)TilePosition.X - 1, (int)TilePosition.Y);
                    break;
                case Global.Directions.Right:
                    NewTilePosition = new Vector2((int)TilePosition.X + 1, (int)TilePosition.Y);
                    break;
            }

            bool moveResult = Engine.Level.CheckMove((int)NewTilePosition.X, (int)NewTilePosition.Y);

            if (moveResult)
            {
                OldPosition = Position;
                TilePosition = NewTilePosition;
                TargetPosition = new Vector2(TilePosition.X * Engine.Level.TileWidth, TilePosition.Y * Engine.Level.TileHeight);
            }

            Moving = moveResult;
            return moveResult;
        }

        
        private void UpdateMove(GameTime gameTime)
        {
            if (MoveStartTime == 0.0f)
            {
                MoveStartTime = gameTime.TotalGameTime.TotalMilliseconds;
            }

            double ElapsedTime = gameTime.TotalGameTime.TotalMilliseconds;

            // calculate how much the player's position should have shifted given how much
            // time has passed since the beginning of the move
            float timePassed = (float)((ElapsedTime - MoveStartTime) / 1000);
            float movePercentage = timePassed / MovementSpeed;

            float pixelX = OldPosition.X + ((TargetPosition.X - OldPosition.X) * movePercentage);
            float pixelY = OldPosition.Y + ((TargetPosition.Y - OldPosition.Y) * movePercentage);

            if (movePercentage > 1)
            {
                pixelX = TargetPosition.X;
                pixelY = TargetPosition.Y;
                MoveStartTime = 0.0f;
            }

            Position = new Vector2(pixelX, pixelY);

            if (Position == TargetPosition)
            {
                Moving = false;
            }
        }
    }
}
