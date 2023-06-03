// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY MPC(MessagePack-CSharp). DO NOT CHANGE IT.
// </auto-generated>

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168
#pragma warning disable CS1591 // document public APIs

#pragma warning disable SA1129 // Do not use default value type constructor
#pragma warning disable SA1309 // Field names should not begin with underscore
#pragma warning disable SA1312 // Variable names should begin with lower-case letter
#pragma warning disable SA1403 // File may only contain a single namespace
#pragma warning disable SA1649 // File name should match first type name

namespace MessagePack.Formatters.Noobie.Sanguosha.Core.Network
{
    public sealed class AskForMultipleChoiceResponseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Noobie.Sanguosha.Core.Network.AskForMultipleChoiceResponse>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Noobie.Sanguosha.Core.Network.AskForMultipleChoiceResponse value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(3);
            writer.Write(value.Timestamp);
            writer.Write(value.Id);
            writer.Write(value.ChoiceIndex);
        }

        public global::Noobie.Sanguosha.Core.Network.AskForMultipleChoiceResponse Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var ____result = new global::Noobie.Sanguosha.Core.Network.AskForMultipleChoiceResponse();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.Timestamp = reader.ReadSingle();
                        break;
                    case 1:
                        ____result.Id = reader.ReadInt32();
                        break;
                    case 2:
                        ____result.ChoiceIndex = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1129 // Do not use default value type constructor
#pragma warning restore SA1309 // Field names should not begin with underscore
#pragma warning restore SA1312 // Variable names should begin with lower-case letter
#pragma warning restore SA1403 // File may only contain a single namespace
#pragma warning restore SA1649 // File name should match first type name