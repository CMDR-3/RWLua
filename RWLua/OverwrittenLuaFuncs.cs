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
        public static LuaMod ModState;

        public OverwrittenLuaFuncs(String modName, LuaMod modState)
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
            ModState.modState.LoadCLRPackage();
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

        public static object FindObjectOfClass(string className)
        {
            Type _class = Type.GetType(className);
            if (_class == null) { return null; };

            var findMethod = typeof(UnityEngine.Object).GetMethod("FindObjectOfType", new Type[] { });
            var genericMethod = findMethod.MakeGenericMethod(_class);

            return (UnityEngine.Object)genericMethod.Invoke(null, null);
        }

        public static void RegisterHook(string className, string methodName, string callback)
        {
            // Load the Assembly-CSharp.dll
            Assembly assembly = Assembly.Load("Assembly-CSharp");

            if (assembly != null)
            {
                // Try to get the type using the assembly
                Type t = assembly.GetType(className);

                if (t != null)
                {
                    DynamicPatchGen patch = new DynamicPatchGen(t, methodName, ModState, callback);
                    if (patch.success)
                    {
                        Debug.Log("yippee");
                    }
                    else
                    {
                        Debug.Log("not so yippee");
                    }
                    return;// patch.success;
                }
                else
                {
                    Debug.LogError($"[RWLua]: Type '{className}' could not be found in Assembly-CSharp.");
                    return;// false;
                }
            }
            else
            {
                Debug.LogError("[RWLua]: Failed to load Assembly-CSharp.");
                return;// false;
            }
        }
    }
}
