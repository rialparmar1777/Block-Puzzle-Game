using FinalProject.GameEffect;
using FinalProject.Interfaces;
using FinalProject.ParticleEffects;
using FinalProject.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Drawing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace FinalProject
{
    /// <summary>
    /// Main game class representing the entry point for the game.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch;
        private GameOverEffect gameOverEffect;
        private NotificationEffect notificationEffect;
        private Stack<IScene> scenes;// Stack to manage different game scenes
        private Score score;

        /// <summary>
        /// Constructor for the Game1 class.
        /// </summary>
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }


        /// <summary>
        /// Initialize method called once when the game is launched.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

            // Update buffer values for scaling
            ScaleHelper.UpdateBufferValues(graphics.PreferredBackBufferHeight, graphics.PreferredBackBufferWidth);

            score = new Score(this);

            IsMouseVisible = true;
           
            base.Initialize();
        }


        /// <summary>
        /// LoadContent method called to load game assets.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(GraphicsDevice);

            // Initialize the game over effect
            gameOverEffect = new GameOverEffect();

            score = new Score(this);

            // Load textures for tiles, symbols, player, backgrounds, and UI elements
            LoadTextures();

            // Load fonts for the score
            LoadFonts();

            notificationEffect = new NotificationEffect(ContentHelper.GetFont("OverFont"));
            // Create the gameboard scene and add it to the scene stack
            scenes = new Stack<IScene>();
            AddScene(new GameBoard(this));

        }

        private void LoadFonts()
        {
            // Add font for score
            ContentHelper.AddFont("KenVectorFutureThin", Content.Load<SpriteFont>("Fonts\\KenVectorFutureThin"));
            ContentHelper.AddFont("KenneySpace", Content.Load<SpriteFont>("Fonts\\KenneySpace"));
            ContentHelper.AddFont("OverFont", Content.Load<SpriteFont>("Fonts\\OverFont"));
        }

        private void LoadTextures()
        {
            // Add tile textures
            ContentHelper.AddTexture("tileBlue_27", Content.Load<Texture2D>("Graphics\\tileBlue_27"));
            ContentHelper.AddTexture("tileGreen_27", Content.Load<Texture2D>("Graphics\\tileGreen_27"));
            ContentHelper.AddTexture("tilePink_27", Content.Load<Texture2D>("Graphics\\tilePink_27"));
            ContentHelper.AddTexture("tileRed_27", Content.Load<Texture2D>("Graphics\\tileRed_27"));
            ContentHelper.AddTexture("tileYellow_27", Content.Load<Texture2D>("Graphics\\tileYellow_27"));

            // Add symbol textures
            ContentHelper.AddTexture("tileBlue_31", Content.Load<Texture2D>("Graphics\\tileBlue_31"));
            ContentHelper.AddTexture("tileGreen_35", Content.Load<Texture2D>("Graphics\\tileGreen_35"));
            ContentHelper.AddTexture("tilePink_30", Content.Load<Texture2D>("Graphics\\tilePink_30"));
            ContentHelper.AddTexture("tileRed_36", Content.Load<Texture2D>("Graphics\\tileRed_36"));
            ContentHelper.AddTexture("tileYellow_33", Content.Load<Texture2D>("Graphics\\tileYellow_33"));

            // Add player texture
            ContentHelper.AddTexture("player", Content.Load<Texture2D>("Graphics\\player"));

            // Add background textures for the GameBoard
            ContentHelper.AddTexture("gameBoardBackground", Content.Load<Texture2D>("Background\\gameBoardBackground"));
            ContentHelper.AddTexture("gameBoardOverlayCheat", Content.Load<Texture2D>("Background\\gameBoardOverlayCheat"));
            ContentHelper.AddTexture("gOver", Content.Load<Texture2D>("Background\\gOver"));


            // Add UI textures
            ContentHelper.AddTexture("grey_panel", Content.Load<Texture2D>("UI\\grey_panel"));
            ContentHelper.AddTexture("score_panel", Content.Load<Texture2D>("UI\\score_panel"));
            ContentHelper.AddTexture("yellow_button04", Content.Load<Texture2D>("UI\\yellow_button04"));
            ContentHelper.AddTexture("yellow_button05", Content.Load<Texture2D>("UI\\yellow_button05"));

        }


        /// <summary>
        /// Update method called once per frame to update game logic.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {

            // Update time and input
            Time.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            InputHelper.Update();

            // Update any running timers
            Timer.Update();

            // Update the active scene
            UpdateActiveScene();


            // Update particle effects
            ParticleEmitter.Update();

            // Update the game over effect
            gameOverEffect.Update();

            notificationEffect.Update();

          

            base.Update(gameTime);
        }
        private void UpdateActiveScene()
        {
            if (scenes.Count > 0)
                scenes.Peek().Update();
            else
                Exit(); // Exit the game if there are no scenes
        }


        /// <summary>
        /// Draw method called once per frame to render game graphics.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
            spriteBatch.Begin();

            // Draw the active scene
            scenes.Peek().Draw(spriteBatch);

            // Draw particle effects
            ParticleEmitter.Draw(spriteBatch);

            // Draw the game over effect
            gameOverEffect.Draw(spriteBatch,GraphicsDevice);

            notificationEffect.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Add a new scene to the scene stack.
        /// </summary>
        public void AddScene(IScene scene)
        {
            // Disable the current scene if it exists
            if (scenes.Count > 0)
                scenes.Peek().OnSceneDisabled();

            // Push the new scene and enable it
            scenes.Push(scene);
            scenes.Peek().OnSceneEnabled();
        }

        /// <summary>
        /// Remove the current scene from the scene stack.
        /// </summary>
        public void RemoveScene()
        {
            // Disable the current scene and pop it from the stack
            scenes.Peek().OnSceneDisabled();
            scenes.Pop();

            // If there is a scene below this one, enabled it
            if (scenes.Count > 0)
                scenes.Peek().OnSceneEnabled();
        }

       
        /// <summary>
        /// Trigger the game over effect to stop the game.
        /// </summary>
        public void StopGame()
        {
            gameOverEffect.StartGameOverEffect();
        }

        public bool CheckSurpassedScore(int currentScore)
        {
            int previousScore = ScoreManager.LoadScore();

            if (currentScore > previousScore)
            {
                ScoreManager.SaveScore(currentScore);
               notificationEffect.ShowNotification(currentScore);
                return false;
            }
            return true;
        }
    }
}