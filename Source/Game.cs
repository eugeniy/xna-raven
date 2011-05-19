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

        Effect effect;

        Camera m_camera;
        Statistics m_stats;
        Skydome m_dome;
        Console m_console;

        VertexPositionColor[] vertices;

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
            m_camera = new Camera2D(this);
            Components.Add(m_camera);

            m_stats = new Statistics(this, Content);
            Components.Add(m_stats);

            // Load the generated skydome
            m_dome = new Skydome(this, Content);
            Components.Add(m_dome);

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

            // TODO: use this.Content to load your game content here

            // Load my basic shader!
            effect = Content.Load<Effect>(@"Shaders\Simple");

            

            vertices = new VertexPositionColor[3];

            vertices[0].Position = new Vector3(-0.5f, -0.5f, 0f);
            vertices[0].Color = Color.Red;
            vertices[1].Position = new Vector3(0, 0.5f, 0f);
            vertices[1].Color = Color.Green;
            vertices[2].Position = new Vector3(0.5f, -0.5f, 0f);
            vertices[2].Color = Color.Yellow;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            effect.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            // Enter fullscreen
            else if (Keyboard.GetState().IsKeyDown(Keys.RightAlt) && Keyboard.GetState().IsKeyDown(Keys.Enter))
                graphics.ToggleFullScreen();

            // Display wire frames and don't cull when tab is pressed
            else if (Keyboard.GetState().IsKeyDown(Keys.Tab))
            {
                var rasterizer = new RasterizerState();
                rasterizer.FillMode = FillMode.WireFrame;
                rasterizer.CullMode = CullMode.None;
                GraphicsDevice.RasterizerState = rasterizer;
            }


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
            GraphicsDevice.Clear(Color.DarkSlateGray);



            m_dome.Draw(gameTime, m_camera);

            effect.CurrentTechnique = effect.Techniques["Simple"];
            effect.Parameters["World"].SetValue(Matrix.Identity);
            effect.Parameters["View"].SetValue(m_camera.View);
            effect.Parameters["Projection"].SetValue(m_camera.Projection);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 1, VertexPositionColor.VertexDeclaration);
            }


            base.Draw(gameTime);
        }
    }
}
