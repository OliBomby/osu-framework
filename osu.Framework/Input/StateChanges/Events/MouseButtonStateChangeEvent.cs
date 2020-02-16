// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Input.States;
using osuTK.Input;

namespace osu.Framework.Input.StateChanges.Events
{
    public class MouseButtonStateChangeEvent : ButtonStateChangeEvent<MouseButton>
    {
        public MouseButtonStateChangeEvent(InputState state, IInput input, MouseButton button, ButtonStateChangeKind kind)
            : base(state, input, button, kind)
        {
        }
    }
}
