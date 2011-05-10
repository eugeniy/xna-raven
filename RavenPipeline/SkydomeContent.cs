using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content;

namespace RavenPipeline
{
    /// <summary>
    /// Load data generated from the XNB file created by SkydomeProcessor to a Skydome class.
    /// </summary>
    [ContentSerializerRuntimeType("Raven.Skydome, Raven")]
    public class SkydomeContent
    {
        public ModelContent Model;
        public CompiledEffectContent Effect;
        public int Count;
        public int Width;
        public int Height;
    }
}
