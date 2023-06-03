// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY MPC(MessagePack-CSharp). DO NOT CHANGE IT.
// </auto-generated>

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168
#pragma warning disable CS1591 // document public APIs

#pragma warning disable SA1312 // Variable names should begin with lower-case letter
#pragma warning disable SA1649 // File name should match first type name

namespace MessagePack.Resolvers
{
    public class GeneratedResolver : global::MessagePack.IFormatterResolver
    {
        public static readonly global::MessagePack.IFormatterResolver Instance = new GeneratedResolver();

        private GeneratedResolver()
        {
        }

        public global::MessagePack.Formatters.IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        private static class FormatterCache<T>
        {
            internal static readonly global::MessagePack.Formatters.IMessagePackFormatter<T> Formatter;

            static FormatterCache()
            {
                var f = GeneratedResolverGetFormatterHelper.GetFormatter(typeof(T));
                if (f != null)
                {
                    Formatter = (global::MessagePack.Formatters.IMessagePackFormatter<T>)f;
                }
            }
        }
    }

    internal static class GeneratedResolverGetFormatterHelper
    {
        private static readonly global::System.Collections.Generic.Dictionary<global::System.Type, int> lookup;

        static GeneratedResolverGetFormatterHelper()
        {
            lookup = new global::System.Collections.Generic.Dictionary<global::System.Type, int>(5)
            {
                { typeof(global::Noobie.Sanguosha.Core.Network.GameDataPacket), 0 },
                { typeof(global::Noobie.Sanguosha.Core.Network.GameResponse), 1 },
                { typeof(global::Noobie.Sanguosha.Core.Network.GameUpdate), 2 },
                { typeof(global::Noobie.Sanguosha.Core.Network.AskForMultipleChoiceResponse), 3 },
                { typeof(global::Noobie.Sanguosha.Core.Network.StatusSync), 4 },
            };
        }

        internal static object GetFormatter(global::System.Type t)
        {
            int key;
            if (!lookup.TryGetValue(t, out key))
            {
                return null;
            }

            switch (key)
            {
                case 0: return new MessagePack.Formatters.Noobie.Sanguosha.Core.Network.GameDataPacketFormatter();
                case 1: return new MessagePack.Formatters.Noobie.Sanguosha.Core.Network.GameResponseFormatter();
                case 2: return new MessagePack.Formatters.Noobie.Sanguosha.Core.Network.GameUpdateFormatter();
                case 3: return new MessagePack.Formatters.Noobie.Sanguosha.Core.Network.AskForMultipleChoiceResponseFormatter();
                case 4: return new MessagePack.Formatters.Noobie.Sanguosha.Core.Network.StatusSyncFormatter();
                default: return null;
            }
        }
    }
}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1312 // Variable names should begin with lower-case letter
#pragma warning restore SA1649 // File name should match first type name
