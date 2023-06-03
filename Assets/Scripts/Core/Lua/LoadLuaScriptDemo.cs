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
a=5
a=a+1

local L = CS.Noobie.Sanguosha.Core.Translator

L.AddTranslation('hello', '你好')
");

            Max = m_LuaEnv.Global.GetInPath<LuaMax>("math.max");
        }

        public int GetA()
        {
            return m_LuaEnv.Global.Get<int>("a");
        }
    }
}
