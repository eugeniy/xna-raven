using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Raven
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Camera m_camera;
        Statistics m_stats;
        Skydome m_dome;
        Terrain m_terrain;
        Console m_console;

        KeyboardState previousKeyboard;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferMultiSampling = true;
            graphics.PreferredBackBufferWidth = 960;
            graphics.PreferredBackBufferHeight = 640;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            m_camera = new Camera(this);
            Components.Add(m_camera);

            m_stats = new Statistics(this, Content);
            Components.Add(m_stats);

            // Load the generated skydome
            m_dome = new Skydome(this, Content);
            Components.Add(m_dome);

            m_terrain = new Terrain(this, Content);
            Components.Add(m_terrain);

            m_console = new Console(this);
            Components.Add(m_console);

            base.Initialize();
        }


        /// <summary>
        /// Handle the event when game gains focus.
        /// </summary>
        /// <param name="sender">The Game.</param>
        /// <param name="args">Arguments for the Activated event.</param>
        protected override void OnActivated(object sender, EventArgs args)
        {
            base.OnActivated(sender, args);

            // Reenable camera if it is not active
            m_camera.Enabled = true;
        }


        /// <summary>
        /// Handle the event when game loses focus.
        /// </summary>
        /// <param name="sender">The Game.</param>
        /// <param name="args">Arguments for the Deactivated event.</param>
        protected override void OnDeactivated(object sender, EventArgs args)
        {
            base.OnDeactivated(sender, args);

            // Disable the camera, it's important since it steals the mouse focus
            m_camera.Enabled = false;
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
                this.Exit();

            // Enter fullscreen
            else if (keyboard.IsKeyDown(Keys.RightAlt) && keyboard.IsKeyDown(Keys.Enter))
                graphics.ToggleFullScreen();

            // Toggle the game console display
            else if (keyboard.IsKeyDown(Keys.OemTilde) && !previousKeyboard.IsKeyDown(Keys.OemTilde))
            {
                if (m_console.Enabled)
                {
                    m_console.Enabled = false;
                    m_camera.Enabled = true;
                }
                else
                {
                    m_camera.Enabled = false;
                    m_console.Enabled = true;
                }
            }

            // Display wire frames and don't cull when tab is pressed
            else if (keyboard.IsKeyDown(Keys.Tab))
            {
                var rasterizer = new RasterizerState();
                rasterizer.FillMode = FillMode.WireFrame;
                rasterizer.CullMode = CullMode.None;
                GraphicsDevice.RasterizerState = rasterizer;
            }

            previousKeyboard = keyboard;


            // Display some debug information
            m_stats["Position"] = String.Format("({0:0.###}, {1:0.###}, {2:0.###})", m_camera.Position.X, m_camera.Position.Y, m_camera.Position.Z);
            m_stats["Yaw"] = String.Format("{0:0.###}", MathHelper.ToDegrees(m_camera.Yaw));
            m_stats["Pitch"] = String.Format("{0:0.###}", MathHelper.ToDegrees(m_camera.Pitch));

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SlateGray);

            m_dome.Draw(gameTime, m_camera);

            m_terrain.Draw(gameTime, m_camera);

            base.Draw(gameTime);
        }
    }
}
