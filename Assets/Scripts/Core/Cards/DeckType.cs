using System;
using System.Collections.Generic;

namespace Noobie.Sanguosha.Core.Cards
{
    public class DeckType : IEquatable<DeckType>
    {
        private static readonly Dictionary<string, DeckType> k_RegisteredDeckTypes = new();

        public static DeckType Register(string name)
        {
            return Register(name, name);
        }

        public static DeckType Register(string name, string shortName)
        {
            if (!k_RegisteredDeckTypes.ContainsKey(shortName))
            {
                k_RegisteredDeckTypes.Add(shortName, new DeckType(name, shortName));
            }

            return k_RegisteredDeckTypes[shortName];
        }

        protected DeckType(string name, string shortName)
        {
            Name = name;
            AbbreviatedName = shortName;
        }

        public string Name { get; }

        /// <summary>
        /// Sets/gets abbreviated name used to uniquely identify and serialize this DeckType.
        /// </summary>
        public string AbbreviatedName { get; }


        public bool Equals(DeckType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DeckType)obj);
        }

        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return Name;
        }

        public static bool operator ==(DeckType left, DeckType right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (ReferenceEquals(null, right)) return false;
            if (ReferenceEquals(null, left)) return false;
            return left.Name == right.Name;
        }

        public static bool operator !=(DeckType left, DeckType right)
        {
            return !(left == right);
        }

        public static DeckType Dealing = Register("Dealing", "0");
        public static DeckType Discard = Register("Discard", "1");
        public static DeckType Compute = Register("Compute", "2");
        public static DeckType Hand = Register("Hand", "3");
        public static DeckType Equipment = Register("Equipment", "4");
        public static DeckType DelayedTools = Register("DelayedTools", "5");
        public static DeckType JudgeResult = Register("JudgeResult", "6");
        public static DeckType GuHuo = Register("GuHuo", "7");
        public static DeckType None = Register("None", "8");
        public static DeckType Characters = Register("Characters", "9");
        public static DeckType ReplacementCharacters = Register("ReplacementCharacters", "a");
        public static DeckType MuNiuLiuMa = Register("MuNiuLiuMa", "b");
    }
}
