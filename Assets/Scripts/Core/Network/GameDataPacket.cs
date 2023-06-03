using MessagePack;

namespace Noobie.Sanguosha.Core.Network
{
    [MessagePackObject]
    [Union(100, typeof(GameResponse))]
    [Union(101, typeof(AskForMultipleChoiceResponse))]
    [Union(200, typeof(GameUpdate))]
    [Union(201, typeof(StatusSync))]
    public abstract class GameDataPacket
    {
        [Key(0)]
        public float Timestamp { get; set; }
    }

    [MessagePackObject]
    [Union(201, typeof(StatusSync))]
    public abstract class GameUpdate : GameDataPacket
    {
    }

    [MessagePackObject]
    [Union(101, typeof(AskForMultipleChoiceResponse))]
    public abstract class GameResponse : GameDataPacket
    {
        [Key(1)]
        public int Id { get; set; }
    }

    [MessagePackObject]
    public class StatusSync : GameUpdate
    {
        [Key(1)]
        public int Status { get; set; }
    }

    [MessagePackObject]
    public class AskForMultipleChoiceResponse : GameResponse
    {
        public AskForMultipleChoiceResponse(int choiceIndex) { ChoiceIndex = choiceIndex; }

        public AskForMultipleChoiceResponse() { }

        [Key(2)]
        public int ChoiceIndex { get; set; }
    }
}
