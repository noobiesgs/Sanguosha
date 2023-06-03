using System;

namespace Noobie.Sanguosha.Core.Characters
{
    [Flags]
    public enum Allegiance : ushort
    {
        Unknown = 0,
        Shu = 1,
        Wei = 1 << 1,
        Wu = 1 << 2,
        Qun = 1 << 3,
        Jin = 1 << 4,
        God = 1 << 5
    }
}
