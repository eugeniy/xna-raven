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
    public class Skydome : DrawableGameComponent
    {
        protected ContentManager m_content;

        protected Effect m_effect;
        protected VertexBuffer m_vertexBuffer;
        protected IndexBuffer m_indexBuffer;

        protected const int m_width = 32;
        protected const int m_height = 16;

        public Model Model;
        public Effect Effect;
        public int Count;
        public int Width;
        public int Height;


        public Skydome(Game game, ContentManager content) : base(game)
        {
            m_content = content;
        }

        protected override void LoadContent()
        {
            m_effect = m_content.Load<Effect>(@"Shaders\Sky");

            GenerateSkydome(m_width, m_height);

            base.LoadContent();
        }




        /// <summary>
        /// Generates the skydome and places the result into vertex buffers.
        /// </summary>
        /// <param name="width">Number of vertices along the equator of the sphere.</param>
        /// <param name="height">Number of vertices from one pole to another.</param>
        protected void GenerateSkydome(int width, int height)
        {
            var vertices = new List<VertexPositionColor>();
            var indices = new List<int>();
            double theta, phi;

            //var verts = new VertexPositionColor

            // Create sphere vertices
            for (int j = 1; j < height - 1; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    theta = j / (float)(height - 1) * Math.PI;
                    phi = i / (float)(width - 1) * Math.PI * 2;

                    // Add a vector with the given position and color
                    vertices.Add(new VertexPositionColor(new Vector3(
                        (float)(Math.Sin(theta) * Math.Cos(phi)),
                        (float)Math.Cos(theta),
                        (float)(-Math.Sin(theta) * Math.Sin(phi))
                    ), Color.Magenta));
                }
            }
            // Add poles of the sphere
            vertices.Add(new VertexPositionColor(new Vector3(0, 1, 0), Color.Magenta));
            vertices.Add(new VertexPositionColor(new Vector3(0, -1, 0), Color.Magenta));


            // Create sphere indices
            for (int j = 0; j < height - 3; j++)
            {
                for (int i = 0; i < width - 1; i++)
                {
                    indices.Add(j * width + i);
                    indices.Add((j + 1) * width + i + 1);

                    indices.Add(j * width + i + 1);
                    indices.Add(j * width + i);

                    indices.Add((j + 1) * width + i);
                    indices.Add((j + 1) * width + i + 1);
                }
            }

            // Again add pole indices
            for (int i = 0; i < width - 1; i++)
            {
                indices.Add((height - 2) * width);
                indices.Add(i);
                indices.Add(i + 1);
                indices.Add((height - 2) * width + 1);
                indices.Add((height - 3) * width + i + 1);
                indices.Add((height - 3) * width + i);
            }

            // Allocate memory on the graphics device and copy vertices in it
            m_vertexBuffer = new VertexBuffer(GraphicsDevice, VertexPositionColor.VertexDeclaration, vertices.Count, BufferUsage.WriteOnly);
            m_vertexBuffer.SetData(vertices.ToArray());

            // Do the same for indices
            m_indexBuffer = new IndexBuffer(GraphicsDevice, typeof(int), indices.Count, BufferUsage.WriteOnly);
            m_indexBuffer.SetData(indices.ToArray());
        }




        /// <summary>
        /// Display the skydome using the loaded shader.
        /// </summary>
        /// <param name="gameTime">Snapshot of the game timing state.</param>
        /// <param name="camera">Reference to the instance of the camera class.</param>
        /// <param name="graphics">Reference to the graphics device.</param>
        public void Draw(GameTime gameTime, Camera camera)
        {
            // Set renderstates for drawing the sky. For maximum efficiency, we draw the sky
            // after everything else, with depth mode set to read only. This allows the GPU to
            // entirely skip drawing sky in the areas that are covered by other solid objects.
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;


            // The sky is infinitely far away, it should move with the camera and as large as its far plane allows.
            Matrix World = Matrix.CreateScale(camera.FarPlane) * Matrix.CreateTranslation(camera.Position);


            m_effect.CurrentTechnique = m_effect.Techniques["Simple"];
            m_effect.Parameters["World"].SetValue(World);
            m_effect.Parameters["View"].SetValue(camera.View);
            m_effect.Parameters["Projection"].SetValue(camera.Projection);
            m_effect.Parameters["Time"].SetValue(gameTime.TotalGameTime.Seconds * 1000 + gameTime.TotalGameTime.Milliseconds);

            GraphicsDevice.Indices = m_indexBuffer;
            GraphicsDevice.SetVertexBuffer(m_vertexBuffer);

            foreach (EffectPass pass in m_effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m_vertexBuffer.VertexCount, 0, m_indexBuffer.IndexCount / 3);
            }


            // Set modified renderstates back to their default values.
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

    }
}
