using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Raven
{
    public class Skydome : DrawableGameComponent
    {
        // Declare a structure to hold skydome vertices.
        public struct VertexPositionNormal
        {
            public Vector3 Position;
            public Vector3 Normal;

            public VertexPositionNormal(Vector3 position, Vector3 normal)
            {
                this.Position = position;
                this.Normal = normal;
            }

            public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
            );
        }

        protected ContentManager m_content;

        protected Effect m_effect;
        protected VertexBuffer m_vertexBuffer;
        protected IndexBuffer m_indexBuffer;

        protected const int m_width = 32;
        protected const int m_height = 16;
        protected const int m_triangleCount = (m_width - 1) * (m_height - 2) * 2;


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
            LoadToBuffer(m_width, m_height);

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
        /// Generates an array of skydome vertices.
        /// </summary>
        /// <param name="width">Number of vertices along the equator of the sphere.</param>
        /// <param name="height">Number of vertices from one pole to another.</param>
        protected VertexPositionNormal[] GenerateVertices(int width, int height)
        {
            var vertices = new List<VertexPositionNormal>();
            double theta, phi;

            // Create sphere vertices
            for (int j = 1; j < height - 1; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    theta = j / (float)(height - 1) * Math.PI;
                    phi = i / (float)(width - 1) * Math.PI * 2;

                    // Add a vector with the given position and color
                    vertices.Add(new VertexPositionNormal(new Vector3(
                        (float)(Math.Sin(theta) * Math.Cos(phi)),
                        (float)Math.Cos(theta),
                        (float)(-Math.Sin(theta) * Math.Sin(phi))
                    ), Vector3.Zero));
                }
            }
            // Add poles of the sphere
            vertices.Add(new VertexPositionNormal(new Vector3(0, 1, 0), Vector3.Zero));
            vertices.Add(new VertexPositionNormal(new Vector3(0, -1, 0), Vector3.Zero));

            return vertices.ToArray();
        }


        /// <summary>
        /// Create a vertex array with calculated normals.
        /// </summary>
        /// <param name="vertices">A vertex array, assumes normal values are zero.</param>
        /// <param name="indices">An index array for the skydome.</param>
        protected VertexPositionNormal[] CalculateVertexNormals(VertexPositionNormal[] vertices, int[] indices)
        {
            for (int i = 0; i < indices.Length / 3; i++)
            {
                int index1 = indices[i * 3];
                int index2 = indices[i * 3 + 1];
                int index3 = indices[i * 3 + 2];

                Vector3 side1 = vertices[index1].Position - vertices[index3].Position;
                Vector3 side2 = vertices[index1].Position - vertices[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                vertices[index1].Normal += normal;
                vertices[index2].Normal += normal;
                vertices[index3].Normal += normal;
            }

            // Normalize normal vectors
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal.Normalize();

            return vertices;
        }


        /// <summary>
        /// Generates an array of skydome indices.
        /// </summary>
        /// <param name="width">Number of vertices along the equator of the sphere.</param>
        /// <param name="height">Number of vertices from one pole to another.</param>
        protected int[] GenerateIndices(int width, int height)
        {
            var indices = new List<int>();

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

            return indices.ToArray();
        }


        /// <summary>
        /// Generates the skydome and places the result into vertex and index buffers.
        /// </summary>
        /// <param name="width">Number of vertices along the equator of the sphere.</param>
        /// <param name="height">Number of vertices from one pole to another.</param>
        protected void LoadToBuffer(int width, int height)
        {
            var vertices = GenerateVertices(width, height);
            var indices = GenerateIndices(width, height);
            vertices = CalculateVertexNormals(vertices, indices);

            // Allocate memory on the graphics device and copy vertices in it
            m_vertexBuffer = new VertexBuffer(GraphicsDevice, VertexPositionNormal.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            m_vertexBuffer.SetData(vertices);

            // Do the same for indices
            m_indexBuffer = new IndexBuffer(GraphicsDevice, typeof(int), indices.Length, BufferUsage.WriteOnly);
            m_indexBuffer.SetData(indices);
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
            Matrix World = Matrix.CreateScale(camera.FarPlane) * Matrix.CreateTranslation(camera.Position);


            m_effect.CurrentTechnique = m_effect.Techniques["Simple"];
            m_effect.Parameters["World"].SetValue(World);
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
