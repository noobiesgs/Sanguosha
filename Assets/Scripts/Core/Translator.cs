using System.Collections.Generic;
using XLua;

namespace Noobie.Sanguosha.Core
{
    [LuaCallCSharp]
    public static class Translator
    {
        private static readonly Dictionary<string, string> k_Translations = new();

        public static void AddTranslation(string key, string value)
        {
            k_Translations[key] = value;
        }

        public static string Translate(string key)
        {
            return k_Translations.TryGetValue(key, out var value) ? value : key;
        }
    }
}
