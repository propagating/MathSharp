﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using MathSharp.Constants;
using static MathSharp.Utils.Helpers;

namespace MathSharp
{
    

    // The bane of every yr11's existence
    // TRIG
    public static partial class Vector
    {
        private static readonly Vector128<float> SinCoefficient0 = Vector128.Create(-0.16666667f, +0.0083333310f, -0.00019840874f, +2.7525562e-06f);
        private static readonly Vector128<float> SinCoefficient1 = Vector128.Create(-2.3889859e-08f, -0.16665852f, +0.0083139502f, -0.00018524670f);
        private const float SinCoefficient1Scalar = -2.3889859e-08f;
        private static readonly Vector128<float> SinCoefficient1Broadcast = Vector128.Create(SinCoefficient1Scalar);

        /// <summary>
        /// Calculates sine for each element of a vector
        /// </summary>
        /// <param name="vector">The vector to calculate the sine of</param>
        /// <returns>A new vector, where each element is the sine of <paramref name="vector"/></returns>
        [MethodImpl(MaxOpt)]
        public static Vector128<float> Sin(Vector128<float> vector)
        {
            if (Sse.IsSupported)
            {
                Vector128<float> vec = Mod2Pi(vector);

                Vector128<float> sign = ExtractSign(vec);
                Vector128<float> tmp = Or(SingleConstants.Pi, sign); // Pi with the sign from vector

                Vector128<float> abs = AndNot(sign, vec); // Gets the absolute of vector

                Vector128<float> neg = Subtract(tmp, vec);

                Vector128<float> comp = CompareLessThanOrEqual(abs, SingleConstants.PiDiv2);
                vec = Select(comp, vec, neg);

                Vector128<float> vectorSquared = Square(vec);

                // Polynomial approx
                Vector128<float> sc0 = SinCoefficient0;

                Vector128<float> constants = SinCoefficient1Broadcast;
                Vector128<float> result = FastMultiplyAdd(constants, vectorSquared, FillWithW(sc0));

                constants = FillWithZ(sc0);
                result = FastMultiplyAdd(result, vectorSquared, constants);

                constants = FillWithY(sc0);
                result = FastMultiplyAdd(result, vectorSquared, constants);

                constants = FillWithX(sc0);
                result = FastMultiplyAdd(result, vectorSquared, constants);

                result = FastMultiplyAdd(result, vectorSquared, SingleConstants.One);

                result = Multiply(result, vec);

                return result;
            }

            return SoftwareFallback(vector);

            static Vector128<float> SoftwareFallback(Vector128<float> vector)
            {
                return Vector128.Create(
                    MathF.Sin(X(vector)),
                    MathF.Sin(Y(vector)),
                    MathF.Sin(Z(vector)),
                    MathF.Sin(W(vector))
                );
            }
        }

        /// <summary>
        /// Calculates sine for each element of a vector, with potentially reduced accuracy but better performance
        /// </summary>
        /// <param name="vector">The vector to calculate the sine of</param>
        /// <returns>A new vector, where each element is the sine of <paramref name="vector"/></returns>
        [MethodImpl(MaxOpt)]
        public static Vector128<float> SinApprox(Vector128<float> vector)
        {
            if (Sse.IsSupported)
            {
                Vector128<float> vec = Mod2Pi(vector);

                Vector128<float> sign = ExtractSign(vec);
                Vector128<float> tmp = Or(SingleConstants.Pi, sign); // Pi with the sign from vector

                Vector128<float> abs = AndNot(sign, vec); // Gets the absolute of vector

                Vector128<float> neg = Subtract(tmp, vec);

                Vector128<float> comp = CompareLessThanOrEqual(abs, SingleConstants.PiDiv2);
                vec = Select(comp, vec, neg);

                Vector128<float> vectorSquared = Square(vec);

                // Fast polynomial approx
                var sc1 = SinCoefficient1;

                var constants = FillWithW(sc1);
                var result = FastMultiplyAdd(constants, vectorSquared, FillWithZ(sc1));

                constants = FillWithY(sc1);
                result = FastMultiplyAdd(result, vectorSquared, constants);

                result = FastMultiplyAdd(result, vectorSquared, SingleConstants.One);

                result = Multiply(result, vec);

                return result;
            }

            return Sin(vector);
        }


        private static readonly Vector128<float> CosCoefficient0 = Vector128.Create(-0.5f, +0.041666638f, -0.0013888378f, +2.4760495e-05f);
        private static readonly Vector128<float> CosCoefficient1 = Vector128.Create(-2.6051615e-07f, -0.49992746f, +0.041493919f, -0.0012712436f);
        private const float CosCoefficient1Scalar = -2.6051615e-07f;

        /// <summary>
        /// Calculates cosine for each element of a vector
        /// </summary>
        /// <param name="vector">The vector to calculate the cosine of</param>
        /// <returns>A new vector, where each element is the cosine of <paramref name="vector"/></returns>
        [MethodImpl(MaxOpt)]
        public static Vector128<float> Cos(Vector128<float> vector)
        {
            if (Sse.IsSupported)
            {
                Vector128<float> vec = Mod2Pi(vector);

                Vector128<float> sign = ExtractSign(vec);
                Vector128<float> tmp = Or(SingleConstants.Pi, sign); // Pi with the sign from vector

                Vector128<float> abs = AndNot(sign, vec); // Gets the absolute of vector

                Vector128<float> neg = Subtract(tmp, vec);

                Vector128<float> comp = CompareLessThanOrEqual(abs, SingleConstants.PiDiv2);

                vec = Select(comp, vec, neg);

                Vector128<float> vectorSquared = Square(vec);

                vec = Select(comp, SingleConstants.One, SingleConstants.NegativeOne);

                // Polynomial approx
                Vector128<float> cc0 = CosCoefficient0;

                Vector128<float> constants = Vector128.Create(CosCoefficient1Scalar);
                Vector128<float> result = FastMultiplyAdd(constants, vectorSquared, FillWithW(cc0));

                constants = FillWithZ(cc0);
                result = FastMultiplyAdd(result, vectorSquared, constants);

                constants = FillWithY(cc0);
                result = FastMultiplyAdd(result, vectorSquared, constants);

                constants = FillWithX(cc0);
                result = FastMultiplyAdd(result, vectorSquared, constants);

                result = FastMultiplyAdd(result, vectorSquared, SingleConstants.One);

                result = Multiply(result, vec);

                return result;
            }

            return SoftwareFallback(vector);

            static Vector128<float> SoftwareFallback(Vector128<float> vector)
            {
                return Vector128.Create(
                    MathF.Cos(X(vector)),
                    MathF.Cos(Y(vector)),
                    MathF.Cos(Z(vector)),
                    MathF.Cos(W(vector))
                );
            }
        }

        /// <summary>
        /// Calculates cosine for each element of a vector, with potentially reduced accuracy but better performance
        /// </summary>
        /// <param name="vector">The vector to calculate the cosine of</param>
        /// <returns>A new vector, where each element is the cosine of <paramref name="vector"/></returns>
        [MethodImpl(MaxOpt)]
        public static Vector128<float> CosApprox(Vector128<float> vector)
        {
            if (Sse.IsSupported)
            {
                Vector128<float> vec = Mod2Pi(vector);

                Vector128<float> sign = ExtractSign(vec);
                Vector128<float> tmp = Or(SingleConstants.Pi, sign); // Pi with the sign from vector

                Vector128<float> abs = AndNot(sign, vec); // Gets the absolute of vector

                Vector128<float> neg = Subtract(tmp, vec);

                Vector128<float> comp = CompareLessThanOrEqual(abs, SingleConstants.PiDiv2);

                vec = Select(comp, vec, neg);

                Vector128<float> vectorSquared = Square(vec);

                vec = Select(comp, SingleConstants.One, SingleConstants.NegativeOne);

                // Fast polynomial approx
                var cc1 = CosCoefficient1;

                var constants = FillWithW(cc1);
                var result = FastMultiplyAdd(constants, vectorSquared, FillWithZ(cc1));

                constants = FillWithY(cc1);
                result = FastMultiplyAdd(result, vectorSquared, constants);

                result = FastMultiplyAdd(result, vectorSquared, SingleConstants.One);

                result = Multiply(result, vec);

                return result;
            }

            return Cos(vector);
        }

        private static readonly Vector128<float> TanCoefficients0 = Vector128.Create(1.0f, -4.667168334e-1f, 2.566383229e-2f, -3.118153191e-4f);
        private static readonly Vector128<float> TanCoefficients1 = Vector128.Create(4.981943399e-7f, -1.333835001e-1f, 3.424887824e-3f, -1.786170734e-5f);
        private static readonly Vector128<float> TanConstants = Vector128.Create(1.570796371f, 6.077100628e-11f, 0.000244140625f, 0.63661977228f);


        /// <summary>
        /// Calculates tangent of each element of a vector
        /// </summary>
        /// <param name="vector">The vector to calculate the tangent of</param>
        /// <returns>A new vector, where each element is the tangent of <paramref name="vector"/></returns>
        [MethodImpl(MaxOpt)]
        public static Vector128<float> Tan(Vector128<float> vector)
        {
            if (Sse.IsSupported)
            {
                var twoDivPi = FillWithW(TanConstants);

                var tc0 = FillWithX(TanConstants);
                var tc1 = FillWithY(TanConstants);
                var epsilon = FillWithZ(TanConstants);

                var va = Multiply(vector, twoDivPi);
                va = Round(va);

                var vc = FastNegateMultiplyAdd(va, tc0, vector);

                var vb = Abs(va);

                vc = FastNegateMultiplyAdd(va, tc1, vc);

                vb = ConvertToInt32(vb).AsSingle();

                var vc2 = Square(vc);

                var t7 = FillWithW(TanCoefficients1);
                var t6 = FillWithZ(TanCoefficients1);
                var t4 = FillWithX(TanCoefficients1);
                var t3 = FillWithW(TanCoefficients0);
                var t5 = FillWithY(TanCoefficients1);
                var t2 = FillWithZ(TanCoefficients0);
                var t1 = FillWithY(TanCoefficients0);
                var t0 = FillWithX(TanCoefficients0);

                var vbIsEven = And(vb, SingleConstants.Epsilon).AsInt32();
                vbIsEven = CompareEqual(vbIsEven, Vector128<int>.Zero);

                var n = FastMultiplyAdd(vc2, t7, t6);
                var d = FastMultiplyAdd(vc2, t4, t3);
                n = FastMultiplyAdd(vc2, n, t5);
                d = FastMultiplyAdd(vc2, d, t2);
                n = Multiply(vc2, n);
                d = FastMultiplyAdd(vc2, d, t1);
                n = FastMultiplyAdd(vc, n, vc);

                var nearZero = InBounds(vc, epsilon);

                d = FastMultiplyAdd(vc2, d, t0);

                n = Select(nearZero, vc, n);
                d = Select(nearZero, SingleConstants.One, d);

                var r0 = Negate(n);
                var r1 = Divide(n, d);
                r0 = Divide(d, r0);

                var isZero = CompareEqual(vector, Vector128<float>.Zero);

                var result = Select(vbIsEven, r1, r0);

                result = Select(isZero, Vector128<float>.Zero, result);

                return result;
            }

            return SoftwareFallback(vector);

            static Vector128<float> SoftwareFallback(Vector128<float> vector)
            {
                return Vector128.Create(
                    MathF.Tan(X(vector)),
                    MathF.Tan(Y(vector)),
                    MathF.Tan(Z(vector)),
                    MathF.Tan(W(vector))
                );
            }
        }

        

        private static readonly Vector128<float> TanEstCoefficients = Vector128.Create(2.484f, -1.954923183e-1f, 2.467401101f, ScalarSingleConstants.OneDivPi);


        /// <summary>
        /// Calculates tangent for each element of a vector, with potentially reduced accuracy but better performance
        /// </summary>
        /// <param name="vector">The vector to calculate the tangent of</param>
        /// <returns>A new vector, where each element is the tangent of <paramref name="vector"/></returns>
        [MethodImpl(MaxOpt)]
        public static Vector128<float> TanApprox(Vector128<float> vector)
        {
            if (Sse.IsSupported)
            {
                var oneDivPi = FillWithW(TanEstCoefficients);

                var v1 = Multiply(vector, oneDivPi);
                v1 = Round(v1);

                v1 = FastNegateMultiplyAdd(SingleConstants.Pi, v1, vector);

                var t0 = FillWithX(TanEstCoefficients);
                var t1 = FillWithY(TanEstCoefficients);
                var t2 = FillWithZ(TanEstCoefficients);

                var v2T2 = FastNegateMultiplyAdd(v1, v1, t2);
                var v2 = Square(v1);
                var v1T0 = Multiply(v1, t0);
                var v1T1 = Multiply(v1, t1);

                var d = ReciprocalApprox(v2T2);
                var n = FastMultiplyAdd(v2, v1T1, v1T0);

                return Multiply(n, d);
            }

            return Tan(vector);
        }


        /// <summary>
        /// Calculates sine and cosine for each element of a vector
        /// </summary>
        /// <param name="vector">The vector to calculate the sine and cosine of</param>
        /// <param name="sin">A new vector, where each element is the sine of <paramref name="vector"/></param>
        /// <param name="cos">A new vector, where each element is the cosine of <paramref name="vector"/></param>
        [MethodImpl(MaxOpt)]
        public static void SinCos(Vector128<float> vector, out Vector128<float> sin, out Vector128<float> cos)
        {
            if (Sse.IsSupported)
            {
                Vector128<float> vec = Mod2Pi(vector);

                Vector128<float> sign = ExtractSign(vec);
                Vector128<float> tmp = Or(SingleConstants.Pi, sign); // Pi with the sign from vector

                Vector128<float> abs = AndNot(sign, vec); // Gets the absolute of vector

                Vector128<float> neg = Subtract(tmp, vec);

                Vector128<float> comp = CompareLessThanOrEqual(abs, SingleConstants.PiDiv2);

                vec = Select(comp, vec, neg);

                Vector128<float> vectorSquared = Square(vec);

                var cosVec = Select(comp, SingleConstants.One, SingleConstants.NegativeOne);


                // Polynomial approx
                Vector128<float> sc0 = SinCoefficient0;

                Vector128<float> constants = Vector128.Create(SinCoefficient1Scalar);
                Vector128<float> result = FastMultiplyAdd(constants, vectorSquared, FillWithW(sc0));

                constants = FillWithZ(sc0);
                result = FastMultiplyAdd(result, vectorSquared, constants);

                constants = FillWithY(sc0);
                result = FastMultiplyAdd(result, vectorSquared, constants);

                constants = FillWithX(sc0);
                result = FastMultiplyAdd(result, vectorSquared, constants);

                result = FastMultiplyAdd(result, vectorSquared, SingleConstants.One);

                result = Multiply(result, vec);

                sin = result;

                // Polynomial approx
                Vector128<float> cc0 = CosCoefficient0;

                constants = Vector128.Create(CosCoefficient1Scalar);
                result = FastMultiplyAdd(constants, vectorSquared, FillWithW(cc0));

                constants = FillWithZ(cc0);
                result = FastMultiplyAdd(result, vectorSquared, constants);

                constants = FillWithY(cc0);
                result = FastMultiplyAdd(result, vectorSquared, constants);

                constants = FillWithX(cc0);
                result = FastMultiplyAdd(result, vectorSquared, constants);

                result = FastMultiplyAdd(result, vectorSquared, SingleConstants.One);

                result = Multiply(result, cosVec);

                cos = result;

                return;
            }

            SoftwareFallback(vector, out sin, out cos);

            static void SoftwareFallback(Vector128<float> vector, out Vector128<float> sin, out Vector128<float> cos)
            {
                sin = Sin(vector);
                cos = Cos(vector);
            }
        }


        /// <summary>
        /// Calculates sine and cosine for each element of a vector, with potentially reduced accuracy but better performance
        /// </summary>
        /// <param name="vector">The vector to calculate the sine and cosine of</param>
        /// <param name="sin">A new vector, where each element is the sine of <paramref name="vector"/></param>
        /// <param name="cos">A new vector, where each element is the cosine of <paramref name="vector"/></param>
        public static void SinCosApprox(Vector128<float> vector, out Vector128<float> sin, out Vector128<float> cos)
        {
            if (Sse.IsSupported)
            {
                Vector128<float> vec = Mod2Pi(vector);

                Vector128<float> sign = ExtractSign(vec);
                Vector128<float> tmp = Or(SingleConstants.Pi, sign); // Pi with the sign from vector

                Vector128<float> abs = AndNot(sign, vec); // Gets the absolute of vector

                Vector128<float> neg = Subtract(tmp, vec);

                Vector128<float> comp = CompareLessThanOrEqual(abs, SingleConstants.PiDiv2);

                vec = Select(comp, vec, neg);
                Vector128<float> vectorSquared = Square(vec);

                var cosVec = Select(comp, SingleConstants.One, SingleConstants.NegativeOne);


                // Fast polynomial approx
                var sc1 = SinCoefficient1;

                var constants = FillWithW(sc1);
                var result = FastMultiplyAdd(constants, vectorSquared, FillWithZ(sc1));

                constants = FillWithY(sc1);
                result = FastMultiplyAdd(result, vectorSquared, constants);

                result = FastMultiplyAdd(result, vectorSquared, SingleConstants.One);

                result = Multiply(result, vec);

                sin = result;

                // Fast polynomial approx
                var cc1 = CosCoefficient1;

                constants = FillWithW(cc1);
                result = FastMultiplyAdd(constants, vectorSquared, FillWithZ(cc1));

                constants = FillWithY(cc1);
                result = FastMultiplyAdd(result, vectorSquared, constants);

                result = FastMultiplyAdd(result, vectorSquared, SingleConstants.One);

                result = Multiply(result, cosVec);

                cos = result;

                return;
            }

            SinCos(vector, out sin, out cos);
        }

        private static readonly Vector128<float> ATan2Constants = Vector128.Create(
                ScalarSingleConstants.Pi,
                ScalarSingleConstants.PiDiv2,
                ScalarSingleConstants.PiDiv4,
                ScalarSingleConstants.Pi * 3.0f / 4.0f
        );


        /// <summary>
        /// Calculates atan2 for each element of 2 vectors
        /// </summary>
        /// <param name="left">The left vector to calculate atan2 with</param>
        /// <param name="right">The right vector to calculate atan2 with</param>
        /// <returns>A new vector, where each element is the atan2 of <paramref name="left"/> with <paramref name="right"/></returns>
        public static Vector128<float> ATan2(Vector128<float> left, Vector128<float> right)
        {
            if (Sse.IsSupported)
            {
                var aTanResultValid = SingleConstants.AllBitsSet;

                var pi = SingleConstants.Pi;
                var piDiv2 = SingleConstants.PiDiv2;
                var piDiv4 = SingleConstants.PiDiv4;
                var threePiDiv4 = SingleConstants.ThreePiDiv4;

                var yEqualsZero = CompareEqual(left, SingleConstants.Zero);
                var xEqualsZero = CompareEqual(right, SingleConstants.Zero);
                var rightIsPositive = ExtractSign(right);
                rightIsPositive = CompareEqual(rightIsPositive.AsInt32(), Vector128<int>.Zero).AsSingle();
                var yEqualsInfinity = IsInfinite(left);
                var xEqualsInfinity = IsInfinite(right);

                var ySign = And(left, SingleConstants.NegativeZero);
                pi = Or(pi, ySign);
                piDiv2 = Or(piDiv2, ySign);
                piDiv4 = Or(piDiv4, ySign);
                threePiDiv4 = Or(threePiDiv4, ySign);

                var r1 = Select(rightIsPositive, ySign, pi);
                var r2 = Select(xEqualsZero, piDiv2, aTanResultValid);
                var r3 = Select(yEqualsZero, r1, r2);
                var r4 = Select(rightIsPositive, piDiv4, threePiDiv4);
                var r5 = Select(xEqualsInfinity, r4, piDiv2);
                var result = Select(yEqualsInfinity, r5, r3);
                aTanResultValid = CompareEqual(result.AsInt32(), aTanResultValid.AsInt32()).AsSingle();

                var v = Divide(left, right);

                var r0 = ATan(v);

                r1 = Select(rightIsPositive, SingleConstants.NegativeZero, pi);
                r2 = Add(r0, r1);

                return Select(aTanResultValid, r2, result);
            }

            return SoftwareFallback(left, right);

            static Vector128<float> SoftwareFallback(Vector128<float> left, Vector128<float> right)
            {
                return Vector128.Create(
                    MathF.Atan2(X(left), X(right)),
                    MathF.Atan2(Y(left), Y(right)),
                    MathF.Atan2(Z(left), Z(right)),
                    MathF.Atan2(W(left), W(right))
                );
            }
        }

        public static readonly Vector128<float> ATanCoefficients0 = Vector128.Create(-0.3333314528f, +0.1999355085f, -0.1420889944f, +0.1065626393f);
        public static readonly Vector128<float> ATanCoefficients1 = Vector128.Create(-0.0752896400f, +0.0429096138f, -0.0161657367f, +0.0028662257f);

        /// <summary>
        /// Calculates atan for each element of a vector
        /// </summary>
        /// <param name="vector">The vector to calculate the atan of</param>
        /// <returns>A new vector, where each element is the atan of <paramref name="vector"/></returns>
        public static Vector128<float> ATan(Vector128<float> vector)
        {
            var abs = Abs(vector);
            var inv = Divide(SingleConstants.One, vector);
            var comp = CompareGreaterThan(vector, SingleConstants.One);
            var sign = Select(comp, SingleConstants.One, SingleConstants.NegativeOne);

            comp = CompareLessThanOrEqual(abs, SingleConstants.One);

            sign = Select(comp, SingleConstants.Zero, sign);

            var vec = Select(comp, vector, inv);

            var vecSquared = Square(vec);

            var tc1 = ATanCoefficients1;

            var constants1 = FillWithZ(tc1);

            var result = FastMultiplyAdd(FillWithW(tc1), vecSquared, constants1);

            constants1 = FillWithY(tc1);

            result = FastMultiplyAdd(result, vecSquared, constants1);

            constants1 = FillWithX(tc1);

            result = FastMultiplyAdd(result, vecSquared, constants1);

            var tC0 = ATanCoefficients0;
            constants1 = FillWithW(tC0);

            result = FastMultiplyAdd(result, vecSquared, constants1);

            constants1 = FillWithZ(tC0);

            result = FastMultiplyAdd(result, vecSquared, constants1);

            constants1 = FillWithY(tC0);

            result = FastMultiplyAdd(result, vecSquared, constants1);

            constants1 = FillWithX(tC0);

            result = FastMultiplyAdd(result, vecSquared, constants1);

            result = FastMultiplyAdd(result, vecSquared, SingleConstants.One);

            result = Multiply(result, vec);

            var result1 = FastMultiplySubtract(sign, SingleConstants.PiDiv2, result);

            comp = CompareEqual(sign, SingleConstants.Zero);
            result = Select(comp, result, result1);
            return result;
        }
    }
}
