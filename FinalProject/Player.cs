﻿using FinalProject.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject
{
    class Player
    {
        // Private fields
        private static Texture2D playerTexture;

        // The gameboard that this player belongs to
        private GameBoard parent;

        // Defer swapping blocks until the next frame
        private bool swapOnNextFrame = false;

        // Not exact screen positions - "grid" based positions around the gameboard (Y = 1 - 12, X = 0 - 4)
        private Point position;

        // Constructor
        public Player(GameBoard parent)
        {
            this.parent = parent;
            playerTexture = ContentHelper.GetTexture("player");
            InputHelper.ButtonPressed += OnButtonPressed; // Subscribe to Button Presses
            InputHelper.MouseMoved += OnMouseMoved; // Subscribe to Mouse Moves
            position = new Point(2, 2); // Start somewhere low in the middle
        }

        // Event handler for when a button is pressed in InputHandler
        private void OnButtonPressed(Buttons buttonPressed)
        {
            switch (buttonPressed)
            {
                case Buttons.Left:
                    if (position.X > 0)
                        position.X -= 1;
                    break;
                case Buttons.Right:
                    if (position.X < 4)
                        position.X += 1;
                    break;
                case Buttons.Down:
                    if (position.Y > 1)
                        position.Y -= 1;
                    break;
                case Buttons.Up:
                    if (position.Y < 12)
                        position.Y += 1;
                    break;
                case Buttons.Swap:
                    swapOnNextFrame = true;
                    break;
                case Buttons.AddRow:
                    if (position.Y - 1 >= 1)
                        position.Y -= 1;
                    parent.AddRow();
                    break;
            }
        }

        // This needs polishing
        private void OnMouseMoved(Point mousePosition)
        {
            mousePosition.X -= ScaleHelper.ScaleWidth(GameBoard.GameBoardXAnchor);
            mousePosition.Y -= ScaleHelper.ScaleHeight(Block.BlockHeight);

            position.X = mousePosition.X / ScaleHelper.ScaleWidth(Block.BlockWidth);
            position.Y = 12 - (mousePosition.Y / ScaleHelper.ScaleHeight(Block.BlockHeight));

            // Make sure X & Y are within bounds
            position.X = Math.Min(position.X, 4);
            position.X = Math.Max(position.X, 0);

            position.Y = Math.Min(position.Y, 12);
            position.Y = Math.Max(position.Y, 1);
        }

        // Use this to keep the player in line as more rows get added
        public void OnRowAdded()
        {
            if (!(InputHelper.MouseEnabled))
            {
                if (position.Y + 1 <= 12)
                    position.Y += 1;
            }
        }

        // Main update method
        public void Update()
        {
            if (swapOnNextFrame) // Used to defer the swapping of blocks until the next update loop
            {
                parent.SwapAt(position.X, position.Y);
                swapOnNextFrame = false;
            }
        }

        // Main draw method
        public void Draw(SpriteBatch spriteBatch)
        {
            // Calculate the drawing position and scale
            Point drawPos = new Point();
            drawPos.X = (ScaleHelper.ScaleWidth(GameBoard.GameBoardXAnchor) + (ScaleHelper.ScaleWidth(Block.BlockWidth) * position.X));
            drawPos.Y = (ScaleHelper.BackBufferHeight - parent.YOffset - (ScaleHelper.ScaleHeight(Block.BlockHeight) * position.Y));

            Point drawScale = new Point();
            drawScale.X = ScaleHelper.ScaleWidth(playerTexture.Width) / 2;
            drawScale.Y = ScaleHelper.ScaleHeight(playerTexture.Height) / 2;

            // Draw the player texture
            spriteBatch.Draw(playerTexture, new Rectangle(drawPos, drawScale), Color.Black);
        }
    }
}
