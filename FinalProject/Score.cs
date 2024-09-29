using FinalProject.GameEffect;
using FinalProject.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject
{
    class Score
    {
        // Private fields
        private int score;
        private int combo;
        private string scoreString;  // The formatted score string with commas
        private string comboString; // The combo string with "x" at the end
        private static SpriteFont scoreFont;
        private static Game1 gameInstance;
        

        // Constructor that receives an instance of Game1
        public Score(Game1 game)
        {
            gameInstance = game;
        }

       
        // Properties
        public int CurrentScore
        {
            get { return score; }
        }

        // Score display variables
        private Vector2 scorePosition;
        private Rectangle scoreRectangle;

        // Combo display variables
        private Vector2 comboPosition;
        private Rectangle comboRectangle;

        // Constants
        private const int MaxScore = 999999999;
        private const int ScoreBoxWidth = 350;
        private const int ScoreBoxHeight = 50;
        private const int ComboBoxWidth = 150;
        private const int ComboBoxHeight = 50;

        private bool surpassedScoreChecked;
        // Constructor
        public Score()
        {
            score = 0;
            combo = 1;// Combo is always 1x to start with
            scoreFont = ContentHelper.GetFont("KenVectorFutureThin");

            scoreString = GetScoreString();
            comboString = GetComboString();
            // Initialize draw variables
            InitializeDrawVariables(); 
           
            surpassedScoreChecked = true;
        }
        // Initialize draw variables
        private void InitializeDrawVariables()
        {
            // Score box position and size
            scorePosition = new Vector2(
               ScaleHelper.BackBufferWidth - (ScaleHelper.BackBufferWidth / 3),
               ScaleHelper.BackBufferHeight / 15);

            scoreRectangle = new Rectangle(
               (int)scorePosition.X,
               (int)scorePosition.Y,
               ScaleHelper.ScaleWidth(ScoreBoxWidth),
               ScaleHelper.ScaleHeight(ScoreBoxHeight));

            // Combo box position and size
            scorePosition.X = scoreRectangle.Right - ScaleHelper.ScaleWidth((int)scoreFont.MeasureString(scoreString).X);

            comboPosition = new Vector2(
               ScaleHelper.BackBufferWidth - (int)(ScaleHelper.BackBufferWidth / 5.6),
               ScaleHelper.BackBufferHeight / 5);

            comboRectangle = new Rectangle(
               (int)comboPosition.X,
               (int)comboPosition.Y,
               ScaleHelper.ScaleWidth(ComboBoxWidth),
               ScaleHelper.ScaleHeight(ComboBoxHeight));

            comboPosition.X = comboRectangle.Right - ScaleHelper.ScaleWidth((int)scoreFont.MeasureString(comboString).X);
        }

        // Add to the score
        public void AddScore()
        {
            score += RandomHelper.Next(100, 999) * combo;
            score = Math.Min(MaxScore, score); // Make sure score doesn't overflow
            scoreString = GetScoreString();
            scorePosition.X = scoreRectangle.Right - ScaleHelper.ScaleWidth((int)scoreFont.MeasureString(scoreString).X);

            if (surpassedScoreChecked)
            {
                UseNotificationEffect();
            }
        }
        
        // Reset the combo
        public void ResetCombo()
        {
            combo = 1;
            comboString = GetComboString();
            comboPosition.X = comboRectangle.Right - ScaleHelper.ScaleWidth((int)scoreFont.MeasureString(comboString).X);

           
        }

        // Main draw method
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the score container box
            spriteBatch.Draw(ContentHelper.GetTexture("score_panel"), scoreRectangle, Color.White);

            // Draw the score text
            spriteBatch.DrawString(
               scoreFont,
               scoreString,
               scorePosition,
               Color.Black,
               0f,
               Vector2.Zero,
               ScaleHelper.ScreenScaleVector,
               SpriteEffects.None,
               0f);

            // Draw the combo container box
            spriteBatch.Draw(ContentHelper.GetTexture("score_panel"), comboRectangle, Color.White);

            // Draw the combo text
            spriteBatch.DrawString(
               scoreFont,
               comboString,
               comboPosition,
               Color.Black,
               0f,
               Vector2.Zero,
               ScaleHelper.ScreenScaleVector,
               SpriteEffects.None,
               0f);
        }

        // Update score string position based on the current score
        private void UpdateScoreStringPosition()
        {
            scoreString = GetScoreString();
            scorePosition.X = scoreRectangle.Right - ScaleHelper.ScaleWidth((int)scoreFont.MeasureString(scoreString).X);
        }

        // Update combo string position based on the current combo
        private void UpdateComboStringPosition()
        {
            comboString = GetComboString();
            comboPosition.X = comboRectangle.Right - ScaleHelper.ScaleWidth((int)scoreFont.MeasureString(comboString).X);
        }

        // Adds commas every three numbers (i.e. 1234567 -> 1,234,567)
        public string GetScoreString()
        {
            string scoreString = score.ToString();
            if (scoreString.Length > 3)
            {
                StringBuilder returnString = new StringBuilder();
                for (var i = 0; i < scoreString.Length; i++)
                {
                    returnString.Append(scoreString[i]);
                    // Insert commas at the appropriate positions
                    if ((i + 1) == scoreString.Length - 3)
                        returnString.Append(",");
                    else if ((i + 1) == scoreString.Length - 6)
                        returnString.Append(",");
                }
                return returnString.ToString();
            }
            return scoreString.ToString();
        }

        // Add "x" to the end to the combo
        public string GetComboString()
        {
            StringBuilder returnString = new StringBuilder();
            returnString.Append(combo.ToString());
            returnString.Append("x");

            return returnString.ToString();
        }

        public void UseNotificationEffect()
        {
            // Example: Call a method from ScoreNotificationEffect
            surpassedScoreChecked=gameInstance.CheckSurpassedScore(score);
        }
    }
}
