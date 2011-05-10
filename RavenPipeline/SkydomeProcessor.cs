using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace RavenPipeline
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentProcessor(DisplayName = "Raven Skydome Processor")]
    public class SkydomeProcessor : ContentProcessor<EffectContent, SkydomeContent>
    {
        protected const int m_width = 32;
        protected const int m_height = 16;
        protected const int m_triangleCount = (m_width - 1) * (m_height - 2) * 2;


        public override SkydomeContent Process(EffectContent input, ContentProcessorContext context)
        {
            var vertices = new List<int>();
            double theta, phi;

            MeshBuilder builder = MeshBuilder.StartMesh("Sphere");
            builder.SetMaterial(new BasicMaterialContent());


            // Create sphere vertices
            for (int j = 1; j < m_height - 1; j++)
            {
                for (int i = 0; i < m_width; i++)
                {
                    theta = j / (float)(m_height - 1) * Math.PI;
                    phi = i / (float)(m_width - 1) * Math.PI * 2;

                    vertices.Add(builder.CreatePosition(
                        (float)(Math.Sin(theta) * Math.Cos(phi)),
                        (float)Math.Cos(theta),
                        (float)(-Math.Sin(theta) * Math.Sin(phi))
                    ));
                }
            }
            // Add poles of the sphere
            vertices.Add(builder.CreatePosition(0, 1, 0));
            vertices.Add(builder.CreatePosition(0, -1, 0));


            // Draw individual triangles
            for (int j = 0; j < m_height - 3; j++)
            {
                for (int i = 0; i < m_width - 1; i++)
                {
                    builder.AddTriangleVertex(vertices[(j) * m_width + i]);
                    builder.AddTriangleVertex(vertices[(j + 1) * m_width + i + 1]);

                    builder.AddTriangleVertex(vertices[(j) * m_width + i + 1]);
                    builder.AddTriangleVertex(vertices[(j) * m_width + i]);

                    builder.AddTriangleVertex(vertices[(j + 1) * m_width + i]);
                    builder.AddTriangleVertex(vertices[(j + 1) * m_width + i + 1]);
                }
            }

            for (int i = 0; i < m_width - 1; i++)
            {
                builder.AddTriangleVertex(vertices[(m_height - 2) * m_width]);
                builder.AddTriangleVertex(vertices[i]);
                builder.AddTriangleVertex(vertices[i + 1]);
                builder.AddTriangleVertex(vertices[(m_height - 2) * m_width + 1]);
                builder.AddTriangleVertex(vertices[(m_height - 3) * m_width + i + 1]);
                builder.AddTriangleVertex(vertices[(m_height - 3) * m_width + i]);
            }


            // Chain to the ModelProcessor so it can convert the mesh we just generated.
            MeshContent skyMesh = builder.FinishMesh();

            // Create the output object.
            SkydomeContent output = new SkydomeContent();
            output.Model = context.Convert<MeshContent, ModelContent>(skyMesh, "ModelProcessor");
            output.Effect = context.Convert<EffectContent, CompiledEffectContent>(input, "EffectProcessor");

            return output;
        }
    }
}