using NLua;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
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

        // /* Note: Finds class then **creates** a copy of said class, passing the copy to Lua. */
        public object RequestClass(string className)
        {
            var assembly = Assembly.Load("Assembly-CSharp");
            var type = assembly.GetTypes().FirstOrDefault(t => t.FullName == className);

            if (type == null)
            {
                throw new Exception($"Class '{className}' not found in Assembly-CSharp.");
            }

            // Create an instance of the class
            var instance = Activator.CreateInstance(type);

            return instance;
        }

        public static Vector2 CreateVec2(double x, double y)
        {
            return new Vector2((float)x, (float)y);
        }

        public IEnumerator Wait(double seconds)
        {
            yield return new WaitForSeconds((float)seconds);
        }
    }
}
