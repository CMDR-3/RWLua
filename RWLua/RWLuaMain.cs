using BepInEx;
using Newtonsoft.Json;
using System.IO;
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
        }


        private bool firstRun = false;
        private void MainMenu_Update(On.Menu.MainMenu.orig_Update orig, Menu.MainMenu self)
        {
            if (!firstRun)
            {
                firstRun = true;
                Debug.Log("[RWLua]: Loading Lua mods...");

                string modsDirectory = Application.streamingAssetsPath + "\\mods\\";
                string[] mods = Directory.GetDirectories(modsDirectory);

                foreach (var folder in mods)
                {
                    if (File.Exists(folder+"\\rwluainfo.json"))
                    {
                        RWLuaInfo info = JsonConvert.DeserializeObject<RWLuaInfo>(File.ReadAllText(folder + "\\rwluainfo.json"));
                        Debug.Log($"[RWLua]: Found mod '{info.name}', loading...");
                        LuaMod loadedMod = new LuaMod(info.name, $"{folder}\\{info.luapath}\\{info.entrypoint}.lua");
                        Debug.Log($"[RWLua]: Loaded mod '{info.name}'!");
                    }
                }

                Debug.Log("[RWLua]: Done!");
            }

            orig(self);
        }

        void PlayerUpdate(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);
        }
    }
}
