# ⚠️ UNFINISHED! ⚠️
### You can still use RWLua in it's current state, however, you will be responsible for significantly more.

# RWLua
is a compatibility layer/wrapper thing for Rain World mods.<br>
Do you not like C#? Do you want to make Rain World mods? Here you go.<br>
<br>
Creating a mod is simple. Go to your Rain World directory, RainWorld_Data, then StreamingAssets, mods, create a new folder named your mod name in all lowercase, and you're done! ...Technically.<br>
You can either choose to do a [code mod](https://rainworldmodding.miraheze.org/wiki/Category:Code_Mods) or a [content mod](https://rainworldmodding.miraheze.org/wiki/Category:Content_Mods).<br>
I presume you're here for the code mod--so let's get started.<br><br>

Under your new folder, create a file called rwluainfo.json and a folder called something. (I recommend "lua", for a reason that will be apparent soon.)<br>
In rwluainfo.json, follow the structure of "id", "name", "luapath", and "entrypoint". As an example, here's a rwluainfo.json for a mod I'm working on:
```json
{
    "id": "zetalasis.beholder",
    "name": "The Beholder",
    "luapath": "lua",
    "entrypoint": "autorun"
}
```
A quick note on the names here:
<br>**"id"** is for dependencies because Lua's require function is stupid.
<br>**"name"** is used for modloading and logging.
<br>**"luapath"** is used for telling RWLua where your lua files are.
<br>**"entrypoint"** is for telling RWLua what file to run when the game starts.
<br>
<br>Now under your luapath folder (mine is called lua), create a file called your entrypoint (mine is autorun) + ".lua" at the end.<br>
Your finished mod structure should look something like this:<br>
RainWorld:<br>
--> RainWorld_Data:<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--> StreamingAssets:<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--> mods:<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--> yourmodname:<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rwluainfo.json<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--> lua:<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;autorun.lua<br><br>

# All set up.. now how to interface with RainWorld?
Because RWLua is based on [NLua](https://github.com/NLua/NLua), you can use the 'import' keyword. You can find more information on how to use the keyword on their GitHub page.

# Where's my prints???
RainWorld -> consolelog.txt.
