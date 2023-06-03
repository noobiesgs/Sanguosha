using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XLua;

namespace Noobie.Sanguosha.Core.Lua
{
    public class LoadLuaScriptDemo
    {
        private readonly LuaEnv m_LuaEnv;

        public LoadLuaScriptDemo(LuaEnv luaEnv)
        {
            m_LuaEnv = luaEnv;
        }

        [CSharpCallLua]
        public delegate double LuaMax(double a, double b);

        public LuaMax Max { get; private set; }

        public void Run()
        {
            m_LuaEnv.DoString(@"
CS.UnityEngine.Debug.Log('hello world from Lua')
a = 5
a = a + 1

local L = CS.Noobie.Sanguosha.Core.Translator
local A = L.AddTranslation

A('hello', '你好')

local LuaCallCSharpClass = CS.Noobie.Sanguosha.Core.Lua.LuaCallCSharpClass
local testObj = LuaCallCSharpClass()

local list_int = CS.System.Collections.Generic.List(CS.System.Int32)
local list = list_int()
list:Add(1)
list:Add(2)
list:Add(3)
testObj:TestList(list)

local array = CS.System.Array.CreateInstance(typeof(CS.System.Int32), 10)
for i = 0, array.Length - 1 do
    array[i] = i
end
testObj:TestArray(array)

local dic_string_int = CS.System.Collections.Generic.Dictionary(CS.System.String, CS.System.Int32)
local dic = dic_string_int()
dic:Add('a', 1)
dic:Add('b', 2)
dic:Add('c', 3)
dic:Add('d', 4)
CS.UnityEngine.Debug.Log('LUA read dic[d]: ' .. dic:get_Item('d'))
testObj:TestDictionary(dic)
");

            Max = m_LuaEnv.Global.GetInPath<LuaMax>("math.max");
        }

        public int GetA()
        {
            return m_LuaEnv.Global.Get<int>("a");
        }
    }

    [LuaCallCSharp]
    public class LuaCallCSharpClass
    {
        public void TestList(List<int> list)
        {
            Debug.Log($"List from LUA, Count: {list.Count}, context: {string.Join(", ", list)}");
        }

        public void TestArray(int[] array)
        {
            Debug.Log($"Array from LUA, Length: {array.Length}, context: {string.Join(", ", array)}");
        }

        public void TestDictionary(Dictionary<string, int> dict)
        {
            Debug.Log($"Dictionary from LUA, Count: {dict.Count}, context: {string.Join(", ", dict.Select(kvp => $"{{{kvp.Key}:{kvp.Value}}}"))}");
        }
    }
}
