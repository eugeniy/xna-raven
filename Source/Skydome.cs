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



        public Skydome()
        {
        }



        public void Draw(GameTime gameTime, Camera camera, GraphicsDevice graphics)
        {

            // Set renderstates for drawing the sky. For maximum efficiency, we draw the sky
            // after everything else, with depth mode set to read only. This allows the GPU to
            // entirely skip drawing sky in the areas that are covered by other solid objects.
            //GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            //GraphicsDevice.BlendState = BlendState.Opaque;

            // Because the sky is infinitely far away, it should not move sideways as the camera
            // moves around the world, so we force the view matrix translation to zero. This
            // way the sky only takes the camera rotation into account, ignoring its position.
            //m_camera.View.Translation = Vector3.Zero;

            // The sky should be drawn behind everything else, at the far clip plane.
            // We achieve this by tweaking the projection matrix to force z=w.
            //projection.M13 = projection.M14;
            //projection.M23 = projection.M24;
            //projection.M33 = projection.M34;
            //projection.M43 = projection.M44;

            RasterizerState rs = new RasterizerState();
            rs.FillMode = FillMode.WireFrame;
            rs.CullMode = CullMode.None;
            graphics.RasterizerState = rs;


            // Draw the sky model.
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                }

                mesh.Draw();
            }

            // Set modified renderstates back to their default values.
            //GraphicsDevice.DepthStencilState = DepthStencilState.Default;


        }

    }
}
