using System;
using UnityEngine;

namespace RWLua
{
    public class OverwrittenLuaFuncs
    {
        public static String ModName;

        public OverwrittenLuaFuncs(String modName)
        {
            ModName = modName;
        }

        public static void Print(String val)
        {
            Debug.Log($"[RWLua ('{ModName}')]: " + val);
        }
    }
}
