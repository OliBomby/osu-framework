// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Utils;
using osuTK;

namespace osu.Framework.Graphics.UserInterface.Graph.Interpolation.Interpolators
{
    public class SingleCurveInterpolator2 : InterpolatorBase, IHasName, IDerivableInterpolator, IIntegrableInterpolator
    {
        private readonly LinearInterpolator linearDegenerate;

        public string Name => "Single curve 2";

        public SingleCurveInterpolator2()
        {
            linearDegenerate = new LinearInterpolator();
        }

        public override double GetInterpolation(double t)
        {
            if (Math.Abs(P) < Precision.DOUBLE_EPSILON)
            {
                return linearDegenerate.GetInterpolation(t);
            }

            double p = -MathHelper.Clamp(P, -1, 1) * 10;
            return f(t, p);
        }

        private static double f(double t, double k)
        {
            return (Math.Pow(2, k * t) - 1) / (Math.Pow(2, k) - 1);
        }

        public double GetDerivative(double t)
        {
            if (Math.Abs(P) < Precision.DOUBLE_EPSILON)
            {
                return linearDegenerate.GetDerivative(t);
            }

            double p = -MathHelper.Clamp(P, -1, 1) * 10;
            return p * Math.Log(2) * Math.Pow(2, p * t) / (Math.Pow(2, p) - 1);
        }

        public double GetIntegral(double t1, double t2)
        {
            if (Math.Abs(P) < Precision.DOUBLE_EPSILON)
            {
                return linearDegenerate.GetIntegral(t1, t2);
            }

            double p = -MathHelper.Clamp(P, -1, 1) * 10;
            return primitive(t2, p) - primitive(t1, p);
        }

        private static double primitive(double t, double p)
        {
            return (Math.Pow(2, p * t) / (p * Math.Log(2)) - t) / (Math.Pow(2, p) - 1);
        }
    }
}
