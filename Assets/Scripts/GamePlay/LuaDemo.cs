using System;
using Noobie.Sanguosha.Core;
using Noobie.Sanguosha.Core.Lua;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

public class LuaDemo : MonoBehaviour
{
    private LuaEnv m_LuaEnv;

    [SerializeField] Text m_Text;

    // Start is called before the first frame update
    void Start()
    {
        m_LuaEnv = new LuaEnv();

        var luaDemo = new LoadLuaScriptDemo(m_LuaEnv);
        luaDemo.Run();

        var a = luaDemo.GetA();
        m_Text.text = $"a from lua = {a}{Environment.NewLine}LUA: max(32, 12) = {luaDemo.Max(32, 12)}{Environment.NewLine}{Translator.Translate("hello")}";
    }

    // Update is called once per frame
    void Update()
    {
        m_LuaEnv?.Tick();
    }

    void OnDestroy()
    {
        m_LuaEnv?.Dispose();
    }
}
