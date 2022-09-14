// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osuTK;

namespace osu.Framework.Graphics.UserInterface.Graph.Interpolation.Interpolators
{
    public class WaveInterpolator : InterpolatorBase, IHasName, IHasCustomDerivativeExtrema, IIntegrableInterpolator, IInvertibleInterpolator
    {
        public string Name => "Wave";

        public double[] ExtremaPositions => new[] { 0, 0.5, 1 };

        public override double GetInterpolation(double t)
        {
            double cycles = Math.Round((1 - Math.Abs(MathHelper.Clamp(P, -1, 1))) * 50) + 0.5;

            return P < 0 ? triangleWave(t, 1 / cycles) : sineWave(t * cycles * 2 * Math.PI);
        }

        public double GetIntegral(double t1, double t2)
        {
            double cycles = Math.Round((1 - Math.Abs(MathHelper.Clamp(P, -1, 1))) * 50) + 0.5;

            return P < 0 ? triangleWaveIntegral(t2, 1 / cycles) - triangleWaveIntegral(t1, 1 / cycles) : sineWavePrimitive(t2, cycles) - sineWavePrimitive(t1, cycles);
        }

        public double GetDerivative(double t)
        {
            double cycles = Math.Round((1 - Math.Abs(MathHelper.Clamp(P, -1, 1))) * 50) + 0.5;

            return P < 0 ? triangleWaveDerivative(t, 1 / cycles) : sineWaveDerivative(t * cycles * 2 * Math.PI) * cycles * 2 * Math.PI;
        }

        public IEnumerable<double> GetInverse(double y)
        {
            double cycles = Math.Round((1 - Math.Abs(MathHelper.Clamp(P, -1, 1))) * 50) + 0.5;

            return P < 0 ? triangleWaveInverse(y, 1 / cycles) : sineWaveInverse(y, 1 / cycles);
        }

        private static double sineWave(double t)
        {
            return (-Math.Cos(t) + 1) / 2;
        }

        private static double sineWaveDerivative(double t)
        {
            return Math.Sin(t) / 2;
        }

        private static double sineWavePrimitive(double t, double c)
        {
            return t / 2 - Math.Sin(2 * Math.PI * c * t) / (4 * Math.PI * c);
        }

        private static double triangleWave(double t, double width)
        {
            double modT = t % width;
            return modT < width / 2 ? 2 * modT / width : 2 - 2 * modT / width;
        }

        // This is a square wave
        private static double triangleWaveDerivative(double t, double width)
        {
            double modT = t % width;
            return modT < width / 2 ? 2 / width : -2 / width;
        }

        /// <summary>
        /// https://math.stackexchange.com/questions/178079/integration-of-sawtooth-square-and-triangle-wave-functions
        /// </summary>
        /// <param name="t"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        private static double triangleWaveIntegral(double t, double width)
        {
            double modT = t % width;
            double n = Math.Floor(t / width);
            double integral = modT < width / 2 ? Math.Pow(modT, 2) / width : 2 * modT - Math.Pow(modT, 2) / width - width / 2;
            return n * width * 0.5 + integral;
        }

        private static IEnumerable<double> sineWaveInverse(double y, double width)
        {
            double x1 = width * Math.Acos(1 - 2 * y) / (2 * Math.PI);
            yield return x1;

            double x2 = width * Math.Acos(2 * y - 1) / (2 * Math.PI) + width / 2;
            yield return x2;

            for (int i = 0; i < 1000; i++)
            {
                x1 += width;
                if (x1 > 1) yield break;

                yield return x1;

                x2 += width;
                if (x2 > 1) yield break;

                yield return x2;
            }
        }

        private IEnumerable<double> triangleWaveInverse(double y, double width)
        {
            double x1 = width * y / 2;
            yield return x1;

            double x2 = width * (2 - y) / 2;
            yield return x2;

            for (int i = 0; i < 1000; i++)
            {
                x1 += width;
                if (x1 > 1) yield break;

                yield return x1;

                x2 += width;
                if (x2 > 1) yield break;

                yield return x2;
            }
        }
    }
}
