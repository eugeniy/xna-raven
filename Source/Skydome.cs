using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Raven
{
    public class Skydome : DrawableGameComponent
    {
        protected ContentManager m_content;

        protected Effect m_effect;
        protected VertexBuffer m_vertexBuffer;
        protected IndexBuffer m_indexBuffer;

        protected const int m_width = 32;
        protected const int m_height = 16;


        public Skydome(Game game, ContentManager content) : base(game)
        {
            m_content = content;
        }


        /// <summary>
        /// Load content used by the component and generate the skydome.
        /// </summary>
        protected override void LoadContent()
        {
            m_effect = m_content.Load<Effect>(@"Shaders\Sky");

            // Load the skydome into memory, ready to be drawn
            LoadToBuffer(new Sphere(m_width, m_height));

            base.LoadContent();
        }


        /// <summary>
        /// Dispose of all content and resources created by the component.
        /// </summary>
        protected override void UnloadContent()
        {
            m_effect.Dispose();
            m_vertexBuffer.Dispose();
            m_indexBuffer.Dispose();

            base.UnloadContent();
        }


        /// <summary>
        /// Generates the skydome and places the result into vertex and index buffers.
        /// </summary>
        /// <param name="sphere">An instance of a sphere to be used as a skydome.</param>
        protected void LoadToBuffer(Sphere sphere)
        {
            // Allocate memory on the graphics device and copy vertices in it
            m_vertexBuffer = new VertexBuffer(GraphicsDevice, Sphere.VertexPositionNormal.VertexDeclaration, sphere.Vertices.Length, BufferUsage.WriteOnly);
            m_vertexBuffer.SetData(sphere.Vertices);

            // Do the same for indices
            m_indexBuffer = new IndexBuffer(GraphicsDevice, typeof(int), sphere.Indices.Length, BufferUsage.WriteOnly);
            m_indexBuffer.SetData(sphere.Indices);
        }


        /// <summary>
        /// Display the skydome using the loaded shader.
        /// </summary>
        /// <param name="gameTime">Snapshot of the game timing state.</param>
        /// <param name="camera">Reference to the instance of the camera class.</param>
        public void Draw(GameTime gameTime, Camera camera)
        {
            // Set renderstates for drawing the sky. For maximum efficiency, we draw the sky
            // after everything else, with depth mode set to read only. This allows the GPU to
            // entirely skip drawing sky in the areas that are covered by other solid objects.
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;


            // The sky is infinitely far away, it should move with the camera and as large as its far plane allows.
            Matrix world = Matrix.CreateScale(camera.FarPlane) * Matrix.CreateTranslation(camera.Position);


            m_effect.CurrentTechnique = m_effect.Techniques["Simple"];
            m_effect.Parameters["World"].SetValue(world);
            m_effect.Parameters["View"].SetValue(camera.View);
            m_effect.Parameters["Projection"].SetValue(camera.Projection);
            m_effect.Parameters["Time"].SetValue(gameTime.TotalGameTime.Seconds * 1000 + gameTime.TotalGameTime.Milliseconds);

            foreach (EffectPass pass in m_effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsDevice.Indices = m_indexBuffer;
                GraphicsDevice.SetVertexBuffer(m_vertexBuffer);
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m_vertexBuffer.VertexCount, 0, m_indexBuffer.IndexCount / 3);
            }


            // Set modified renderstates back to their default values.
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

    }
}
