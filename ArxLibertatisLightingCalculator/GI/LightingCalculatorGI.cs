using ArxLibertatisEditorIO.MediumIO;
using ArxLibertatisEditorIO.MediumIO.FTS;
using ArxLibertatisEditorIO.Util;
using ArxLibertatisLightingCalculator.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;
using System.Linq;

namespace ArxLibertatisLightingCalculator.GI
{
    public class LightingCalculatorGI : ILightingCalculator
    {
        //protected readonly List<Light> dynLights = new List<Light>();
        protected readonly List<Patch> patches = new();

        public virtual void Calculate(MediumArxLevel mal)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            //clear lists
            patches.Clear();

            //calculate normal lighting to use as base value for patches
            var preCalculator = new LightingCalculatorDistanceAngleShadow();
            preCalculator.Calculate(mal);

            //TODO: would it be smarter to group polygons into a grid of patches instead of a patch for each poly? O(n²) after all
            //mesh to patches
            int vertIndex = 0;
            for (int i = 0; i < mal.FTS.cells.Count; ++i)
            {
                var c = mal.FTS.cells[i];
                for (int j = 0; j < c.polygons.Count; ++j)
                {
                    var p = c.polygons[j];
                    patches.Add(Patch.FromPolygon(p, ref vertIndex, mal.LLF.lightColors));
                }
            }
            int vertCount = vertIndex;

            //solve radiosity
            SolveRadiosity(2);

            //Parallel.ForEach(patches, (p) => { p.Radiosity.Clamp(); });

            List<Tuple<int, Vertex>> workOrders = new();

            //calculate vertices again
            ProgressPrinter prog = new(vertCount, "Calculating Vertex Light");
            vertIndex = 0;
            for (int i = 0; i < mal.FTS.cells.Count; ++i)
            {
                var c = mal.FTS.cells[i];
                for (int j = 0; j < c.polygons.Count; ++j)
                {
                    var p = c.polygons[j];
                    for (int k = 0; k < p.VertexCount; ++k)
                    {
                        workOrders.Add(new Tuple<int, Vertex>(vertIndex++, p.vertices[k]));
                    }
                }
            }

            Parallel.ForEach(workOrders, (tup) =>
            {
                mal.LLF.lightColors[tup.Item1] = CalculateVertex(tup.Item2);
                prog.Progress();
            });

            stopwatch.Stop();
            Console.WriteLine($"Total Time: {stopwatch.Elapsed}");
        }

        public virtual float CalculateFormFactor(Patch destination, Patch source)
        {
            // Calculate the Form Factor between p1 and p2
            // This should take into account the distance and angle between the patches
            // You can use a simplified version like the dot product between their normals
            //float formFactor = Vector3.Dot(p1.Normal, p2.Normal); //TODO: ask chatgpt to make this better
            //return formFactor;

            /*float distance = Vector3.Distance(p1.Position, p2.Position);
            float cosineTheta = Vector3.Dot(p1.Normal, Vector3.Normalize(p2.Position - p1.Position));
            float cosinePhi = Vector3.Dot(p2.Normal, Vector3.Normalize(p1.Position - p2.Position));

            float solidAngle = 2 * MathF.PI * (1 - cosineTheta) * (1 - cosinePhi);
            float formFactor = (p1.size * p2.size * cosineTheta * cosinePhi) / (MathF.PI * distance * distance * solidAngle);*/

            float factorAngle = Vector3.Dot(destination.normal, source.normal);
            if (factorAngle < 0)
            {
                return 0;
            }
            float distance = Vector3.Distance(destination.position, source.position);
            float factorDistance = MathF.Pow(Math.Clamp(1 - distance / 100, 0, 1), 2);
            float sizeMax = MathF.Max(destination.size, source.size);
            float destSize = destination.size / sizeMax;
            float sourceSize = source.size / sizeMax;
            float factorSize = MathF.Pow(1 - destSize, 2) * MathF.Pow(sourceSize, 2);

            return factorAngle * factorDistance * factorSize;
        }

        public virtual void SolveRadiosity(int iterations)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; ++i)
            {
                Console.WriteLine($"Radiosity Iteration {i + 1}/{iterations}");
                ProgressPrinter prog = new(patches.Count * patches.Count, "Calculating Radiosity");
                Parallel.ForEach(patches, (destination) =>
                {
                    Color bouncedLight = new(0, 0, 0);

                    foreach (Patch source in patches)
                    {
                        if (destination != source)
                        {
                            float formFactor = CalculateFormFactor(destination, source);
                            bouncedLight += source.radiosity * source.reflectivity * formFactor;
                        }
                        prog.Progress();
                    }

                    destination.radiosityTmp = destination.emission + bouncedLight;
                });
            }
            Parallel.ForEach(patches, (p) =>
            {
                p.radiosity = p.radiosityTmp;
            });
            stopwatch.Stop();
            Console.WriteLine($"Radiosity Iterations({iterations}) took {stopwatch.Elapsed}");
        }

        class DistanceComparer : IComparer<Patch>
        {
            Vector3 vertexPosition;
            public DistanceComparer(Vector3 vertexPosition)
            {
                this.vertexPosition = vertexPosition;
            }

            public int Compare(Patch x, Patch y)
            {
                return Vector3.Distance(x.position, vertexPosition).CompareTo(Vector3.Distance(y.position, vertexPosition));
            }
        }

        public IEnumerable<Patch> FindSurroundingPatches(Vector3 vertexPosition, float distance)
        {
            var comparer = new DistanceComparer(vertexPosition);
            //return patches.ToImmutableSortedSet(comparer).Take(numPatches).ToList();
            return patches.Where((x) => Vector3.Distance(x.position, vertexPosition) < distance);
        }

        public Color CalculateVertex(Vertex v)
        {
            var surroundingPatches = FindSurroundingPatches(v.position, 300);
            Color interpolatedColor = new(0, 0, 0);

            foreach (Patch patch in surroundingPatches)
            {
                float factorAngle = Vector3.Dot(patch.normal, v.normal);
                if (factorAngle < 0)
                {
                    continue;
                }
                float weight = patch.size / 100;
                float distance = Vector3.Distance(patch.position, v.position);
                float factorDistance = MathF.Pow(Math.Clamp(1 - distance / 300, 0, 1), 2);
                weight *= factorDistance;

                interpolatedColor += patch.radiosity * weight;
            }

            return interpolatedColor.Clamped();
        }
    }
}
