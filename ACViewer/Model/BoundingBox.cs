using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using ACViewer.Enum;
using ACViewer.Render;

namespace ACViewer.Model
{
    public class BoundingBox
    {
        public Vector3 Mins { get; set; }
        public Vector3 Maxs { get; set; }

        public Vector3 Size { get; set; }
        
        public float MaxSize { get; set; }

        public Vector3 Center { get; set; }

        public List<Vector3> Verts { get; set; }

        public List<Face> Faces { get; set; }

        public BoundingBox(List<Vector3> verts)
        {
            Init(verts);
        }

        public void Init(List<Vector3> verts)
        {
            GetMinMax(verts);
            BuildVerts();
            BuildFaces();
        }

        public BoundingBox(Dictionary<TextureSet, InstanceBatch> batches)
        {
            var verts = new List<Vector3>();

            foreach (var batch in batches.Values)
            {
                foreach (var instance in batch.Instances_Env)
                {
                    var pos = instance.Position;
                    var rotation = instance.Rotation;

                    var transform = Matrix.CreateFromQuaternion(new Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W)) * Matrix.CreateTranslation(pos);

                    foreach (var drawCall in batch.DrawCalls.Values)
                    {
                        foreach (var vertex in drawCall.Vertices)
                            verts.Add(Vector3.Transform(vertex.Position, transform));
                    }
                }
            }
            Init(verts);
        }

        public void GetMinMax(List<Vector3> verts)
        {
            var mins = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var maxs = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (var vert in verts)
            {
                if (vert.X < mins.X)
                    mins.X = vert.X;
                if (vert.Y < mins.Y)
                    mins.Y = vert.Y;
                if (vert.Z < mins.Z)
                    mins.Z = vert.Z;

                if (vert.X > maxs.X)
                    maxs.X = vert.X;
                if (vert.Y > maxs.Y)
                    maxs.Y = vert.Y;
                if (vert.Z > maxs.Z)
                    maxs.Z = vert.Z;
            }

            Mins = mins;
            Maxs = maxs;

            Size = new Vector3(Maxs.X - Mins.X, Maxs.Y - Mins.Y, Maxs.Z - Mins.Z);

            MaxSize = Math.Max(Math.Max(Size.X, Size.Y), Size.Z);

            var halfSize = Size * 0.5f;
            
            Center = new Vector3(Mins.X + halfSize.X, Mins.Y + halfSize.Y, Mins.Z + halfSize.Z);

            //Console.WriteLine("Mins: " + Mins);
            //Console.WriteLine("Maxs: " + Maxs);
            //Console.WriteLine("Size: " + Size);
        }

        public void BuildVerts()
        {
            Verts = new List<Vector3>();
            /*Verts.Add(new Vector3(Mins.X, Mins.Y, Maxs.Z));   // Front NW
            Verts.Add(new Vector3(Maxs.X, Mins.Y, Maxs.Z));   // Front NE
            Verts.Add(new Vector3(Maxs.X, Mins.Y, Mins.Z));   // Front SE
            Verts.Add(new Vector3(Mins.X, Mins.Y, Mins.Z));   // Front SW
            Verts.Add(new Vector3(Maxs.X, Maxs.Y, Maxs.Z));   // Back NE
            Verts.Add(new Vector3(Mins.X, Maxs.Y, Maxs.Z));   // Back NW
            Verts.Add(new Vector3(Mins.X, Maxs.Y, Mins.Z));   // Back SW
            Verts.Add(new Vector3(Maxs.X, Maxs.Y, Mins.Z));   // Back SE*/

            Verts.Add(new Vector3(Maxs.X, Maxs.Y, Maxs.Z));   // Front NW
            Verts.Add(new Vector3(Mins.X, Maxs.Y, Maxs.Z));   // Front NE
            Verts.Add(new Vector3(Mins.X, Maxs.Y, Mins.Z));   // Front SE
            Verts.Add(new Vector3(Maxs.X, Maxs.Y, Mins.Z));   // Front SW
            Verts.Add(new Vector3(Mins.X, Mins.Y, Maxs.Z));   // Back NE
            Verts.Add(new Vector3(Maxs.X, Mins.Y, Maxs.Z));   // Back NW
            Verts.Add(new Vector3(Maxs.X, Mins.Y, Mins.Z));   // Back SW
            Verts.Add(new Vector3(Mins.X, Mins.Y, Mins.Z));   // Back SE
        }

        public void BuildFaces()
        {
            Faces = new List<Face>();

            var front = new Face(Facing.Front, Verts[0], Verts[1], Verts[2], Verts[3], -Vector3.UnitY);
            var back = new Face(Facing.Back, Verts[4], Verts[5], Verts[6], Verts[7], Vector3.UnitY);
            var left = new Face(Facing.Left, Verts[5], Verts[0], Verts[3], Verts[6], -Vector3.UnitX);
            var right = new Face(Facing.Right, Verts[1], Verts[4], Verts[7], Verts[2], Vector3.UnitX);
            var top = new Face(Facing.Top, Verts[5], Verts[4], Verts[1], Verts[0], Vector3.UnitZ);
            var bottom = new Face(Facing.Bottom, Verts[3], Verts[2], Verts[7], Verts[6], -Vector3.UnitZ);

            Faces.AddRange(new List<Face>() { front, back, left, right, top, bottom });
        }

        public Facing GetTargetFace()
        {
            var eval = new List<Face>() { Faces[(int)Facing.Front], Faces[(int)Facing.Left], Faces[(int)Facing.Top] };
            var gfxObjMode = ModelViewer.Instance.GfxObjMode;
            if (gfxObjMode)
                eval = new List<Face>() { Faces[(int)Facing.Back], Faces[(int)Facing.Right], Faces[(int)Facing.Top] };

            var sorted = eval.OrderByDescending(i => i.Area).ToList();

            //foreach (var face in sorted)
                //Console.WriteLine($"Face: {face.Facing} - Area: {face.Area}");

            var mostArea = sorted[0];

            if (mostArea.Facing == Facing.Top)
            {
                var secondMostArea = sorted[1];
                var ratio = secondMostArea.Area / mostArea.Area;
                if (ratio > 0.5f)     // should we still be using top? it was originally designed for screenshot mode, and can be confusing w/ camera
                    return sorted[1].Facing;
            }

            return sorted[0].Facing;
        }
    }
}
