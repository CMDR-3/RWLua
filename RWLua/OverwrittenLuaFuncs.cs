using NLua;
using System;
using UnityEngine;

namespace RWLua
{
    public class OverwrittenLuaFuncs
    {
        public static String ModName;
        public static Lua ModState;

        public OverwrittenLuaFuncs(String modName, Lua modState)
        {
            ModName = modName;
            ModState = modState;
        }

        public static void Print(String val)
        {
            Debug.Log($"[RWLua ('{ModName}')]: " + val);
        }

        public static void PrepareImport()
        {
            ModState.LoadCLRPackage();
        }
    }
}
