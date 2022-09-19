// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Utils;
using osuTK.Graphics;

namespace osu.Framework.Graphics.UserInterface.Graph.Markers
{
    public class CustomMarkerGenerator : IMarkerGenerator
    {
        public delegate string ToStringFunction(double value);

        public double Offset { get; set; }

        public double StepSize { get; set; }

        public bool Snappable { get; set; }

        public bool Reduce { get; set; }

        public bool DrawMarker { get; set; }

        public double MarkerLength { get; set; }

        public Color4 MarkerColor { get; set; }

        public ToStringFunction ValueToString { get; set; }

        public CustomMarkerGenerator() { }

        public IEnumerable<GraphMarker> GenerateMarkers(double start, double end, Direction orientation, int maxMarkers)
        {
            double step = StepSize;

            if ((end - start) / step > maxMarkers)
            {
                if (Reduce)
                {
                    while ((end - start) / step > maxMarkers)
                    {
                        step *= 2;
                    }
                }
                else
                {
                    yield break;
                }
            }

            double vStart = Math.Ceiling((start - Offset) / step) * step + Offset;
            double v = vStart;
            int i = 0;

            while (v <= end + Precision.DOUBLE_EPSILON)
            {
                string? text = ValueToString?.Invoke(v);
                yield return new GraphMarker
                {
                    Orientation = orientation,
                    Text = text,
                    Value = v,
                    Snappable = Snappable,
                    DrawMarker = DrawMarker,
                    MarkerLength = MarkerLength,
                    MarkerColor = MarkerColor
                };

                v = vStart + step * ++i;
            }
        }
    }
}
