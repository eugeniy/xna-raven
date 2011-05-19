using System;
using Microsoft.Xna.Framework;
using Microsoft.Scripting;
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
        protected CompiledCode m_code;

        public Console(Game game) : base(game)
        {
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
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// Compile the given source code string.
        /// </summary>
        /// <param name="code">The source code.</param>
        /// <returns>True if compiled successfully, false otherwise.</returns>
        public bool Compile(string code)
        {
            try
            {
                var source = m_engine.CreateScriptSourceFromString(code, SourceCodeKind.Statements);
                m_code = source.Compile();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Execute the compiled source code.
        /// </summary>
        /// <returns>True if executed successfully, false otherwise.</returns>
        public bool Execute()
        {
            try
            {
                m_code.Execute(m_scope);
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
