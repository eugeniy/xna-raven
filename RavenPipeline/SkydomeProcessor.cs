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
    [ContentProcessor(DisplayName = "Skydome Processor")]
    public class SkydomeProcessor : ContentProcessor<EffectContent, SkydomeContent>
    {
        const float cylinderSize = 100;
        const int cylinderSegments = 32;

        public override SkydomeContent Process(EffectContent input, ContentProcessorContext context)
        {
            MeshBuilder builder = MeshBuilder.StartMesh("sky");

            // Create two rings of vertices around the top and bottom of the cylinder.
            List<int> topVertices = new List<int>();
            List<int> bottomVertices = new List<int>();

            for (int i = 0; i < cylinderSegments; i++)
            {
                float angle = MathHelper.TwoPi * i / cylinderSegments;

                float x = (float)Math.Cos(angle) * cylinderSize;
                float z = (float)Math.Sin(angle) * cylinderSize;

                topVertices.Add(builder.CreatePosition(x, cylinderSize, z));
                bottomVertices.Add(builder.CreatePosition(x, -cylinderSize, z));
            }

            // Create two center vertices, used for closing the top and bottom.
            int topCenterVertex = builder.CreatePosition(0, cylinderSize * 2, 0);
            int bottomCenterVertex = builder.CreatePosition(0, -cylinderSize * 2, 0);

            builder.SetMaterial(new BasicMaterialContent());



            // Create the individual triangles that make up our skydome.
            for (int i = 0; i < cylinderSegments; i++)
            {
                int j = (i + 1) % cylinderSegments;

                // Calculate texture coordinates for this segment of the cylinder.
                float u1 = (float)i / (float)cylinderSegments;
                float u2 = (float)(i + 1) / (float)cylinderSegments;

                // Two triangles form a quad, one side segment of the cylinder.
                builder.AddTriangleVertex(topVertices[i]);
                builder.AddTriangleVertex(topVertices[j]);
                builder.AddTriangleVertex(bottomVertices[i]);

                builder.AddTriangleVertex(topVertices[j]);
                builder.AddTriangleVertex(bottomVertices[j]);
                builder.AddTriangleVertex(bottomVertices[i]);

                // Triangle fanning inward to fill the top above this segment.
                builder.AddTriangleVertex(topCenterVertex);
                builder.AddTriangleVertex(topVertices[j]);
                builder.AddTriangleVertex(topVertices[i]);

                // Triangle fanning inward to fill the bottom below this segment.
                builder.AddTriangleVertex(bottomCenterVertex);
                builder.AddTriangleVertex(bottomVertices[i]);
                builder.AddTriangleVertex(bottomVertices[j]);

            }


            // Chain to the ModelProcessor so it can convert the mesh we just generated.
            MeshContent skyMesh = builder.FinishMesh();

            // Create the output object.
            SkydomeContent output = new SkydomeContent();
            output.Model = context.Convert<MeshContent, ModelContent>(skyMesh, "ModelProcessor");

            //EffectProcessor compiler = new EffectProcessor();
            //CompiledEffectContent compiledContent = compiler.Process(input, context);
            //return new TOutput(compiledContent.GetEffectCode());
            output.Effect = context.Convert<EffectContent, CompiledEffectContent>(input, "EffectProcessor");

            return output;

            // TODO: process the input object, and return the modified data.


        }
    }
}