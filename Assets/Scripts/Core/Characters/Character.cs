using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using Noobie.Sanguosha.Core.Skills;

namespace Noobie.Sanguosha.Core.Characters
{
    public class Character
    {
        public Character(string name, Allegiance allegiance, CharacterSymbol symbol, string replaceFrom, int maxHealth, params ISkill[] skills)
            : this(name, allegiance, symbol, replaceFrom, maxHealth, maxHealth, 0, skills)
        {

        }

        public Character(string name, Allegiance allegiance, CharacterSymbol symbol, string replaceFrom, int health, int maxHealth, params ISkill[] skills)
            : this(name, allegiance, symbol, replaceFrom, health, maxHealth, 0, skills)
        {

        }

        public Character(string name, Allegiance allegiance, int health, int maxHealth, params ISkill[] skills)
            : this(name, allegiance, CharacterSymbol.Normal, string.Empty, health, maxHealth, 0, skills)
        {

        }

        public Character(string name, Allegiance allegiance, int maxHealth, params ISkill[] skills)
        : this(name, allegiance, CharacterSymbol.Normal, string.Empty, maxHealth, maxHealth, 0, skills)
        {

        }

        public Character(
            string name,
            Allegiance allegiance,
            CharacterSymbol symbol,
            string replaceFrom,
            int health,
            int maxHealth,
            int armor,
            params ISkill[] skills) : this(name, allegiance, symbol, replaceFrom, health, maxHealth, armor, (IEnumerable<ISkill>)skills)
        {

        }

        public Character(
            [NotNull] string name,
            Allegiance allegiance,
            CharacterSymbol symbol,
            string replaceFrom,
            int health,
            int maxHealth,
            int armor,
            IEnumerable<ISkill> skills)
        {
            if (health > maxHealth || health <= 0 || maxHealth <= 0)
            {
                throw new ArgumentException("Health must be greater than 0 and less than or equal to max health.");
            }

            Name = name ?? throw new ArgumentNullException(nameof(name));
            Allegiance = allegiance;
            Symbol = symbol;
            ReplaceFrom = replaceFrom;
            Health = health;
            MaxHealth = maxHealth;
            Armor = armor;
            Skills = new ReadOnlyCollection<ISkill>(new List<ISkill>(skills));
        }

        public Allegiance Allegiance { get; }

        public string Name { get; }

        public string ReplaceFrom { get; }

        public CharacterSymbol Symbol { get; }

        public int Health { get; }

        public int MaxHealth { get; }

        public int Armor { get; }

        public IReadOnlyList<ISkill> Skills { get; }

        public bool IsReplacementCharacter => !string.IsNullOrEmpty(ReplaceFrom);
    }
}
