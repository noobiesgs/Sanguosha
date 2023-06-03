using System;
using Noobie.Sanguosha.Core.Players;

namespace Noobie.Sanguosha.Core.Cards
{
    public class DeckPlace : IEquatable<DeckPlace>
    {
        public DeckPlace(Player player, DeckType deckType)
        {
            Player = player;
            DeckType = deckType;
        }

        public DeckPlace(ICard card, DeckType deckType)
        {
            Card = card;
            DeckType = deckType;
        }

        public Player Player { get; }

        public DeckType DeckType { get; }

        public ICard Card { get; }

        public bool Equals(DeckPlace other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Player, other.Player) && Equals(DeckType, other.DeckType) && Equals(Card, other.Card);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DeckPlace)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Player, DeckType, Card);
        }

        public static bool operator ==(DeckPlace left, DeckPlace right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (ReferenceEquals(null, right)) return false;
            if (ReferenceEquals(null, left)) return false;
            return Equals(left.Player, right.Player) && Equals(left.DeckType, right.DeckType) && Equals(left.Card, right.Card);
        }

        public static bool operator !=(DeckPlace left, DeckPlace right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            if (Player != null)
            {
                return $"Player: {Player.Id}, DeckType: {DeckType}";
            }

            if (Card != null)
            {
                return $"Card: {Card}, DeckType: {DeckType}";
            }

            return $"Global, DeckType: {DeckType}";
        }
    }
}
