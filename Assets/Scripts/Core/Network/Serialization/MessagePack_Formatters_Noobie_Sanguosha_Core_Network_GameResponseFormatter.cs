// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY MPC(MessagePack-CSharp). DO NOT CHANGE IT.
// </auto-generated>

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168
#pragma warning disable CS1591 // document public APIs

#pragma warning disable SA1403 // File may only contain a single namespace
#pragma warning disable SA1649 // File name should match first type name

namespace MessagePack.Formatters.Noobie.Sanguosha.Core.Network
{
    public sealed class GameResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Noobie.Sanguosha.Core.Network.GameResponse>
    {
        private readonly global::System.Collections.Generic.Dictionary<global::System.RuntimeTypeHandle, global::System.Collections.Generic.KeyValuePair<int, int>> typeToKeyAndJumpMap;
        private readonly global::System.Collections.Generic.Dictionary<int, int> keyToJumpMap;

        public GameResponseFormatter()
        {
            this.typeToKeyAndJumpMap = new global::System.Collections.Generic.Dictionary<global::System.RuntimeTypeHandle, global::System.Collections.Generic.KeyValuePair<int, int>>(1, global::MessagePack.Internal.RuntimeTypeHandleEqualityComparer.Default)
            {
                { typeof(global::Noobie.Sanguosha.Core.Network.AskForMultipleChoiceResponse).TypeHandle, new global::System.Collections.Generic.KeyValuePair<int, int>(101, 0) },
            };
            this.keyToJumpMap = new global::System.Collections.Generic.Dictionary<int, int>(1)
            {
                { 101, 0 },
            };
        }

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Noobie.Sanguosha.Core.Network.GameResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::System.Collections.Generic.KeyValuePair<int, int> keyValuePair;
            if (value != null && this.typeToKeyAndJumpMap.TryGetValue(value.GetType().TypeHandle, out keyValuePair))
            {
                writer.WriteArrayHeader(2);
                writer.WriteInt32(keyValuePair.Key);
                switch (keyValuePair.Value)
                {
                    case 0:
                        global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Noobie.Sanguosha.Core.Network.AskForMultipleChoiceResponse>(options.Resolver).Serialize(ref writer, (global::Noobie.Sanguosha.Core.Network.AskForMultipleChoiceResponse)value, options);
                        break;
                    default:
                        break;
                }

                return;
            }

            writer.WriteNil();
        }

        public global::Noobie.Sanguosha.Core.Network.GameResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            if (reader.ReadArrayHeader() != 2)
            {
                throw new global::System.InvalidOperationException("Invalid Union data was detected. Type:global::Noobie.Sanguosha.Core.Network.GameResponse");
            }

            options.Security.DepthStep(ref reader);
            var key = reader.ReadInt32();

            if (!this.keyToJumpMap.TryGetValue(key, out key))
            {
                key = -1;
            }

            global::Noobie.Sanguosha.Core.Network.GameResponse result = null;
            switch (key)
            {
                case 0:
                    result = (global::Noobie.Sanguosha.Core.Network.GameResponse)global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::Noobie.Sanguosha.Core.Network.AskForMultipleChoiceResponse>(options.Resolver).Deserialize(ref reader, options);
                    break;
                default:
                    reader.Skip();
                    break;
            }

            reader.Depth--;
            return result;
        }
    }


}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1403 // File may only contain a single namespace
#pragma warning restore SA1649 // File name should match first type name
