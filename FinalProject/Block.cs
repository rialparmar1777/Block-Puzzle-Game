using FinalProject.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject
{
    public enum BlockType
    {
        Empty = 0,
        Red,
        Pink,
        Yellow,
        Green,
        Blue
    }

    /// <summary>
    /// Class that describes information associated with a single block on the GameBoard
    /// </summary>
    class Block : IEquatable<Block>
    {
       
        private Color drawColor = Color.White;
        private bool isMatched = false;
        private bool isActive = true;
        private bool drawThisFrame = false;
        public Point position;
        private Timer blockStateTimer;
        private BlockState state;
        private BlockType type;

        private Texture2D TileTexture
        {
            get
            {
                // Determine the texture based on the block type
                switch (type)
                {
                    case BlockType.Blue:
                        return ContentHelper.GetTexture("tileBlue_27");
                    case BlockType.Green:
                        return ContentHelper.GetTexture("tileGreen_27");
                    case BlockType.Red:
                        return ContentHelper.GetTexture("tileRed_27");
                    case BlockType.Yellow:
                        return ContentHelper.GetTexture("tileYellow_27");
                    case BlockType.Pink:
                        return ContentHelper.GetTexture("tilePink_27");
                }
                return null;
            }
        }

        private Texture2D SymbolTexture
        {
            get
            {
                // Determine the symbol texture based on the block type
                switch (type)
                {
                    case BlockType.Blue:
                        return ContentHelper.GetTexture("tileBlue_31");
                    case BlockType.Green:
                        return ContentHelper.GetTexture("tileGreen_35");
                    case BlockType.Red:
                        return ContentHelper.GetTexture("tileRed_36");
                    case BlockType.Yellow:
                        return ContentHelper.GetTexture("tileYellow_33");
                    case BlockType.Pink:
                        return ContentHelper.GetTexture("tilePink_30");
                }
                return null;
            }
        }

        private enum BlockState
        {
            None,
            Blinking,
            Disappearing,
            Removed
        }

        
        public bool CanBeMatched
        {
            get
            {
                // Check conditions for a block to be matched
                if (!(isMatched) && isActive && !(type == BlockType.Empty))
                    return true;

                return false;
            }
        }

        public bool CanBeSwapped
        {
            get
            {
                // Check conditions for a block to be swapped
                if (type == BlockType.Empty)
                    return true;

                if (!(isMatched) && isActive)
                    return true;

                return false;
            }
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public BlockType Type
        {
            get { return type; }
        }

       
        public const int BlockHeight = 54;

        public const int BlockWidth = 54;


        public Block(BlockType type = BlockType.Empty)
        {
            this.type = type;

            blockStateTimer = new Timer();
            blockStateTimer.OnComplete += SetStateDisappearing;
            blockStateTimer.SetTimer(1);
        }

        public void OnMatched()
        {
            if (!(isMatched))
            {
                // Block has been matched, initiate blinking and disappearing
                isMatched = true;
                state = BlockState.Blinking;
                blockStateTimer.StartTimer();
            }
        }

        public void SetStateDisappearing(object sender, EventArgs e)
        {
            // Set block state to disappearing when the timer completes
            state = BlockState.Disappearing;
            blockStateTimer.StopTimer();
        }

        public static Block RndBlockExcEmpty()
        {
            // Generate a random block excluding the empty type
            return new Block((BlockType)RandomHelper.Next(1, 6));
        }
       
        private Point GetBlockPosition(int x, int y, int offset)
        {
            // Calculate the position of the block on the game board
            return new Point(
               GameBoard.GameBoardXAnchor + (x * BlockWidth),
               ScaleHelper.BackBufferHeight - (offset + (y * BlockHeight))
               );
        }

        public void ForcePosition(int x, int y, int offset)
        {
            // Forcefully set the position of the block
            position = GetBlockPosition(x, y, offset);
        }

        // Move the block, but enforce a maximum amount of movement (tween it)
        int maximumFrameMove = 10;
        public void UpdatePosition(int x, int y, int offset)
        {
            // Update the position of the block with a maximum movement constraint
            Point actualPosition = GetBlockPosition(x, y, offset);

            if (actualPosition == position)
                return;

            if (type == BlockType.Empty)
            {
                position = actualPosition;
                isActive = true;
                return;
            }

            int xDiff = Math.Abs(actualPosition.X - position.X);
            int yDiff = Math.Abs(actualPosition.Y - position.Y);

            bool newIsActive = true;

            if (xDiff > maximumFrameMove)
            {
                newIsActive = false;
                if (actualPosition.X > position.X)
                    position.X += maximumFrameMove;
                else
                    position.X -= maximumFrameMove;
            }
            else
            {
                position.X = actualPosition.X;

                if (yDiff > maximumFrameMove)
                {
                    newIsActive = false;
                    if (actualPosition.Y > position.Y)
                        position.Y += maximumFrameMove;
                    else
                        position.Y -= maximumFrameMove;
                }
                else
                {
                    position.Y = actualPosition.Y;
                }
            }

            if (newIsActive != isActive)
            {
                isActive = newIsActive;
            }
        }

        public void Update()
        {
            // Update the block's draw state
            HandleDrawState();
        }

        private void HandleDrawState()
        {
            // Handle the block's draw state based on its current state
            if (state == BlockState.Disappearing
               && drawColor.A == 0)
            {
                state = BlockState.Removed;
               
            }

            switch (state)
            {
                case BlockState.None:
                    drawThisFrame = true; break;
                case BlockState.Blinking:
                    drawThisFrame = true;
                    if (Time.FrameCount % 2 == 0)
                        drawThisFrame = false;
                    break;
                case BlockState.Disappearing:
                    drawThisFrame = true;
                    drawColor.A = (byte)Math.Max(0, drawColor.A - 15);
                    break;
                case BlockState.Removed:
                    type = BlockType.Empty;
                    drawThisFrame = false; break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the block if it should be drawn
            if (type == BlockType.Empty)
                return;

            if (drawThisFrame)
            {
                Point drawPosition = new Point(
                   ScaleHelper.ScaleWidth((int)position.X),
                   (int)position.Y);

                // Create a point for the "actual" scale of the tile
                Point drawScale = ScaleHelper.ScalePoint(new Point(BlockWidth, BlockHeight));

                // Draw the tile
                spriteBatch.Draw(TileTexture, new Rectangle(drawPosition, drawScale), drawColor);

                // Calculate the "actual" position of the symbol (add 1/4 of the block height)
                drawPosition.X += (int)Math.Ceiling((float)drawScale.X / 4);
                drawPosition.Y += (int)Math.Ceiling((float)drawScale.Y / 4);

                // Calculate the scale of the symbol (half of that of the block)
                drawScale.X /= 2;
                drawScale.Y /= 2;

                // Draw the symbol
                spriteBatch.Draw(SymbolTexture, new Rectangle(drawPosition, drawScale), drawColor);
            }
        }

        public bool Equals(Block other)
        {
            return position == other.position;
        }
    }
}
