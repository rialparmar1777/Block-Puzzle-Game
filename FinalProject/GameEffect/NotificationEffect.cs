using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.GameEffect
{
    public class NotificationEffect
    {
        private bool isActive;
        private float timer;
        private int surpassedScore;

        private Vector2 position;
        private float scale;
        private SpriteFont font;

        public NotificationEffect(SpriteFont font)
        {
            this.font = font;
            isActive = false;
            timer = 0f;
            position = new Vector2(100, 720); // Initial position at the bottom of the screen
            scale = 1f; // Initial scale
        }

        public void ShowNotification(int surpassedScore)
        {
            this.surpassedScore = surpassedScore;
            isActive = true;
            timer = 0f;
        }

        public void Update()
        {
            if (isActive)
            {
                timer += Time.DeltaTime;

                float yOffset = MathHelper.Lerp(720, 100, timer / 5f); 
                position = new Vector2(position.X, yOffset);

                scale = MathHelper.Lerp(1f, 2f, timer / 2f); 

                if (timer >= 5f)
                {
                    isActive = false;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isActive)
            {
                Color color = Color.Yellow;

                float desiredFontSize = 90f; 

                float adjustedScale = desiredFontSize / font.MeasureString("A").Y;
                spriteBatch.DrawString(
                    font,
                    $"You surpassed the previous score!\nNew Score: {surpassedScore}",
                    position,
                    color,
                    0f, 
                    Vector2.Zero, 
                    adjustedScale, 
                    SpriteEffects.None,
                    0f 
                );

               
            }
        }
    }
}
