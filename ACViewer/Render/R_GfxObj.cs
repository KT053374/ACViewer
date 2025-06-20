using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ACE.Entity.Enum;
using ACE.Server.Physics.Collision;

namespace ACViewer.Render
{
    public class R_GfxObj
    {
        public GfxObj GfxObj { get; set; }

        public VertexPositionColor[] Vertices { get; set; }
        public int[] Indices { get; set; }
        public Vector3 Scale { get; set; }

        public R_GfxObj(GfxObj gfxObj, Vector3 scale)
        {
            GfxObj = gfxObj;
            Scale = scale;

            BuildVertices();
            BuildIndices();
        }

        public void BuildVertices()
        {
            var vertices = new List<VertexPositionColor>();

            // get max index
            var maxIdx = GetMaxIndex();
            if (maxIdx != GfxObj.VertexArray.Vertices.Count)
                Console.WriteLine($"WARNING: maxIdx != vertices.count ({maxIdx} vs. {GfxObj.VertexArray.Vertices.Count})");

            for (var i = 0; i < maxIdx; i++)
            {
                GfxObj.VertexArray.Vertices.TryGetValue((ushort)i, out var vertex);
                if (vertex == null)
                {
                    Console.WriteLine("Adding null vertex at index " + i);
                    vertices.Add(new VertexPositionColor());
                    continue;
                }
                vertices.Add(new VertexPositionColor(vertex.Origin.ToXna() * Scale, Color.White));
            }
            Vertices = vertices.ToArray();
        }

        public void BuildIndices()
        {
            var indices = new List<int>();

            int firstPolyIdx = 0;
            // dictionary -> will these already be read in correct order?
            foreach (var poly in GfxObj.Polygons.Values)
            {
                // TODO: improve rendering for 2-sided faces
                //if (poly.Stippling == StipplingType.NoPos) continue;
                
                var polyVerts = poly.VertexIDs.Count;
                for (var i = 0; i < polyVerts; i++)
                {
                    var v = (int)poly.VertexIDs[i];
                    if (i == 0)
                        firstPolyIdx = v;

                    indices.Add(v);
                    if (i != polyVerts - 1)
                        indices.Add(poly.VertexIDs[i + 1]);
                    else
                        indices.Add(firstPolyIdx);
                }
            }
            Indices = indices.ToArray();
        }

        public ushort GetMaxIndex()
        {
            var keys = GfxObj.VertexArray.Vertices.Keys.ToList();
            keys.Sort();
            keys.Reverse();

            var max = keys.FirstOrDefault();
            return (ushort)(max + 1);
        }
    }
}
