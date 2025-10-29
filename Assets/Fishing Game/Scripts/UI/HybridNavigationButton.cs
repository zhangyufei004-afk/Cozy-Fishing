

using UnityEngine;
using UnityEngine.UI;

namespace FishingGame.Fishing_Game.Scripts.UI
{
    /// <summary>
    /// Custom button class which overrides the navigation behaviour to allow explicit setting of select on left/right/up/down with automatic fallback for
    /// others.
    /// </summary>
    public class HybridNavigationButton : Button
    {
        public override Selectable FindSelectableOnLeft()
        {
            return navigation.selectOnLeft != null ? navigation.selectOnLeft
                : FindSelectable(Vector3.left);
        }

        public override Selectable FindSelectableOnRight()
        {
            return navigation.selectOnRight != null ? navigation.selectOnRight
                : FindSelectable(Vector3.right);
        }

        public override Selectable FindSelectableOnUp()
        {
            return navigation.selectOnUp != null ? navigation.selectOnUp
                : FindSelectable(Vector3.up);
        }

        public override Selectable FindSelectableOnDown()
        {
            return navigation.selectOnDown != null ? navigation.selectOnDown
                : FindSelectable(Vector3.down);
        }
    }
}