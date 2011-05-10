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
    public class Skydome
    {
        public Model Model;
        public Effect Effect;


        /// <summary>
        /// Apply the shader effect to all meshes in the model.
        /// TODO: Figure out how to to do this when model and effect are automatically serialized.
        /// </summary>
        public void Initialize()
        {
            if (!Effect.Equals(Model.Meshes[0].MeshParts[0].Effect))
                Model.Meshes[0].MeshParts[0].Effect = Effect.Clone();
        }


        /// <summary>
        /// Display the skydome using the loaded shader.
        /// </summary>
        /// <param name="gameTime">Snapshot of the game timing state.</param>
        /// <param name="camera">Reference to the instance of the camera class.</param>
        /// <param name="graphics">Reference to the graphics device.</param>
        public void Draw(GameTime gameTime, Camera camera, GraphicsDevice graphics)
        {
            // Set renderstates for drawing the sky. For maximum efficiency, we draw the sky
            // after everything else, with depth mode set to read only. This allows the GPU to
            // entirely skip drawing sky in the areas that are covered by other solid objects.
            graphics.DepthStencilState = DepthStencilState.DepthRead;


            // Copy model transforms to a matrix.
            var transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);

            // The sky is infinitely far away, it should move with the camera and as large as its far plane allows.
            Matrix World = Matrix.CreateScale(camera.FarPlane) * Matrix.CreateTranslation(camera.Position);


            // Draw the sky model.
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.CurrentTechnique = effect.Techniques["Simple"];
                    effect.Parameters["World"].SetValue(transforms[mesh.ParentBone.Index] * World);
                    effect.Parameters["View"].SetValue(camera.View);
                    effect.Parameters["Projection"].SetValue(camera.Projection);
                }

                mesh.Draw();
            }


            // Set modified renderstates back to their default values.
            graphics.DepthStencilState = DepthStencilState.Default;
        }

    }
}
