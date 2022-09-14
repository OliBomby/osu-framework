// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics.UserInterface.Graph.Interpolation;
using osu.Framework.Utils;
using osuTK;

namespace osu.Framework.Graphics.UserInterface.Graph
{
    public static class GraphStateExtensions
    {
        public static double GetValue(this AnchorState[] anchors, double x)
        {
            // Find the section
            var previousAnchor = anchors[0];
            var nextAnchor = anchors[^1];

            foreach (var anchor in anchors)
            {
                if (anchor.Pos.X < x)
                {
                    previousAnchor = anchor;
                }
                else
                {
                    nextAnchor = anchor;
                    break;
                }
            }

            // Calculate the value via interpolation
            var difference = nextAnchor.Pos - previousAnchor.Pos;

            if (Precision.AlmostEquals(difference.X, 0))
            {
                return previousAnchor.Pos.Y;
            }

            return previousAnchor.Pos.Y + difference.Y * nextAnchor.Interpolator.GetInterpolation((x - previousAnchor.Pos.X) / difference.X);
        }

        public static double GetDerivative(this AnchorState[] anchors, double x)
        {
            // Find the section
            var previousAnchor = anchors[0];
            var nextAnchor = anchors[1];

            foreach (var anchor in anchors)
            {
                if (anchor.Pos.X < x)
                {
                    previousAnchor = anchor;
                }
                else
                {
                    nextAnchor = anchor;
                    break;
                }
            }

            // Calculate the value via interpolation
            var difference = nextAnchor.Pos - previousAnchor.Pos;

            if (Precision.AlmostEquals(difference.X, 0))
            {
                return difference.Y > 0 ? double.PositiveInfinity : double.NegativeInfinity;
            }

            double derivative;

            if (nextAnchor.Interpolator is IDerivableInterpolator derivableInterpolator)
            {
                derivative = derivableInterpolator.GetDerivative((x - previousAnchor.Pos.X) / difference.X);
            }
            else
            {
                derivative = 1;
            }

            return derivative * difference.Y / difference.X;
        }

        public static double GetIntegral(this AnchorState[] anchors, double t1, double t2)
        {
            double height = 0;
            AnchorState? previousAnchor = null;

            foreach (var anchor in anchors)
            {
                if (previousAnchor == null)
                {
                    previousAnchor = anchor;
                    continue;
                }

                var p1 = previousAnchor.Value.Pos;
                var p2 = anchor.Pos;

                double p1Cx = MathHelper.Clamp(p1.X, t1, t2);
                double p2Cx = MathHelper.Clamp(p2.X, t1, t2);

                if (p2.X < t1 || p1.X > t2)
                {
                    previousAnchor = anchor;
                    continue;
                }

                var difference = p2 - p1;
                double differenceCx = p2Cx - p1Cx;

                if (Precision.AlmostEquals(differenceCx, 0))
                {
                    previousAnchor = anchor;
                    continue;
                }

                double integral;

                if (anchor.Interpolator is IIntegrableInterpolator integrableInterpolator)
                {
                    integral = integrableInterpolator.GetIntegral((p1Cx - p1.X) / difference.X, (p2Cx - p1.X) / difference.X);
                }
                else
                {
                    integral = 0.5 * Math.Pow((p2Cx - p1.X) / difference.X, 2) - 0.5 * Math.Pow((p1Cx - p1.X) / difference.X, 2);
                }

                height += integral * difference.X * difference.Y + differenceCx * p1.Y;

                previousAnchor = anchor;
            }

            return height;
        }

        public static double GetMaxValue(this AnchorState[] anchors)
        {
            return getExtremeValue(anchors, false);
        }

        public static double GetMinValue(this AnchorState[] anchors)
        {
            return getExtremeValue(anchors, true);
        }

        private static double getExtremeValue(AnchorState[] anchors, bool min)
        {
            AnchorState? previousAnchor = null;
            double extremeValue = min ? double.PositiveInfinity : double.NegativeInfinity;

            void compare(double value)
            {
                if (min ? value < extremeValue : value > extremeValue)
                {
                    extremeValue = value;
                }
            }

            foreach (var anchor in anchors)
            {
                compare(anchor.Pos.Y);

                // If the interpolator has custom extrema, then we check the min/max value for all the specified locations
                if (previousAnchor is not null && anchor.Interpolator is IHasCustomExtrema hasCustomExtrema)
                {
                    var p1 = previousAnchor.Value.Pos;
                    var p2 = anchor.Pos;
                    float differenceY = p2.Y - p1.Y;

                    foreach (double extremaPosition in hasCustomExtrema.ExtremaPositions)
                    {
                        compare(p1.Y + differenceY * anchor.Interpolator.GetInterpolation(extremaPosition));
                    }
                }

                previousAnchor = anchor;
            }

            return extremeValue;
        }

        public static double GetMaxDerivative(this AnchorState[] anchors)
        {
            return getExtremeDerivative(anchors, false);
        }

        public static double GetMinDerivative(this AnchorState[] anchors)
        {
            return getExtremeDerivative(anchors, true);
        }

        private static double getExtremeDerivative(AnchorState[] anchors, bool min)
        {
            AnchorState? previousAnchor = null;
            double extremeValue = min ? double.PositiveInfinity : double.NegativeInfinity;

            void compare(double value)
            {
                if (min ? value < extremeValue : value > extremeValue)
                {
                    extremeValue = value;
                }
            }

            foreach (var anchor in anchors)
            {
                if (previousAnchor is null)
                {
                    previousAnchor = anchor;
                    continue;
                }

                var p1 = previousAnchor.Value.Pos;
                var p2 = anchor.Pos;
                var difference = p2 - p1;
                double baseDerivative = difference.Y / difference.X;

                if (anchor.Interpolator is IDerivableInterpolator derivableInterpolator)
                {
                    // If the interpolator has custom derivative extrema, then we check the min/max derivative for all the specified locations
                    if (anchor.Interpolator is IHasCustomDerivativeExtrema hasCustomDerivativeExtrema)
                    {
                        foreach (double position in hasCustomDerivativeExtrema.ExtremaPositions)
                        {
                            compare(derivableInterpolator.GetDerivative(position) * baseDerivative);
                        }
                    }
                    else
                    {
                        compare(derivableInterpolator.GetDerivative(0) * baseDerivative);
                        compare(derivableInterpolator.GetDerivative(1) * baseDerivative);
                    }
                }
                else
                {
                    compare(baseDerivative);
                }

                previousAnchor = anchor;
            }

            return extremeValue;
        }

        public static double GetMaxIntegral(this AnchorState[] anchors)
        {
            return getExtremeIntegral(anchors, false);
        }

        public static double GetMinIntegral(this AnchorState[] anchors)
        {
            return getExtremeIntegral(anchors, true);
        }

        private static double getExtremeIntegral(AnchorState[] anchors, bool min)
        {
            double height = 0;
            double extremeValue = 0;
            AnchorState? previousAnchor = null;

            void compare(double value, double width, double p1Y, Vector2 difference)
            {
                double actualValue = value * difference.X * difference.Y + width * difference.X * p1Y + height;

                if (min ? actualValue < extremeValue : actualValue > extremeValue)
                {
                    extremeValue = actualValue;
                }
            }

            foreach (var anchor in anchors)
            {
                if (previousAnchor is null)
                {
                    previousAnchor = anchor;
                    continue;
                }

                var p1 = previousAnchor.Value.Pos;
                var p2 = anchor.Pos;
                var difference = p2 - p1;

                if (Precision.AlmostEquals(difference.X, 0))
                {
                    previousAnchor = anchor;
                    continue;
                }

                double endIntegral;

                if (anchor.Interpolator is IIntegrableInterpolator integrableInterpolator)
                {
                    endIntegral = integrableInterpolator.GetIntegral(0, 1) * difference.X * difference.Y + difference.X * p1.Y;

                    // If the interpolator has custom integral extrema, then we check the min/max integral for all the specified locations
                    if (anchor.Interpolator is IHasCustomIntegralExtrema hasCustomIntegralExtrema)
                    {
                        foreach (double position in hasCustomIntegralExtrema.ExtremaPositions)
                        {
                            compare(integrableInterpolator.GetIntegral(0, position), position, p1.Y, difference);
                        }
                    }
                    else
                    {
                        compare(integrableInterpolator.GetIntegral(0, 1), 1, p1.Y, difference);
                    }

                    // Check if the interpolation passes through 0
                    if (difference.Y * p1.Y < 0)
                    {
                        // Possibility of max/min not at endpoints. We need to calculate where the curve passes through zero
                        if (integrableInterpolator is IInvertibleInterpolator invertibleInterpolator)
                        {
                            // Calculate all the zeros of the interpolation
                            var zeros = invertibleInterpolator.GetInverse(-p1.Y / difference.Y);

                            foreach (double position in zeros)
                            {
                                if (position is < 0 or > 1) continue;

                                compare(integrableInterpolator.GetIntegral(0, position), position, p1.Y, difference);
                            }
                        }
                        else
                        {
                            double position = min
                                ? GradientDescentUtils.GradientDescent(
                                    d => integrableInterpolator.GetIntegral(0, d) * difference.X * difference.Y + d * difference.X * p1.Y,
                                    0, 1, 0.1)
                                : GradientDescentUtils.GradientAscent(
                                    d => integrableInterpolator.GetIntegral(0, d) * difference.X * difference.Y + d * difference.X * p1.Y,
                                    0, 1, 0.1);

                            compare(integrableInterpolator.GetIntegral(0, position), position, p1.Y, difference);
                        }
                    }
                }
                else
                {
                    // Assume a linear interpolation
                    endIntegral = 0.5 * difference.X * difference.Y + difference.X * p1.Y;
                    compare(0.5, 1, p1.Y, difference);

                    // Check if the interpolation passes through 0
                    if (difference.Y * p1.Y < 0)
                    {
                        // Possibility of max/min not at endpoints. For a linear interpolator this is possible to solve algebraically
                        float position = -p1.Y / difference.Y;

                        if (position is >= 0 and <= 1)
                        {
                            compare(0.5 * Math.Pow(position, 2), position, p1.Y, difference);
                        }
                    }
                }

                height += endIntegral;
                previousAnchor = anchor;
            }

            return extremeValue;
        }
    }
}
