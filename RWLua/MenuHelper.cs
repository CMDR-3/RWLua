using Menu;
using System;
using UnityEngine;

namespace RWLua
{
    public class MenuHelper
    {
        public static SimpleButton createButton(String displayText, String signalText, Vector2 pos, Vector2 size)
        {
            return new SimpleButton(null, null, displayText, signalText, pos, size);
        }
    }
}
