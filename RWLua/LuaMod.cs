﻿using System;
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

            modState.DoFile(modPath); // /* Should point to the autorun file that then can require other files from the mod */
        }
    }
}
