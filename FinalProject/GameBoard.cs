using FinalProject.Interfaces;
using FinalProject.Utilities.Containers;
using FinalProject.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FinalProject.GameEffect;

namespace FinalProject
{

    // Class representing the game board
    class GameBoard : IScene
    {
        // Private fields

        private Game1 parent;
        private Board<Block> gameBoard;
        private Timer rowMovingTimer;
        private Timer rowPauseTimer;
        private bool rowPaused = false;
        private Timer comboResetTimer;
        private float rowMovingTimerSetValue = 0.05f;
        private Player player;
        private Score score;
        public const int GameBoardXAnchor = 306;
        public const int maxLevel = 80;
        public int YOffset = 0;
      
        // Constructor
        public GameBoard(Game1 parent)
        {
            // Parent game1 of this gameboard
            this.parent = parent;

            // Init player
            player = new Player(this);

            // Init the list of rows 
            gameBoard = new Board<Block>(6);

            // Init the row adding timer
            rowMovingTimer = new Timer(true); 
            rowMovingTimer.OnComplete += MoveRowsUp;
            rowMovingTimer.SetTimer(rowMovingTimerSetValue);
            rowMovingTimer.StartTimer();

            // Init the row pausing timer
            rowPauseTimer = new Timer(true); 
            rowPauseTimer.OnComplete += UnpauseGameBoard;
            rowPauseTimer.SetTimer(2f);
            rowPauseTimer.StartTimer();

            // Init the combo resetting timer
            comboResetTimer = new Timer(true); 
            comboResetTimer.OnComplete += ResetCombo;
            comboResetTimer.SetTimer(0.5f);
            comboResetTimer.StartTimer();

            // Add the first row
            AddRow();

            // Init the scoreboard
            score = new Score();
        }

 // Method for handling button press
        private void OnButtonPressed(Buttons button)
        {
            if (button == Buttons.Pause)
            {
                parent.AddScene(new PauseMenu(parent));
            }

            if (button == Buttons.Escape)
            {
                parent.RemoveScene();
            }
        }

        // Main update method
        public void Update()
        {
            // Check if any row has reached the top
            bool isAnyRowTouchingTop = false;
            for (int i = 0; i < gameBoard.Rows; i++)
            {
                bool isRowComplete = true;
                for (int j = 0; j < gameBoard.Columns; j++)
                {
                    if (gameBoard[i][j].position.Y > 0)
                    {
                        isRowComplete = false;
                        break;
                    }
                }

                if (isRowComplete)
                {
                    isAnyRowTouchingTop = true;
                    break; 
                }
            }

            // Stop the game if any complete row has touched the top
            if (isAnyRowTouchingTop)
            {
                int currentScore = score.CurrentScore;
                ScoreManager.SaveScore(currentScore);
                StopGame();
            }

            // Add a new row when a full row is exposed at the bottom
            if (YOffset >= ScaleHelper.ScaleHeight(Block.BlockHeight))
            {
                AddRow(); YOffset = 0; // Reset the Y offset
            }

            // Update the player
            player.Update();

            // Update all blocks on the gameboard
            for (var i = 0; i < gameBoard.Rows; i++)
            {
                for (var j = 0; j < gameBoard.Columns; j++)
                {
                    var thisBlock = gameBoard[i, j];
                    if (!(thisBlock.Type == BlockType.Empty))
                    {
                        thisBlock.UpdatePosition(j, i, YOffset);
                        thisBlock.Update();
                        if (thisBlock.Type == BlockType.Empty)
                            gameBoard.RemoveItem(i, j);
                    }
                }
            }

            // Apply gravity to the gameboard
            gameBoard.CompressBoardDownwards();
            gameBoard.RemoveEmptyRows();

            // Check for matches this frame
            DoMatchChecking();
        }

        // Stop the game
        private void StopGame()
        {
            parent.StopGame();
        }

        // Main draw method
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the background
            Rectangle drawTo = new Rectangle(Point.Zero, new Point(ScaleHelper.BackBufferWidth, ScaleHelper.BackBufferHeight));
            spriteBatch.Draw(ContentHelper.GetTexture("gameBoardBackground"), drawTo, Color.White);

            // Draw each block on the GameBoard
            for (var i = 0; i < gameBoard.Rows; i++)
            {
                for (var j = 0; j < gameBoard.Columns; j++)
                {
                    gameBoard[i, j].Draw(spriteBatch);
                }
            }

            // Draw the player
            player.Draw(spriteBatch);

            // Draw the score
            score.Draw(spriteBatch);

            // Draw the background overlay
            spriteBatch.Draw(ContentHelper.GetTexture("gameBoardOverlayCheat"), drawTo, Color.White);
        }

        // Called when the scene is disabled
        public void OnSceneDisabled()
        {
            // Unsubscribe to button input when we are not the active scene
            InputHelper.ButtonPressed -= OnButtonPressed;
        }

        // Called when the scene is enabled
        public void OnSceneEnabled()
        {
            InputHelper.ButtonPressed += OnButtonPressed;
        }

        // Add a random row to the GameBoard
        public void AddRow()
        {
            // Flag the bottom row as active, if present
            if (gameBoard.Rows > 0)
            {
                for (var i = 0; i < gameBoard.Columns; i++)
                    gameBoard[0][i].IsActive = true;
            }

            // Get a new random row
            Block[] randomRow = GetRandomRow();

            gameBoard.AddAbove(randomRow);

            // Update the player when a row is added
            player.OnRowAdded();
        }

        // Generate a random row
        public Block[] GetRandomRow()
        {
            Block[] returnArr = new Block[6];
            for (var i = 0; i <= 5; i++)
            {
                var nextBlock = Block.RndBlockExcEmpty();
                returnArr[i] = nextBlock;
                nextBlock.ForcePosition(i, 0, YOffset);
            }

            return returnArr;
        }

        // Move all rows up a position
        public void MoveRowsUp(object sender, EventArgs e)
        {
            if (!(rowPaused))
                YOffset += 1;
        }

        // Unpause the gameboard
        public void UnpauseGameBoard(object sender, EventArgs e)
        {
            if (BoardIsStale())
                rowPaused = false;
            else
                rowPauseTimer.ResetTimer();
        }

        // Reset the combo
        public void ResetCombo(object sender, EventArgs e)
        {
            if (BoardIsStale())
                score.ResetCombo();
        }

        // Swap values on a specified row at specified position + position+1 (player drawn from top left corner)
        public void SwapAt(int x, int y)
        {
            if (!(y >= gameBoard.Rows))
            {
                if (gameBoard[y][x].CanBeSwapped && gameBoard[y][x + 1].CanBeSwapped)
                    gameBoard.SwapItemOnRow(y, x, x + 1);
            }
        }

        // Check if there are any "active" blocks (matched/moving) to determine if the game board is stale
        private bool BoardIsStale()
        {
            bool isStale = true;
            for (var i = 0; i < gameBoard.Rows && isStale; i++)
            {
                for (var j = 0; j < gameBoard.Columns && isStale; j++)
                {
                    if (!(gameBoard[i][j].CanBeSwapped))
                        isStale = false;
                }
            }
            return isStale;
        }


        // Method to check for matches in the rows and columns (not to be confused w/ "CheckMatches")
        private void DoMatchChecking()
        {
            var allMatchedBlocks = new List<Block>();
            // Check for matches in the rows
            for (var i = 1; i < gameBoard.Rows; i++)
            {
                if (gameBoard.IsRowDirty(i))
                {
                    Block[] matchedBlocks = CheckMatches(gameBoard.GetRow(i));
                    if (matchedBlocks.Length > 0)
                    {
                        foreach (Block block in matchedBlocks)
                        {
                            if (!(allMatchedBlocks.Contains(block)))
                                allMatchedBlocks.Add(block);
                        }
                    }
                }
            }

            // Check for matches in the columns
            for (var i = 0; i < gameBoard.Columns; i++)
            {
                if (gameBoard.IsColumnDirty(i))
                {
                    Block[] matchedBlocks = CheckMatches(gameBoard.GetColumn(i));
                    if (matchedBlocks.Length > 0)
                    {
                        foreach (Block block in matchedBlocks)
                        {
                            if (!(allMatchedBlocks.Contains(block)))
                                allMatchedBlocks.Add(block);
                        }
                    }
                }
            }
            gameBoard.FlagAllClean();

            // Tag all the blocks that have been matched
            if (allMatchedBlocks.Count > 0)
            {
                rowPaused = true;
                rowPauseTimer.ResetTimer();
                foreach (Block block in allMatchedBlocks)
                {
                    block.OnMatched();
                    score.AddScore();
                }
            }
        }

        // Takes a list of ordered blocks and returns a list of any matches of length 3 or greater
        private Block[] CheckMatches(Block[] blocks)
        {
            int pos = 1;
            bool prevSame = false; 
            List<Block> matchedBlocks = new List<Block>();

            // Check from the second position in the array, up to the second to last position
            while (pos <= blocks.Length - 2)
            {
               
                if (blocks[pos].CanBeMatched
                   && blocks[pos + 1].CanBeMatched
                   && blocks[pos].Type == blocks[pos + 1].Type)
                {
                   
                    if (prevSame)
                    {
                        pos += 1;
                        matchedBlocks.Add(blocks[pos]);
                    }
                   
                    else if (blocks[pos - 1].CanBeMatched
                       && blocks[pos - 1].Type == blocks[pos].Type)
                    {
                        for (var i = pos - 1; i <= pos + 1; i++)
                            matchedBlocks.Add(blocks[i]);
                        prevSame = true;
                        pos += 1;
                    }
                   
                    else if ((pos + 2) <= blocks.Length - 1
                       && blocks[pos + 2].CanBeMatched
                       && blocks[pos + 2].Type == blocks[pos].Type)
                    {
                        for (var i = pos; i <= pos + 2; i++)
                            matchedBlocks.Add(blocks[i]);
                        prevSame = true;
                        pos += 2;
                    }
                    else
                    {
                        pos += 3;
                    }
                }
                else
                {
                    prevSame = false;
                    pos += 2;
                }
            }
            return matchedBlocks.ToArray();
        }
    }
}
