using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Scripting.Hosting;
using IronPython.Hosting;

namespace Raven
{
    /// <summary>
    /// A command line interface, which allows to modify game state using a Python language.
    /// </summary>
    public class Console : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected ScriptEngine m_engine;
        protected ScriptRuntime m_runtime;
        protected ScriptScope m_scope;

        protected SpriteBatch m_spriteBatch;
        protected Texture2D m_overlay;
        protected Rectangle m_destination;

        public Console(Game game) : base(game)
        {
            m_destination = new Rectangle(0, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height / 3);
            Enabled = false;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            m_engine = Python.CreateEngine();
            m_runtime = m_engine.Runtime;
            m_scope = m_engine.CreateScope();
            m_scope.SetVariable("__name__", "__main__");

            base.Initialize();
        }

        /// <summary>
        /// Load the content used by the component.
        /// </summary>
        protected override void LoadContent()
        {
            m_spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create a black, semi-transparrent 1x1 texture.
            m_overlay = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            m_overlay.SetData<Color>(new Color[] { new Color(0, 0, 0, 0.5f) });

            base.LoadContent();
        }

        /// <summary>
        /// Called when the DrawableGameComponent needs to be drawn.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {
            if (Enabled)
            {
                m_spriteBatch.Begin();
                m_spriteBatch.Draw(m_overlay, m_destination, Color.Black);
                m_spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// Execute the given source code.
        /// </summary>
        /// <returns>True if executed successfully, false otherwise.</returns>
        public bool Execute(string code)
        {
            try
            {
                m_engine.Execute(code, m_scope);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Access the scope variables through a dictionary interface.
        /// </summary>
        /// <param name="name">Name of the variable.</param>
        /// <returns>The value for the given variable.</returns>
        public dynamic this[string name]
        {
            get {
                dynamic output;
                return m_scope.TryGetVariable(name, out output) ? output : null;
            }
            set { m_scope.SetVariable(name, (object)value); }
        }
    }
}
