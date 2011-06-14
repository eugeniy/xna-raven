using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Raven
{
    public class Sphere
    {
        private VertexPositionNormal[] m_vertices;
        private int[] m_indices;


        public Sphere(int width, int height)
        {
            m_indices = GenerateIndices(width, height);
            m_vertices = CalculateVertexNormals(GenerateVertices(width, height), Indices);
        }


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


        /// <summary>
        /// Generates an array of skydome vertices.
        /// </summary>
        /// <param name="width">Number of vertices along the equator of the sphere.</param>
        /// <param name="height">Number of vertices from one pole to another.</param>
        public VertexPositionNormal[] GenerateVertices(int width, int height)
        {
            var vertices = new List<VertexPositionNormal>();

            // Create sphere vertices
            for (int j = 1; j < height - 1; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    double theta = j / (float)(height - 1) * Math.PI;
                    double phi = i / (float)(width - 1) * Math.PI * 2;

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
        /// Generates an array of skydome indices.
        /// </summary>
        /// <param name="width">Number of vertices along the equator of the sphere.</param>
        /// <param name="height">Number of vertices from one pole to another.</param>
        public int[] GenerateIndices(int width, int height)
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
        /// Create a vertex array with calculated normals.
        /// </summary>
        /// <param name="vertices">A vertex array, assumes normal values are zero.</param>
        /// <param name="indices">An index array for the skydome.</param>
        public VertexPositionNormal[] CalculateVertexNormals(VertexPositionNormal[] vertices, int[] indices)
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


        public VertexPositionNormal[] Vertices { get { return m_vertices; } }
        public int[] Indices { get { return m_indices; } }
    }
}
