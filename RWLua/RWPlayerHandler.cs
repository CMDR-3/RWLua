using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWLua
{
    public class RWPlayerHandler
    {
        // /* list because Jolly Co-op but I don't have downpour so I can't test it regardless */
        public static List<Player> PlayerList = new List<Player>();
        public static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);
            if (!PlayerList.Contains(self)) PlayerList.Add(self);
            foreach (LuaMod mod in RWLuaMain.loadedMods)
            {
                LuaFunction playerUpdate = mod.requestFunction("PlayerUpdate");
                if (playerUpdate != null) { playerUpdate.Call(self); };
            }
        }
    }
}
