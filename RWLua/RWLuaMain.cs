using BepInEx;
using HarmonyLib;
using Newtonsoft.Json;
using NLua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RWLua
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class RWLuaMain : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "rwlua";
        public const string PLUGIN_NAME = "RWLua";
        public const string PLUGIN_VERSION = "1.0.0";

        private void OnEnable()
        {
            On.Menu.MainMenu.Update += MainMenu_Update;
            On.RainWorldGame.Update += RainWorldGame_Update;
            On.RainWorld.Update += RainWorld_Update;
            On.RainWorldGame.ExitToMenu += RainWorldGame_ExitToMenu;
            On.ProcessManager.ctor += ProcessManager_ctor;
            On.Player.Update += RWPlayerHandler.Player_Update;
        }

        private void ProcessManager_ctor(On.ProcessManager.orig_ctor orig, ProcessManager self, RainWorld rainWorld)
        {
            orig(self, rainWorld);
            ProcessManagerHelper.procManager = self;
        }

        private void RainWorldGame_ExitToMenu(On.RainWorldGame.orig_ExitToMenu orig, RainWorldGame self)
        {
            rwGameInstance = null;
            foreach (LuaMod mod in loadedMods)
            {
                // /* RainWorldGame is the scug save not the entire game, RainWorld is the encompassing class */
                // /* edge case for when a mod modifys stuff in the world but the player exits the game. */
                // /* maybe they'll come back? */
                mod.modState["rwgameinstance"] = null;
            }
            orig(self);
        }

        public static List<LuaMod> loadedMods = new List<LuaMod>();

        private bool firstRun = false;

        protected RainWorld rwInstance = null;
        protected RainWorldGame rwGameInstance = null;
        private void RainWorld_Update(On.RainWorld.orig_Update orig, RainWorld self)
        {
            rwInstance = self;
            if (!firstRun && self.started)
            {
                firstRun = true;
                Debug.Log("[RWLua]: Loading Lua mods...");

                if (!File.Exists(Path.GetDirectoryName(BepInEx.Paths.ExecutablePath) + "\\" + "CLRPackage.lua"))
                {
                    string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                    UriBuilder uri = new UriBuilder(codeBase);
                    string path = Uri.UnescapeDataString(uri.Path);
                    path = Path.GetDirectoryName(path);
                    File.Copy(path + "\\CLRPackage.lua", Path.GetDirectoryName(BepInEx.Paths.ExecutablePath) + "\\" + "CLRPackage.lua");
                }

                string modsDirectory = Application.streamingAssetsPath + "\\mods\\";
                string[] mods = Directory.GetDirectories(modsDirectory);

                foreach (var folder in mods)
                {
                    if (File.Exists(folder + "\\rwluainfo.json"))
                    {
                        RWLuaInfo info = JsonConvert.DeserializeObject<RWLuaInfo>(File.ReadAllText(folder + "\\rwluainfo.json"));
                        Debug.Log($"[RWLua]: Found mod '{info.name}', loading...");

                        LuaMod loadedMod = new LuaMod(info.name, $"{folder}\\{info.luapath}\\{info.entrypoint}.lua");

                        loadedMod.modState["rainworld"] = rwInstance;
                        loadedMod.modState["menu"] = new MenuHelper();
                        loadedMod.modState["processmanager"] = ProcessManagerHelper.procManager;

                        loadedMods.Add(loadedMod);

                        Debug.Log($"[RWLua]: Loaded mod '{info.name}'!");
                    }
                }

                Debug.Log("[RWLua]: Done!");
            }

            foreach (LuaMod mod in loadedMods)
            {
                LuaFunction updateFunc = mod.requestFunction("Update");
                if (updateFunc != null) { updateFunc.Call(self); };
            }

            orig(self);
        }

        private void RainWorldGame_Update(On.RainWorldGame.orig_Update orig, RainWorldGame self)
        {
            /* game just started. yipee! alert mods pls */
            if (rwGameInstance == null)
            {
                // /* probably the worst way to do this but im too lazy to look into singletons. */
                rwGameInstance = self;

                foreach (LuaMod mod in loadedMods)
                {
                    mod.modState["rwgameinstance"] = rwGameInstance;
                    LuaFunction gameStartFunc = mod.requestFunction("GameLoad");
                    if (gameStartFunc != null) { gameStartFunc.Call(rwGameInstance); };
                }
            }

            orig(self);
        }

        private void MainMenu_Update(On.Menu.MainMenu.orig_Update orig, Menu.MainMenu self)
        {
            orig(self);
        }

        void PlayerUpdate(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);
        }
    }
}
