///⚠️ WARNING ⚠️
/// This code is quite possibly the WORST way to implement dynamic patch generation
/// but it works... soo like....
/// anyway, if you're wanting to figure out how it works, enjoy the spaghetti!!!
/// (harmony REALLY doesn't like generating patches like this...)

using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using NLua;
using UnityEngine;

namespace RWLua
{
    class DynamicPatchGen
    {
        public static Harmony harmony;
        private MethodInfo originalMethod;
        private LuaMod mod;
        private string callbackName;
        public bool success = false;

        private static Dictionary<MethodBase, DynamicPatchGen> patchInstances = new Dictionary<MethodBase, DynamicPatchGen>();

        public DynamicPatchGen(Type targetType, string methodName, LuaMod luaState, string callbackFunctionName)
        {
            originalMethod = targetType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (originalMethod == null)
            {
                throw new ArgumentException($"Method '{methodName}' not found in type '{targetType.FullName}'.");
            }

            Debug.Log($"[RWLua (DynamicPatchGen)]: Found method '{originalMethod.DeclaringType.FullName}::{methodName}()'");

            patchInstances[originalMethod] = this;

            bool isVoid = originalMethod.ReturnType == typeof(void);
            bool hasParameters = originalMethod.GetParameters().Length > 0;

            MethodInfo prefixMethodInfo;
            if (isVoid)
            {
                prefixMethodInfo = hasParameters
                    ? typeof(DynamicPatchGen).GetMethod(nameof(StaticLuaPrefixVoidWithParams), BindingFlags.NonPublic | BindingFlags.Static)
                    : typeof(DynamicPatchGen).GetMethod(nameof(StaticLuaPrefixVoid), BindingFlags.NonPublic | BindingFlags.Static);
            }
            else
            {
                prefixMethodInfo = hasParameters
                    ? typeof(DynamicPatchGen).GetMethod(nameof(StaticLuaPrefixWithResultAndParams), BindingFlags.NonPublic | BindingFlags.Static)
                    : typeof(DynamicPatchGen).GetMethod(nameof(StaticLuaPrefixWithResult), BindingFlags.NonPublic | BindingFlags.Static);
            }

            if (prefixMethodInfo == null)
            {
                throw new InvalidOperationException("Could not find an appropriate prefix method.");
            }

            try
            {
                callbackName = callbackFunctionName;
                harmony.Patch(originalMethod, prefix: new HarmonyMethod(prefixMethodInfo));
                mod = luaState;
                Debug.Log("[RWLua (DynamicPatchGen)]: Harmony patch applied successfully.");
                success = true;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[RWLua (DynamicPatchGen)]: FAILED to create a patch with reason \"{e.Message}\", \"{e.InnerException?.Message}\"");
            }
        }

        // static prefix for void methods without parameters
        private static bool StaticLuaPrefixVoid(object __instance)
        {
            if (patchInstances.TryGetValue(__instance.GetType().GetMethod("MethodName"), out var patchInstance)) // Adjust according to your context
            {
                return patchInstance.InstanceLuaPrefixVoid(__instance);
            }

            Debug.LogError("[RWLua (DynamicPatchGen)]: No patch instance found for the method.");
            return true;
        }

        // static prefix for void methods with parameters
        private static void StaticLuaPrefixVoidWithParams(object __instance, object[] __args)
        {
            if (patchInstances.TryGetValue(__instance.GetType().GetMethod("MethodName"), out var patchInstance)) // Adjust according to your context
            {
                patchInstance.InstanceLuaPrefixVoid(__instance, __args);
            }
            else
            {
                Debug.LogError("[RWLua (DynamicPatchGen)]: No patch instance found for the method.");
            }
        }

        // static prefix for methods with return values without parameters
        private static bool StaticLuaPrefixWithResult(object __instance, ref object __result)
        {
            if (patchInstances.TryGetValue(__instance.GetType().GetMethod("MethodName"), out var patchInstance)) // Adjust according to your context
            {
                return patchInstance.InstanceLuaPrefixWithResult(__instance, ref __result);
            }

            Debug.LogError("[RWLua (DynamicPatchGen)]: No patch instance found for the method.");
            return true;
        }

        // static prefix for methods with return values and parameters
        private static bool StaticLuaPrefixWithResultAndParams(object __instance, ref object __result, object[] __args)
        {
            if (patchInstances.TryGetValue(__instance.GetType().GetMethod("MethodName"), out var patchInstance)) // Adjust according to your context
            {
                return patchInstance.InstanceLuaPrefixWithResult(__instance, ref __result, __args);
            }

            Debug.LogError("[RWLua (DynamicPatchGen)]: No patch instance found for the method.");
            return true;
        }

        // non-static instance prefix method for void methods without parameters
        private bool InstanceLuaPrefixVoid(object __instance)
        {
            try
            {
                if (mod.requestFunction(callbackName) is LuaFunction func)
                {
                    func.Call(__instance);
                }
                else
                {
                    Debug.LogWarning($"[RWLua (DynamicPatchGen)]: Lua function '{callbackName}' not found.");
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[RWLua (DynamicPatchGen)]: Exception in InstanceLuaPrefixVoid: {ex}");
                return true;
            }
        }

        // non-static instance prefix method for void methods with parameters
        private void InstanceLuaPrefixVoid(object __instance, object[] __args)
        {
            try
            {
                if (mod.requestFunction(callbackName) is LuaFunction func)
                {
                    if (__args != null && __args.Length > 0)
                    {
                        func.Call(__args);
                    }
                    else
                    {
                        Debug.LogWarning($"[RWLua (DynamicPatchGen)]: No arguments provided for Lua function '{callbackName}'. Calling with instance only.");
                        func.Call(__instance);
                    }
                }
                else
                {
                    Debug.LogWarning($"[RWLua (DynamicPatchGen)]: Lua function '{callbackName}' not found.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[RWLua (DynamicPatchGen)]: Exception in InstanceLuaPrefixVoid: {ex}");
            }
        }

        // non-static instance prefix method for methods with return values without parameters
        private bool InstanceLuaPrefixWithResult(object __instance, ref object __result)
        {
            try
            {
                if (mod.requestFunction(callbackName) is LuaFunction func)
                {
                    Debug.Log($"[RWLua (DynamicPatchGen)]: Calling Lua function '{callbackName}' with instance: {__instance}");
                    var luaResult = func.Call(__instance);

                    if (luaResult != null && luaResult.Length > 0)
                    {
                        __result = Convert.ChangeType(luaResult[0], originalMethod.ReturnType);
                    }
                }
                else
                {
                    Debug.LogWarning($"[RWLua (DynamicPatchGen)]: Lua function '{callbackName}' not found.");
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[RWLua (DynamicPatchGen)]: Exception in InstanceLuaPrefixWithResult: {ex}");
                return true;
            }
        }

        // non-static instance prefix method for methods with return values and parameters
        private bool InstanceLuaPrefixWithResult(object __instance, ref object __result, object[] __args)
        {
            try
            {
                if (mod.requestFunction(callbackName) is LuaFunction func)
                {
                    Debug.Log($"[RWLua (DynamicPatchGen)]: Calling Lua function '{callbackName}' with instance: {__instance} and arguments: {string.Join(", ", __args)}");

                    var luaResult = (__args != null && __args.Length > 0)
                        ? func.Call(__instance, __args)
                        : func.Call(__instance);

                    if (luaResult != null && luaResult.Length > 0)
                    {
                        __result = Convert.ChangeType(luaResult[0], originalMethod.ReturnType);
                    }
                }
                else
                {
                    Debug.LogWarning($"[RWLua (DynamicPatchGen)]: Lua function '{callbackName}' not found.");
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[RWLua (DynamicPatchGen)]: Exception in InstanceLuaPrefixWithResult: {ex}");
                return true;
            }
        }
    }
}