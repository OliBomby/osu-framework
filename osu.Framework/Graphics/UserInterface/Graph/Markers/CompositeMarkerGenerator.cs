// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;

namespace osu.Framework.Graphics.UserInterface.Graph.Markers
{
    public class CompositeMarkerGenerator : IMarkerGenerator
    {
        public IMarkerGenerator[] Generators { get; set; }

        public CompositeMarkerGenerator(IMarkerGenerator[] generators)
        {
            Generators = generators;
        }

        public IEnumerable<GraphMarker> GenerateMarkers(double start, double end, Direction orientation, int maxMarkers)
        {
            foreach (var generator in Generators)
            {
                foreach (var marker in generator.GenerateMarkers(start, end, orientation, maxMarkers))
                {
                    yield return marker;
                }
            }
        }
    }
}
