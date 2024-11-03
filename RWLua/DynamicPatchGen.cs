using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Scripting;
using UnityEngine;

namespace RWLua
{
    class DynamicPatchGen
    {
        public static Harmony harmony;

        // WARNING--Runtime compilation of hooks is SLOW!! //
        // Limit usage of dynamic patches as much as possible. //
        public async static void LoadAndApplyPatch(string patchCode, string methodToPatch)
        {
            try
            {
                Debug.Log("HERE");
                // Evaluate the patch code
                var scriptOptions = ScriptOptions.Default
                    .WithReferences(Assembly.GetExecutingAssembly()) // Add your assemblies as needed
                    .WithImports("HarmonyLib", "UnityEngine", "Assembly-CSharp");

                var script = await CSharpScript.EvaluateAsync(patchCode, scriptOptions);

                // Get the method info for the patch
                var patchMethod = script.GetType().GetMethod("Postfix", BindingFlags.Public | BindingFlags.Static);
                if (patchMethod != null)
                {
                    Debug.Log("HERE 2");
                    var originalMethod = AccessTools.Method(methodToPatch);
                    if (originalMethod != null)
                    {
                        harmony.Patch(originalMethod, postfix: new HarmonyMethod(patchMethod));
                        Debug.Log("Patch applied successfully to " + methodToPatch + "!");
                    }
                    else
                    {
                        Debug.LogError($"Original method '{methodToPatch}' not found.");
                    }
                }
                else
                {
                    Debug.Log("HERE 3");
                    Debug.LogError("Patch method 'Postfix' not found.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to apply patch: {ex.Message}");
            }
        }
    }
}
