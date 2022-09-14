// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Utils;
using osuTK.Graphics;

namespace osu.Framework.Graphics.UserInterface.Graph.Markers
{
    public class DividedBeatMarkerGenerator : IMarkerGenerator
    {
        public int BeatDivisor { get; set; }

        public bool Snappable { get; set; }

        public DividedBeatMarkerGenerator()
            : this(4)
        {
        }

        public DividedBeatMarkerGenerator(int beatDivisor, bool snappable = false)
        {
            BeatDivisor = beatDivisor;
            Snappable = snappable;
        }

        public IEnumerable<GraphMarker> GenerateMarkers(double start, double end, Direction orientation, int maxMarkers)
        {
            var markers = new List<GraphMarker>();

            double step = 1d / BeatDivisor;

            while ((end - start) / step > maxMarkers)
            {
                step *= 2;
            }

            var vStart = Math.Ceiling(start / step) * step;
            var v = vStart;
            int i = 0;

            while (v <= end + Precision.DOUBLE_EPSILON)
            {
                Color4 markerColor;
                double markerLength;

                if (Math.Abs(v % 4) < Precision.DOUBLE_EPSILON)
                {
                    markerColor = Colour4.White;
                    markerLength = 20;
                }
                else if (Math.Abs(v % 1) < Precision.DOUBLE_EPSILON)
                {
                    markerColor = Color4.White;
                    markerLength = 12;
                }
                else if (Math.Abs(v % (1d / 2)) < Precision.DOUBLE_EPSILON)
                {
                    markerColor = Color4.Red;
                    markerLength = 7;
                }
                else if (Math.Abs(v % (1d / 4)) < Precision.DOUBLE_EPSILON)
                {
                    markerColor = Color4.DodgerBlue;
                    markerLength = 7;
                }
                else if (Math.Abs(v % (1d / 8)) < Precision.DOUBLE_EPSILON)
                {
                    markerColor = Color4.Yellow;
                    markerLength = 7;
                }
                else if (Math.Abs(v % (1d / 6)) < Precision.DOUBLE_EPSILON)
                {
                    markerColor = Color4.Purple;
                    markerLength = 7;
                }
                else
                {
                    markerColor = Color4.Gray;
                    markerLength = 7;
                }

                markers.Add(new GraphMarker
                {
                    Orientation = orientation, Value = v, DrawMarker = true,
                    MarkerColor = markerColor, MarkerLength = markerLength, Text = null, Snappable = Snappable
                });

                v = vStart + step * ++i;
            }

            return markers;
        }
    }
}
