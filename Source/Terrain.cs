using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Raven
{
    public class Terrain : DrawableGameComponent
    {
        protected const int m_width = 20;
        protected const int m_height = 10;

        protected VertexPositionColor[] m_vertices;
        protected int[] m_indices;
        protected float[,] m_heightMap;

        protected ContentManager m_content;
        protected Effect m_effect;
        protected VertexBuffer m_vertexBuffer;
        protected IndexBuffer m_indexBuffer;

        public Terrain(Game game, ContentManager content) : base(game)
        {
            m_content = content;
            m_heightMap = new float[m_width, m_height];
        }

        /// <summary>
        /// Load content used by the component to generate the terrain.
        /// </summary>
        protected override void LoadContent()
        {
            LoadHeightMap();
            GenerateVertices();
            GenerateIndices();
            LoadToBuffer();

            // Load a simple shader used to display the terrain
            m_effect = m_content.Load<Effect>(@"Shaders\Simple");

            base.LoadContent();
        }

        /// <summary>
        /// Load values into the height map array.
        /// </summary>
        protected void LoadHeightMap()
        {
        }

        /// <summary>
        /// Generate terrain vertices.
        /// </summary>
        protected void GenerateVertices()
        {
            m_vertices = new VertexPositionColor[m_width * m_height];

            for (int x = 0; x < m_width; x++)
            {
                for (int y = 0; y < m_height; y++)
                {
                    m_vertices[x + y * m_width].Position = new Vector3(x, m_heightMap[x, y], -y);
                    m_vertices[x + y * m_width].Color = Color.White;
                }
            }
        }

        /// <summary>
        /// Generate terrain indices.
        /// </summary>
        protected void GenerateIndices()
        {
            int index = 0;
            m_indices = new int[(m_width - 1) * (m_height - 1) * 6];

            for (int y = 0; y < m_height - 1; y++)
            {
                for (int x = 0; x < m_width - 1; x++)
                {
                    m_indices[index++] = x + (y + 1) * m_width; // top left
                    m_indices[index++] = (x + 1) + y * m_width; // bottom right
                    m_indices[index++] = x + y * m_width; // bottom left

                    m_indices[index++] = x + (y + 1) * m_width;
                    m_indices[index++] = (x + 1) + (y + 1) * m_width; // top right
                    m_indices[index++] = (x + 1) + y * m_width;
                }
            }
        }

        /// <summary>
        /// Copies vertices and indices into a video card buffers.
        /// </summary>
        protected void LoadToBuffer()
        {
            // Allocate memory on the graphics device and copy vertices in it
            m_vertexBuffer = new VertexBuffer(GraphicsDevice, VertexPositionColor.VertexDeclaration, m_vertices.Length, BufferUsage.WriteOnly);
            m_vertexBuffer.SetData(m_vertices);

            // Do the same for indices
            m_indexBuffer = new IndexBuffer(GraphicsDevice, typeof(int), m_indices.Length, BufferUsage.WriteOnly);
            m_indexBuffer.SetData(m_indices);
        }

        /// <summary>
        /// Display the terrain using the given shader.
        /// </summary>
        /// <param name="gameTime">Snapshot of the game timing state.</param>
        /// <param name="camera">Reference to the instance of the camera class.</param>
        public void Draw(GameTime gameTime, Camera camera)
        {
            m_effect.CurrentTechnique = m_effect.Techniques["Simple"];
            m_effect.Parameters["World"].SetValue(Matrix.Identity);
            m_effect.Parameters["View"].SetValue(camera.View);
            m_effect.Parameters["Projection"].SetValue(camera.Projection);

            foreach (EffectPass pass in m_effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsDevice.Indices = m_indexBuffer;
                GraphicsDevice.SetVertexBuffer(m_vertexBuffer);
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m_vertexBuffer.VertexCount, 0, m_indexBuffer.IndexCount / 3);
            }
        }
    }
}
