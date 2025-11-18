using System;
using System.Collections.Generic;
using Cinemachine.Utility;
using UnityEngine;
using ClipperLib;

namespace Cinemachine
{
    /// <summary>
    /// Responsible for baking confiners via BakeConfiner function.
    /// </summary>
    internal class ConfinerOven
    {
        public class BakedSolution
        {
            public float FrustumHeight { get; }
            
            private float m_frustumSizeIntSpace;

            private readonly AspectStretcher m_AspectStretcher;
            private readonly bool m_HasBones;
            private readonly double m_SqrPolygonDiagonal;

            private List<List<IntPoint>> m_OriginalPolygon;
            public List<List<IntPoint>> m_Solution;

            private const double k_ClipperEpsilon = 0.01f * k_FloatToIntScaler;

            public BakedSolution(
                float aspectRatio, float frustumHeight, bool hasBones, Rect polygonBounds,
                List<List<IntPoint>> originalPolygon, List<List<IntPoint>> solution)
            {
                m_AspectStretcher = new AspectStretcher(aspectRatio, polygonBounds.center.x);
                FrustumHeight = frustumHeight;
                m_frustumSizeIntSpace = frustumHeight * k_FloatToIntScaler;
                m_HasBones = hasBones;
                m_OriginalPolygon = originalPolygon;
                m_Solution = solution;

                float polygonSizeX = polygonBounds.width / aspectRatio * k_FloatToIntScaler;
                float polygonSizeY = polygonBounds.height * k_FloatToIntScaler;
                m_SqrPolygonDiagonal = polygonSizeX * polygonSizeX + polygonSizeY * polygonSizeY;
            }

            public void Clear()
            {
                m_Solution = null;
                m_OriginalPolygon = null;
            }

            public bool IsValid(float frustumHeight)
            {
                return m_Solution != null && Mathf.Abs(frustumHeight - FrustumHeight) < k_MinStepSize;
            }

            public Vector2 ConfinePoint(in Vector2 pointToConfine)
            {
                if (m_Solution.Count <= 0) return pointToConfine; // empty confiner -> no need to confine
                
                Vector2 pInConfinerSpace = m_AspectStretcher.Stretch(pointToConfine);
                IntPoint p =
                    new IntPoint(pInConfinerSpace.x * k_FloatToIntScaler, pInConfinerSpace.y * k_FloatToIntScaler);
                for (int i = 0; i < m_Solution.Count; ++i)
                {
                    if (Clipper.PointInPolygon(p, m_Solution[i]) != 0) // 0: outside, +1: inside , -1: point on poly boundary
                    {
                        return pointToConfine; // inside, no confinement needed
                    }
                }

                // If the poly has bones and if the position to confine is not outside of the original
                // bounding shape, then it is possible that the bone in a neighbouring section
                // is closer than the bone in the correct section of the polygon, if the current section 
                // is very large and the neighbouring section is small.  In that case, we'll need to 
                // add an extra check when calculating the nearest point.
                bool checkIntersectOriginal = m_HasBones && IsInsideOriginal(p);

                // Confine point
                IntPoint closest = p;
                double minDistance = double.MaxValue;
                for (int i = 0; i < m_Solution.Count; ++i)
                {
                    int numPoints = m_Solution[i].Count;
                    for (int j = 0; j < numPoints; ++j)
                    {
                        IntPoint l1 = m_Solution[i][j];
                        IntPoint l2 = m_Solution[i][(j + 1) % numPoints];

                        IntPoint c = IntPointLerp(l1, l2, ClosestPointOnSegment(p, l1, l2));
                        double diffX = Mathf.Abs(p.X - c.X);
                        double diffY = Mathf.Abs(p.Y - c.Y);
                        double distance = diffX * diffX + diffY * diffY;

                        // penalty for points from which the target is not visible, preferring visibility over proximity
                        if (diffX > m_frustumSizeIntSpace || diffY > m_frustumSizeIntSpace)
                        {
                            distance += m_SqrPolygonDiagonal; // penalty is the biggest distance between any two points
                        }

                        if (distance < minDistance && (!checkIntersectOriginal || !DoesIntersectOriginal(p, c)))
                        {
                            minDistance = distance;
                            closest = c;
                        }
                    }
                }

                var result = new Vector2(closest.X * k_IntToFloatScaler, closest.Y * k_IntToFloatScaler);
                return m_AspectStretcher.Unstretch(result);
            }

#if UNITY_EDITOR
            // Used by inspector to draw the baked path
            private List<List<Vector2>> m_Vector2Path;
            public List<List<Vector2>> GetBakedPath()
            {
                // Convert to client space
                var numPaths = m_Solution.Count;
                if (m_Vector2Path == null)
                {
                    m_Vector2Path = new List<List<Vector2>>(numPaths);
                    for (int i = 0; i < numPaths; ++i)
                    {
                        var srcPoly = m_Solution[i];
                        int numPoints = srcPoly.Count;
                        var pathSegment = new List<Vector2>(numPoints);
                        for (int j = 0; j < numPoints; j++)
                        {
                            // Restore the original aspect ratio
                            pathSegment.Add(m_AspectStretcher.Unstretch(
                                new Vector2(srcPoly[j].X, srcPoly[j].Y) * k_IntToFloatScaler));
                        }

                        m_Vector2Path.Add(pathSegment);
                    }
                }

                return m_Vector2Path;
            }
#endif

            private bool IsInsideOriginal(IntPoint p)
            {
                for (int i = 0; i < m_OriginalPolygon.Count; ++i)
                {
                    if (Clipper.PointInPolygon(p, m_OriginalPolygon[i]) != 0)
                    {
                        return true;
                    }
                }

                return false;
            }

            private static float ClosestPointOnSegment(IntPoint p, IntPoint s0, IntPoint s1)
            {
                double sX = s1.X - s0.X;
                double sY = s1.Y - s0.Y;
                double len2 = sX * sX + sY * sY;
                if (len2 < k_ClipperEpsilon)
                    return 0; // degenerate segment

                double s0pX = p.X - s0.X;
                double s0pY = p.Y - s0.Y;
                double dot = s0pX * sX + s0pY * sY;
                return Mathf.Clamp01((float) (dot / len2));
            }

            private static IntPoint IntPointLerp(IntPoint a, IntPoint b, float lerp)
            {
                return new IntPoint
                {
                    X = Mathf.RoundToInt(a.X + (b.X - a.X) * lerp),
                    Y = Mathf.RoundToInt(a.Y + (b.Y - a.Y) * lerp),
                };
            }

            private bool DoesIntersectOriginal(IntPoint l1, IntPoint l2)
            {
                foreach (var original in m_OriginalPolygon)
                {
                    int numPoints = original.Count;
                    for (int i = 0; i < numPoints; ++i)
                    {
                        if (FindIntersection(l1, l2, original[i], original[(i + 1) % numPoints]) == 2)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            private static int FindIntersection(in IntPoint p1, in IntPoint p2, in IntPoint p3, in IntPoint p4)
            {
                // Get the segments' parameters.
                double dx12 = p2.X - p1.X;
                double dy12 = p2.Y - p1.Y;
                double dx34 = p4.X - p3.X;
                double dy34 = p4.Y - p3.Y;

                // Solve for t1 and t2
                double denominator = dy12 * dx34 - dx12 * dy34;
                double t1 =
                    ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34)
                    / denominator;
                if (double.IsInfinity(t1) || double.IsNaN(t1))
                {
                    // The lines are parallel (or close enough to it).
                    if (IntPointDiffSqrMagnitude(p1, p3) < k_ClipperEpsilon ||
                        IntPointDiffSqrMagnitude(p1, p4) < k_ClipperEpsilon ||
                        IntPointDiffSqrMagnitude(p2, p3) < k_ClipperEpsilon ||
                        IntPointDiffSqrMagnitude(p2, p4) < k_ClipperEpsilon)
                    {
                        return 2; // they are the same line, or very close parallels
                    }

                    return 0; // no intersection
                }

                double t2 = ((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12) / -denominator;
                return (t1 >= 0 && t1 <= 1 && t2 >= 0 && t2 < 1) ? 2 : 1; // 2 = segments intersect, 1 = lines intersect
            }

            private static double IntPointDiffSqrMagnitude(IntPoint p1, IntPoint p2)
            {
                double X = p1.X - p2.X;
                double Y = p1.Y - p2.Y;
                return X * X + Y * Y;
            }
        }

        private struct AspectStretcher
        {
            public float Aspect { get; }
            private readonly float m_InverseAspect;
            private readonly float m_CenterX;

            public AspectStretcher(float aspect, float centerX)
            {
                Aspect = aspect;
                m_InverseAspect = 1 / Aspect;
                m_CenterX = centerX;
            }

            public Vector2 Stretch(Vector2 p)
            {
                return new Vector2((p.x - m_CenterX) * m_InverseAspect + m_CenterX, p.y);
            }

            public Vector2 Unstretch(Vector2 p)
            {
                return new Vector2((p.x - m_CenterX) * Aspect + m_CenterX, p.y);
            }
        }

        private float m_MinFrustumHeightWithBones;

        private List<List<IntPoint>> m_OriginalPolygon;
        private List<List<IntPoint>> m_Skeleton = new List<List<IntPoint>>();

        private const long k_FloatToIntScaler = 100000;
        private const float k_IntToFloatScaler = 1.0f / k_FloatToIntScaler;
        private const float k_MinStepSize = 0.005f;

        private Rect m_PolygonRect;
        AspectStretcher m_AspectStretcher = new AspectStretcher(1, 0);

        private float m_maxComputationTimeForFullSkeletonBakeInSeconds = 5f;

        public ConfinerOven(
            in List<List<Vector2>> inputPath, 
            in float aspectRatio, float maxFrustumHeight) : base()
        {
            Initialize(inputPath, aspectRatio, maxFrustumHeight);
        }

        /// <summary>
        /// Converts and returns a prebaked ConfinerState for the input frustumHeight.
        /// </summary>
        public BakedSolution GetBakedSolution(float frustumHeight)
        {
            // Inflate with clipper to frustumHeight
            var offsetter = new ClipperOffset();
            offsetter.AddPaths(m_OriginalPolygon, JoinType.jtMiter, EndType.etClosedPolygon);
            var solution = new List<List<IntPoint>>();
            offsetter.Execute(ref solution, -1f * frustumHeight * k_FloatToIntScaler);
            
            // Add in the skeleton
            var bakedSolution = new List<List<IntPoint>>();
            if (State == BakingState.BAKING || m_Skeleton.Count == 0)
            {
                bakedSolution = solution;
            }
            else
            {
                Clipper c = new Clipper();
                c.AddPaths(solution, PolyType.ptSubject, true);
                c.AddPaths(m_Skeleton, PolyType.ptClip, true);
                c.Execute(ClipType.ctUnion, bakedSolution, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);
            }

            return new BakedSolution(
                m_AspectStretcher.Aspect, frustumHeight,
                m_MinFrustumHeightWithBones < frustumHeight,
                m_PolygonRect, m_OriginalPolygon, bakedSolution);
        }

        private struct PolygonSolution
        {
            public List<List<IntPoint>> m_Polygons;
            public float m_FrustumHeight;

            public bool StateChanged(in List<List<IntPoint>> paths)
            {
                if (paths.Count != m_Polygons.Count)
                    return true;
                for (int i = 0; i < paths.Count; ++i)
                {
                    if (paths[i].Count != m_Polygons[i].Count)
                        return true;
                }

                return false;
            }

            public bool IsEmpty => m_Polygons == null;
        }

        public enum BakingState 
        {
            BAKING, BAKED, TIMEOUT
        }
        public BakingState State { get; private set; }

        public float m_BakeProgress;

        private struct BakingStateCache
        {
            public ClipperOffset offsetter;
            public List<PolygonSolution> solutions;
            public PolygonSolution rightCandidate;
            public PolygonSolution leftCandidate;
            public List<List<IntPoint>> maxCandidate;
            public float stepSize;
            public float maxFrustumHeight;
            public float currentFrustumHeight;

            public float bakeTime;
        }
        private BakingStateCache m_Cache;

        private void Initialize(in List<List<Vector2>> inputPath, in float aspectRatio, float maxFrustumHeight)
        {
            m_Skeleton.Clear();
            m_Cache.maxFrustumHeight = maxFrustumHeight;
            m_MinFrustumHeightWithBones = float.MaxValue;

            m_PolygonRect = GetPolygonBoundingBox(inputPath);
            m_AspectStretcher = new AspectStretcher(aspectRatio, m_PolygonRect.center.x);

            // Initialize clipper
            m_OriginalPolygon = new List<List<IntPoint>>(inputPath.Count);
            for (var i = 0; i < inputPath.Count; ++i)
            {
                var srcPath = inputPath[i];
                int numPoints = srcPath.Count;
                var path = new List<IntPoint>(numPoints);
                for (int j = 0; j < numPoints; ++j)
                {
                    // Neutralize the aspect ratio
                    var p = m_AspectStretcher.Stretch(srcPath[j]);
                    path.Add(new IntPoint(p.x * k_FloatToIntScaler, p.y * k_FloatToIntScaler));
                }
                m_OriginalPolygon.Add(path);
            }
            
            // Skip the expensive skeleton calculation if it's not wanted
            if (m_Cache.maxFrustumHeight  < 0)
            {
                State = BakingState.BAKED; // if we don't need skeleton, then we don't need to bake
                return;
            }

            // Don't compute further than what is the theoretical max
            float polygonHalfHeight = Mathf.Min(m_PolygonRect.width / aspectRatio, m_PolygonRect.height) / 2f;
            
            // exact comparison to 0 is intentional!
            if (m_Cache.maxFrustumHeight  == 0 || m_Cache.maxFrustumHeight  > polygonHalfHeight) 
            {
                m_Cache.maxFrustumHeight = polygonHalfHeight; 
            }
            m_Cache.stepSize = m_Cache.maxFrustumHeight;

            // Binary search for state changes so we can compute the skeleton
            m_Cache.offsetter = new ClipperOffset();
            m_Cache.offsetter.AddPaths(m_OriginalPolygon, JoinType.jtMiter, EndType.etClosedPolygon);

            List<List<IntPoint>> solution = new List<List<IntPoint>>();
            m_Cache.offsetter.Execute(ref solution, 0);
            
            m_Cache.solutions = new List<PolygonSolution>();
            m_Cache.solutions.Add(new PolygonSolution
            {
                m_Polygons = solution,
                m_FrustumHeight = 0,
            });

            m_Cache.rightCandidate = new PolygonSolution();
            m_Cache.leftCandidate = new PolygonSolution
            {
                m_Polygons = solution,
                m_FrustumHeight = 0,
            };
            m_Cache.currentFrustumHeight = 0;
            m_Cache.maxCandidate = new List<List<IntPoint>>();
            m_Cache.offsetter.Execute(ref m_Cache.maxCandidate, -1f * m_Cache.maxFrustumHeight * k_FloatToIntScaler);

            m_Cache.bakeTime = 0;
            State = BakingState.BAKING;
            m_BakeProgress = 0;
        }
        
        /// <summary>
        /// Creates shrinkable polygons from input parameters.
        /// The algorithm is divide and conquer. It iteratively shrinks down the input 
        /// polygon towards its shrink directions. If the polygon intersects with itself, 
        /// then we divide the polygon into two polygons at the intersection point, and 
        /// continue the algorithm on these two polygons separately. We need to keep track of
        /// the connectivity information between sub-polygons.
        /// </summary>
        public void BakeConfiner(float maxComputationTimePerFrameInSeconds)
        {
            if (State != BakingState.BAKING) 
                return;
            
            var startTime = Time.realtimeSinceStartup;
            
            while (m_Cache.solutions.Count < 1000)
            {
                bool stateChangeFound = false;
                var numPaths = m_Cache.leftCandidate.m_Polygons.Count;
                var candidate = new List<List<IntPoint>>(numPaths);

                m_Cache.stepSize = Mathf.Min(m_Cache.stepSize, 
                    m_Cache.maxFrustumHeight - m_Cache.leftCandidate.m_FrustumHeight);
#if false
                Debug.Log($"States = {solutions.Count}, "
                          + $"Frustum height = {currentFrustumHeight}, stepSize = {stepSize}");
#endif
                m_Cache.currentFrustumHeight = 
                    m_Cache.leftCandidate.m_FrustumHeight + m_Cache.stepSize;
                if (Math.Abs(m_Cache.currentFrustumHeight - m_Cache.maxFrustumHeight) < 
                    UnityVectorExtensions.Epsilon)
                {
                    candidate = m_Cache.maxCandidate;
                }
                else
                {
                    m_Cache.offsetter.Execute(
                        ref candidate, -1f * m_Cache.currentFrustumHeight * k_FloatToIntScaler);
                }
                stateChangeFound = m_Cache.leftCandidate.StateChanged(in candidate);

                if (stateChangeFound)
                {
                    m_Cache.rightCandidate = new PolygonSolution
                    {
                        m_Polygons = candidate,
                        m_FrustumHeight = m_Cache.currentFrustumHeight,
                    };
                    m_Cache.stepSize = Mathf.Max(m_Cache.stepSize / 2f, k_MinStepSize);
                }
                else
                {
                    m_Cache.leftCandidate = new PolygonSolution
                    {
                        m_Polygons = candidate,
                        m_FrustumHeight = m_Cache.currentFrustumHeight,
                    };

                    // if we have not found right yet, then we don't need to decrease stepsize
                    if (!m_Cache.rightCandidate.IsEmpty)
                    {
                        m_Cache.stepSize = Mathf.Max(m_Cache.stepSize / 2f, k_MinStepSize);
                    }
                }
                
                // if we have a right candidate, and left and right are sufficiently close, 
                // then we have located a state change point
                if (!m_Cache.rightCandidate.IsEmpty && m_Cache.stepSize <= k_MinStepSize)
                {
                    // Add both states: one before the state change and one after
                    m_Cache.solutions.Add(m_Cache.leftCandidate);
                    m_Cache.solutions.Add(m_Cache.rightCandidate);

                    m_Cache.leftCandidate = m_Cache.rightCandidate;
                    m_Cache.rightCandidate = new PolygonSolution();
                    
                    // Back to max step
                    m_Cache.stepSize = m_Cache.maxFrustumHeight;
                }
                else if (m_Cache.rightCandidate.IsEmpty || 
                         m_Cache.leftCandidate.m_FrustumHeight >= m_Cache.maxFrustumHeight)
                {
                    m_Cache.solutions.Add(m_Cache.leftCandidate);
                    break; // stop searching, because we are at the bound
                }

                // Pause after max time per iteration reached
                var elapsedTime = Time.realtimeSinceStartup - startTime;
                if (elapsedTime > maxComputationTimePerFrameInSeconds)
                {
                    m_Cache.bakeTime += elapsedTime;
                    if (m_Cache.bakeTime > m_maxComputationTimeForFullSkeletonBakeInSeconds)
                    {
                        State = BakingState.TIMEOUT; 
                    }

                    m_BakeProgress = m_Cache.leftCandidate.m_FrustumHeight / m_Cache.maxFrustumHeight;
                    return;
                }
            }

            ComputeSkeleton(in m_Cache.solutions);
            m_BakeProgress = 1;
            State = BakingState.BAKED;
        }
        
        private static Rect GetPolygonBoundingBox(in List<List<Vector2>> polygons)
        {
            float minX = float.PositiveInfinity, maxX = float.NegativeInfinity;
            float minY = float.PositiveInfinity, maxY = float.NegativeInfinity;
            for (int i = 0; i < polygons.Count; ++i)
            {
                var path = polygons[i];
                for (int j = 0; j < path.Count; ++j)
                {
                    var p = path[j];
                    minX = Mathf.Min(minX, p.x);
                    maxX = Mathf.Max(maxX, p.x);
                    minY = Mathf.Min(minY, p.y);
                    maxY = Mathf.Max(maxY, p.y);
                }
            }
            return new Rect(minX, minY, Mathf.Max(0, maxX - minX), Mathf.Max(0, maxY - minY));
        }
        
        private void ComputeSkeleton(in List<PolygonSolution> solutions)
        {
            // At each state change point, collect geometry that gets lost over the transition
            var clipper = new Clipper();
            var offsetter = new ClipperOffset();
            for (int i = 1; i < solutions.Count - 1; i += 2)
            {
                var prev = solutions[i];
                var next = solutions[i+1];

                const int padding = 5; // to counteract precision problems - inflates small regions
                double step = padding * k_FloatToIntScaler * (next.m_FrustumHeight - prev.m_FrustumHeight);

                // Grow the larger polygon to inflate marginal regions
                var expandedPrev = new List<List<IntPoint>>();
                offsetter.Clear();
                offsetter.AddPaths(prev.m_Polygons, JoinType.jtMiter, EndType.etClosedPolygon);
                offsetter.Execute(ref expandedPrev, step);

                // Grow the smaller polygon to be a bit bigger than the expanded larger one
                var expandedNext = new List<List<IntPoint>>();
                offsetter.Clear();
                offsetter.AddPaths(next.m_Polygons, JoinType.jtMiter, EndType.etClosedPolygon);
                offsetter.Execute(ref expandedNext, step * 2);

                // Compute the difference - this is the lost geometry
                var solution = new List<List<IntPoint>>();
                clipper.Clear();
                clipper.AddPaths(expandedPrev, PolyType.ptSubject, true);
                clipper.AddPaths(expandedNext, PolyType.ptClip, true);
                clipper.Execute(ClipType.ctDifference, solution, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

                // Add that lost geometry to the skeleton
                if (solution.Count > 0 && solution[0].Count > 0)
                {
                    m_Skeleton.AddRange(solution);
                    if (m_MinFrustumHeightWithBones == float.MaxValue)
                        m_MinFrustumHeightWithBones = next.m_FrustumHeight;
                }
            }
        }
    }
}