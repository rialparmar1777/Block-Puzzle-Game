using FinalProject.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.GameEffect
{
    public class GameOverEffect
    {
        private bool isGameOver;
        private float gameOverDuration = 50.0f;
        private float gameOverTimer;
        private bool shakeActive;
        private float shakeTimer;
        private float shakeIntensity;
        private Vector2 shakeOffset;

        private Texture2D gameOverImage;

        public GameOverEffect()
        {
            isGameOver = false;
            gameOverTimer = 0.0f;

            shakeActive = false;
            shakeTimer = 0.0f;
            shakeIntensity = 5.0f;
            shakeOffset = Vector2.Zero;

            // Load the game over image
            ////gameOverImage = ContentHelper.GetTexture("gOver"); 
        }

        public void StartGameOverEffect()
        {
            isGameOver = true;
            gameOverTimer = 0.0f;
            shakeActive = true;
            shakeTimer = 0.0f;
        }

        public void Update()
        {
            if (isGameOver)
            {
                gameOverTimer += Time.DeltaTime;

                if (gameOverTimer >= gameOverDuration)
                {
                    isGameOver = false;
                }

                if (shakeActive)
                {
                    UpdateShakeEffect();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            if (isGameOver)
            {
                int screenWidth = graphicsDevice.Viewport.Width;
                int screenHeight = graphicsDevice.Viewport.Height;

                // Draw the game over image covering the entire screen
                spriteBatch.Draw(
                    ContentHelper.GetTexture("gOver"), 
                    new Rectangle(0, 0, screenWidth, screenHeight),
                    Color.White
                );
            }
        }

        private void UpdateShakeEffect()
        {
            shakeTimer += Time.DeltaTime;

            if (shakeTimer <= 50.0f)
            {
                shakeOffset = new Vector2(
                    (float)Math.Sin(shakeTimer * 50) * shakeIntensity,
                    (float)Math.Cos(shakeTimer * 50) * shakeIntensity
                );
            }
            else
            {
                shakeActive = false;
                shakeOffset = Vector2.Zero;
            }
        }
    }

}
