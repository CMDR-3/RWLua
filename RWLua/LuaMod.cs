using System;
using System.IO;
using NLua;

namespace RWLua
{
    public class LuaMod
    {
        OverwrittenLuaFuncs funcs;
        public Lua modState;
        public LuaMod(String modName, String modPath)
        {
            modState = new Lua();
            funcs = new OverwrittenLuaFuncs(modName, modState);

            modState.RegisterFunction("print", funcs, typeof(OverwrittenLuaFuncs).GetMethod("Print"));
            modState.RegisterFunction("prepareimport", funcs, typeof(OverwrittenLuaFuncs).GetMethod("PrepareImport"));
            modState.RegisterFunction("requestclass", funcs, typeof(OverwrittenLuaFuncs).GetMethod("RequestClass"));
            modState.RegisterFunction("vec2", funcs, typeof(OverwrittenLuaFuncs).GetMethod("CreateVec2"));

            // /* Prepare the package.path so it points to the mod directory and you can require from it */
            String topLevel = Path.GetDirectoryName(modPath);
            topLevel = topLevel.Replace("\\", "/");
            modState.DoString("package.path = package.path .. \";" + topLevel + "/?/?.lua;" + topLevel + "/?.lua\"");
            modState.DoFile(modPath); // /* Should point to the autorun file that then can require other files from the mod */
        }

        // /* returns null if function doesn't exist. case sensitive. */
        public LuaFunction requestFunction(String functionName)
        {
            if (modState[functionName] != null)
            {
                try
                {
                    return modState[functionName] as LuaFunction;
                }
                catch (Exception)
                {
                    return null;
                }
            };

            return null;
        }
    }
}
