﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using static MathSharp.Utils.Helpers;
using OpenTK;
using OpenVector2 = OpenTK.Vector2;
using SysVector2 = System.Numerics.Vector2;

namespace MathSharp.UnitTests
{
    public static class TestHelpers
    {
        public static bool IsNegative(float f) => (BitConverter.SingleToInt32Bits(f) & (1u << 31)) != 0;

        public static unsafe Vector128<T> ForEach<T>(Vector128<T> vector, Func<T, T> transform) where T : unmanaged
        {
            T* pool = stackalloc T[Vector128<T>.Count];

            for (var i = 0; i < Vector128<T>.Count; i++)
            {
                pool[i] = transform(vector.GetElement(i));
            }

            return Unsafe.Read<Vector128<T>>(pool);
        }

        public static bool PerElemCheck(Vector128<float> a, Func<float, bool> check)
        {
            for (var i = 0; i < Vector128<float>.Count; i++)
            {
                if (!check(a.GetElement(i))) return false;
            }

            return true;
        }

        public static bool PerElemCheck(Vector128<float> a, Vector128<float> b, Func<float, float, bool> check)
        {
            for (var i = 0; i < Vector128<float>.Count; i++)
            {
                if (!check(a.GetElement(i), b.GetElement(i))) return false;
            }

            return true;
        }

        public static bool PerElemCheck(Vector128<float> a, Vector128<float> b, Vector128<float> c, Func<float, float, float, bool> check)
        {
            for (var i = 0; i < Vector128<float>.Count; i++)
            {
                if (!check(a.GetElement(i), b.GetElement(i), c.GetElement(i))) return false;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static OpenVector2 ByValToSlowVector2(Vector128<float> vec)
        {
            return new OpenVector2(X(vec), Y(vec));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector3 ByValToSlowVector3(Vector128<float> vec)
        {
            return new Vector3(X(vec), Y(vec), Z(vec));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector4 ByValToSlowVector4(Vector128<float> vec)
        {
            return new Vector4(X(vec), Y(vec), Z(vec), W(vec));
        }

        public static bool AreEqual(Vector128<float> left, Vector128<float> right)
        {
            for (int i = 0; i < Vector128<float>.Count; i++)
            {
                int l = left.AsInt32().GetElement(i);
                int r = right.AsInt32().GetElement(i);

                if (l != r) return false;
            }

            return true;
        }

        public static bool AreEqual(Vector256<double> left, Vector256<double> right)
        {
            for (int i = 0; i < Vector256<double>.Count; i++)
            {
                long l = left.AsInt64().GetElement(i);
                long r = right.AsInt64().GetElement(i);

                if (l != r) return false;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool AreApproxEqual(Vector128<float> left, Vector128<float> right, float tolerance)
        {
            for (int i = 0; i < Vector128<float>.Count; i++)
            {
                var l = left.GetElement(i);
                var r = right.GetElement(i);

                var diff = MathF.Abs(l - r);

                if (diff < tolerance || l.Equals(r))
                {
                    continue;
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool AreEqual(Vector4 left, Vector128<float> right)
            => left.X.Equals(X(right)) && left.Y.Equals(Y(right)) && left.Z.Equals(Z(right)) && left.W.Equals(W(right));

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool AreEqual(Vector3 left, Vector128<float> right)
            => left.X.Equals(X(right)) && left.Y.Equals(Y(right)) && left.Z.Equals(Z(right));

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool AreEqual(OpenVector2 left, Vector128<float> right)
            => left.X.Equals(X(right)) && left.Y.Equals(Y(right));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool AreEqual(Vector128<float> left, OpenVector2 right)
            => right.X.Equals(X(left)) && right.Y.Equals(Y(left));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool AreEqual(SysVector2 left, Vector128<float> right)
            => left.X.Equals(X(right)) && left.Y.Equals(Y(right));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool AreEqual(Vector128<float> left, SysVector2 right)
            => right.X.Equals(X(left)) && right.Y.Equals(Y(left));

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool AreEqual(float left, Vector128<float> right)
            => left.Equals(X(right));

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool AreApproxEqual(float left, float right, float interval = 0.000000001f)
        {
            if (float.IsNaN(left) && float.IsNaN(right))
                return true;

            return Math.Abs(left - right) < interval;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool AreApproxEqual(Vector4 left, Vector128<float> right, float interval = 0.000000001f)
        {
            return AreApproxEqual(left.X, X(right), interval)
                   && AreApproxEqual(left.Y, Y(right), interval)
                   && AreApproxEqual(left.Z, Z(right), interval)
                   && AreApproxEqual(left.W, W(right), interval);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool AreApproxEqual(Vector3 left, Vector128<float> right, float interval = 0.000000001f)
        {
            return AreApproxEqual(left.X, X(right), interval)
                   && AreApproxEqual(left.Y, Y(right), interval)
                   && AreApproxEqual(left.Z, Z(right), interval);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool AreApproxEqual(OpenVector2 left, Vector128<float> right, float interval = 0.000000001f)
        {
            return AreApproxEqual(left.X, X(right), interval)
                   && AreApproxEqual(left.Y, Y(right), interval);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool AreApproxEqual(float left, Vector128<float> right)
        {
            return AreApproxEqual(left, X(right));
        }

        

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool AreAllEqual(bool[] bools, Vector128<int> boolVecZeroIsFalseNotZeroIsTrue)
        {
            for (var i = 0; i < 4; i++)
            {
                if (bools[i] && boolVecZeroIsFalseNotZeroIsTrue.GetElement(i) != -1
                    || !bools[i] && boolVecZeroIsFalseNotZeroIsTrue.GetElement(i) != 0)
                    return false;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool AreAllNotEqual(bool[] bools, Vector128<int> boolVecZeroIsFalseNotZeroIsTrue)
        {
            for (var i = 0; i < 4; i++)
            {
                if (bools[i] && boolVecZeroIsFalseNotZeroIsTrue.GetElement(i) != 0
                    || !bools[i] && boolVecZeroIsFalseNotZeroIsTrue.GetElement(i) != -1)
                    return false;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool AreAllEqual(bool[] bools, Vector256<long> boolVecZeroIsFalseNotZeroIsTrue)
        {
            for (var i = 0; i < 4; i++)
            {
                if (bools[i] && boolVecZeroIsFalseNotZeroIsTrue.GetElement(i) != -1
                    || !bools[i] && boolVecZeroIsFalseNotZeroIsTrue.GetElement(i) != 0)
                    return false;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool AreAllNotEqual(bool[] bools, Vector256<long> boolVecZeroIsFalseNotZeroIsTrue)
        {
            for (var i = 0; i < 4; i++)
            {
                if (bools[i] && boolVecZeroIsFalseNotZeroIsTrue.GetElement(i) != 0
                    || !bools[i] && boolVecZeroIsFalseNotZeroIsTrue.GetElement(i) != -1)
                    return false;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool AreEqual(Vector4d left, Vector256<double> right)
            => left.X.Equals(X(right)) && left.Y.Equals(Y(right)) && left.Z.Equals(Z(right)) && left.W.Equals(W(right));

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool AreEqual(Vector3d left, Vector256<double> right)
            => left.X.Equals(X(right)) && left.Y.Equals(Y(right)) && left.Z.Equals(Z(right));

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool AreEqual(Vector2d left, Vector256<double> right)
            => left.X.Equals(X(right)) && left.Y.Equals(Y(right));

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool AreEqual(double left, Vector256<double> right)
            => left.Equals(X(right));

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool AreApproxEqual(double left, double right, double interval = 0.000000001d)
        {
            if (double.IsNaN(left) && double.IsNaN(right))
                return true;

            return Math.Abs(left - right) < interval;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool AreApproxEqual(Vector4d left, Vector256<double> right, double interval = 0.000000001d)
        {
            return AreApproxEqual(left.X, X(right), interval)
                   && AreApproxEqual(left.Y, Y(right), interval)
                   && AreApproxEqual(left.Z, Z(right), interval)
                   && AreApproxEqual(left.W, W(right), interval);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool AreApproxEqual(Vector3d left, Vector256<double> right, double interval = 0.000000001d)
        {
            return AreApproxEqual(left.X, X(right), interval)
                   && AreApproxEqual(left.Y, Y(right), interval)
                   && AreApproxEqual(left.Z, Z(right), interval);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool AreApproxEqual(Vector2d left, Vector256<double> right, double interval = 0.000000001d)
        {
            return AreApproxEqual(left.X, X(right), interval)
                   && AreApproxEqual(left.Y, Y(right), interval);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool AreApproxEqual(double left, Vector256<double> right)
        {
            return AreApproxEqual(left, X(right));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector2d ByValToSlowVector2d(Vector256<double> vec)
        {
            return new Vector2d(X(vec), Y(vec));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector3d ByValToSlowVector3d(Vector256<double> vec)
        {
            return new Vector3d(X(vec), Y(vec), Z(vec));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector4d ByValToSlowVector4d(Vector256<double> vec)
        {
            return new Vector4d(X(vec), Y(vec), Z(vec), W(vec));
        }
    }
}
