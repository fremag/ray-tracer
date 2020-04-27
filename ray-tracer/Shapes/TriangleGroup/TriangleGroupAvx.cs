using System.Collections.Generic;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace ray_tracer.Shapes.TriangleGroup
{
    public class TriangleGroupAvx : AbstractTriangleGroupOptim
    {
        public TriangleGroupAvx(List<Triangle> triangles, in bool keepParent = false)
            : base(triangles, keepParent)
        {
            
        }

        public TriangleGroupAvx(Group triangleGroup) : base(triangleGroup)
        {
        }

        public TriangleGroupAvx()
        {
            
        }

        protected override AbstractTriangleGroup GetSubGroup(List<Triangle> triangles, bool keepParent)
        {
            return new TriangleGroupAvx(triangles, keepParent);
        }

        protected override unsafe void IntersectAll(ref Tuple origin, ref Tuple rayDir, Intersections intersections)
        {
            var count = p1_X.Length;
            var dirCrossE2_X = stackalloc float[count];
            var dirCrossE2_Y = stackalloc float[count];
            var dirCrossE2_Z = stackalloc float[count];
            var det = stackalloc float[count];

            var vRayDirX = Vector256.Create((float) rayDir.X);
            var vRayDirY = Vector256.Create((float) rayDir.Y);
            var vRayDirZ = Vector256.Create((float) rayDir.Z);

            var skipAll = true;
            var skip = stackalloc float[count];
            var epsilon = (float) Helper.Epsilon;
            var epsPos = Vector256.Create(epsilon);
            var epsNeg = Vector256.Create(-epsilon);

            fixed (float* ptre1x = e1_X)
            fixed (float* ptre1y = e1_Y)
            fixed (float* ptre1z = e1_Z)
            fixed (float* ptre2x = e2_X)
            fixed (float* ptre2y = e2_Y)
            fixed (float* ptre2z = e2_Z)
            {
                for (var i = 0; i < count; i += Size)
                {
                    var e1x = Avx.LoadVector256(ptre1x + i);
                    var e1y = Avx.LoadVector256(ptre1y + i);
                    var e1z = Avx.LoadVector256(ptre1z + i);

                    var e2x = Avx.LoadVector256(ptre2x + i);
                    var e2y = Avx.LoadVector256(ptre2y + i);
                    var e2z = Avx.LoadVector256(ptre2z + i);

                    var crossX = Avx.Subtract(Avx.Multiply(vRayDirY, e2z), Avx.Multiply(vRayDirZ, e2y));
                    var crossY = Avx.Subtract(Avx.Multiply(vRayDirZ, e2x), Avx.Multiply(vRayDirX, e2z));
                    var crossZ = Avx.Subtract(Avx.Multiply(vRayDirX, e2y), Avx.Multiply(vRayDirY, e2x));

                    var vDet = Avx.Add(Avx.Multiply(e1x, crossX), Avx.Add(Avx.Multiply(e1y, crossY), Avx.Multiply(e1z, crossZ)));
                    Avx.Store(det + i, vDet);
                    Avx.Store(dirCrossE2_X + i, crossX);
                    Avx.Store(dirCrossE2_Y + i, crossY);
                    Avx.Store(dirCrossE2_Z + i, crossZ);

                    var v1 = Avx.Compare(vDet, epsNeg, FloatComparisonMode.OrderedLessThanNonSignaling);
                    var v2 = Avx.Compare(vDet, epsPos, FloatComparisonMode.OrderedGreaterThanNonSignaling);
                    var result = Avx.And(v1, v2);
                    Avx.Store(skip + i, result);
                    skipAll &= !Avx.TestZ(result, result);
                }
            }

            if (skipAll)
            {
                return;
            }

            var u = stackalloc float[count];
            var f = stackalloc float[count];
            var p1ToOrigin_X = stackalloc float[count];
            var p1ToOrigin_Y = stackalloc float[count];
            var p1ToOrigin_Z = stackalloc float[count];
            var vOriginX = Vector256.Create((float) origin.X);
            var vOriginY = Vector256.Create((float) origin.Y);
            var vOriginZ = Vector256.Create((float) origin.Z);

            var vOne = Vector256.Create(1f);
            var vZero = Vector256.Create(0f);
            
            var v = stackalloc float[count];
            var originCrossE1_X = stackalloc float[count];
            var originCrossE1_Y = stackalloc float[count];
            var originCrossE1_Z = stackalloc float[count];

            fixed (float* ptrp1x = p1_X)
            fixed (float* ptrp1y = p1_Y)
            fixed (float* ptrp1z = p1_Z)
            fixed (float* ptre1x = e1_X)
            fixed (float* ptre1y = e1_Y)
            fixed (float* ptre1z = e1_Z)
            {
                for (var i = 0; i < count; i += Size)
                {
                    var vSkip = Avx.LoadVector256(skip + i);
                    var skipVector = ! Avx.TestZ(vSkip, vSkip);
                    if (skipVector)
                    {
                        continue;
                    }

                    var p1x = Avx.LoadVector256(ptrp1x + i);
                    var p1y = Avx.LoadVector256(ptrp1y + i);
                    var p1z = Avx.LoadVector256(ptrp1z + i);
                    
                    var vp1ToOrigin_X = Avx.Subtract(vOriginX, p1x);
                    var vp1ToOrigin_Y = Avx.Subtract(vOriginY, p1y);
                    var vp1ToOrigin_Z = Avx.Subtract(vOriginZ, p1z);
                    
                    var vdirCrossE2_X = Avx.LoadVector256(dirCrossE2_X + i);
                    var vdirCrossE2_Y = Avx.LoadVector256(dirCrossE2_Y + i);
                    var vdirCrossE2_Z = Avx.LoadVector256(dirCrossE2_Z + i);
                    
                    var uuX = Avx.Multiply(vp1ToOrigin_X, vdirCrossE2_X);
                    var uuY = Avx.Multiply(vp1ToOrigin_Y, vdirCrossE2_Y);
                    var uuZ = Avx.Multiply(vp1ToOrigin_Z, vdirCrossE2_Z);
                    var uu = Avx.Add(uuX, Avx.Add(uuY, uuZ));

                    var vDet = Avx.LoadVector256(det + i);
                    var vF = Avx.Divide(vOne, vDet);
                    var vU = Avx.Multiply(vF, uu);

                    var vCmpZero = Avx.Compare(vU, vZero, FloatComparisonMode.OrderedLessThanNonSignaling);
                    var vCmpOne = Avx.Compare(vU, vOne, FloatComparisonMode.OrderedGreaterThanNonSignaling);
                    vSkip = Avx.Or(vCmpOne, vCmpZero);
                    
                    Avx.Store(skip+i, vSkip);
                    Avx.Store(f+i, vF);
                    Avx.Store(u+i, vU);
                    Avx.Store(p1ToOrigin_X + i, vp1ToOrigin_X);
                    Avx.Store(p1ToOrigin_Y + i, vp1ToOrigin_Y);
                    Avx.Store(p1ToOrigin_Z + i, vp1ToOrigin_Z);

                    var e1x = Avx.LoadVector256(ptre1x + i);
                    var e1y = Avx.LoadVector256(ptre1y + i);
                    var e1z = Avx.LoadVector256(ptre1z + i);
                    var p1ToOriginX = Avx.LoadVector256(p1ToOrigin_X + i);
                    var p1ToOriginY = Avx.LoadVector256(p1ToOrigin_Y + i);
                    var p1ToOriginZ = Avx.LoadVector256(p1ToOrigin_Z + i);

                    var vx1 = Avx.Multiply(p1ToOriginY, e1z);
                    var vx2 = Avx.Multiply(p1ToOriginZ, e1y);
                    var vX = Avx.Subtract(vx1, vx2);
                    Avx.Store(originCrossE1_X + i, vX);

                    var vy1 = Avx.Multiply(p1ToOriginZ, e1x);
                    var vy2 = Avx.Multiply(p1ToOriginX, e1z);
                    var vY = Avx.Subtract(vy1, vy2);
                    Avx.Store(originCrossE1_Y + i, vY);

                    var vz1 = Avx.Multiply(p1ToOriginX, e1y);
                    var vz2 = Avx.Multiply(p1ToOriginY, e1x);
                    var vZ = Avx.Subtract(vz1, vz2);
                    Avx.Store(originCrossE1_Z + i, vZ);

                    var vOriginCrossE1_X = Avx.LoadVector256(originCrossE1_X + i);
                    var vOriginCrossE1_Y = Avx.LoadVector256(originCrossE1_Y + i);
                    var vOriginCrossE1_Z = Avx.LoadVector256(originCrossE1_Z + i);
                    
                    var vf = Avx.LoadVector256(f + i);
                    var vvX = Avx.Multiply(vRayDirX, vOriginCrossE1_X);
                    var vvY = Avx.Multiply(vRayDirY, vOriginCrossE1_Y);
                    var vvZ = Avx.Multiply(vRayDirZ, vOriginCrossE1_Z);
                    var vv = Avx.Multiply(vf, Avx.Add(vvX, Avx.Add(vvY, vvZ)));
                    Avx.Store(v + i, vv);
                }
            }
            
            for (var i = 0; i < Triangles.Count; i++)
            {
                if (float.IsNaN(skip[i]) || v[i] < 0 || (u[i] + v[i]) > 1)
                {
                    continue;
                }

                var t = f[i] * (e2_X[i] * originCrossE1_X[i] + e2_Y[i] * originCrossE1_Y[i] + e2_Z[i] * originCrossE1_Z[i]);
                var intersection = new Intersection(t, Triangles[i] , u[i], v[i]);
                intersections.Add(intersection);
            }
        }
    }
}