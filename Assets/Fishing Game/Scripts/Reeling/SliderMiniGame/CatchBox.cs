using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FishingGame.Reeling
{
    /// <summary>
    /// This class is apart of the Slidersminigame
    /// It has a single function CheckUIOverlap that checks if the catchbox and fish icon are overlapping
    /// </summary>
    public class CatchBox : MonoBehaviour
    {
        /// <summary>
        /// Returns true if the Fish icon and the catchbox ui elements are overlapping
        /// Both parameters can be either the catchbox or fish icon
        /// </summary>
        /// <param name="rectTrans1">The transform of one of the UI boxes</param>
        /// <param name="rectTrans2">The transform of one of the UI boxes</param>
        public bool CheckUIOverlap(RectTransform rectTrans1, RectTransform rectTrans2)
        {
            Rect rect1 = new Rect(rectTrans1.localPosition.x, rectTrans1.localPosition.y, rectTrans1.rect.width, rectTrans1.rect.height);
            Rect rect2 = new Rect(rectTrans2.localPosition.x, rectTrans2.localPosition.y, rectTrans2.rect.width, rectTrans2.rect.height);
            return rect1.Overlaps(rect2);
        }
    }
}

