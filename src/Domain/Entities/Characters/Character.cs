using System.Collections.Generic;
using Crpg.Domain.Common;
using Crpg.Domain.Entities.Items;
using Crpg.Domain.Entities.Users;

namespace Crpg.Domain.Entities.Characters
{
    /// <summary>
    /// Represents a cRPG character.
    /// </summary>
    public class Character : AuditableEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Generation { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public float ExperienceMultiplier { get; set; }
        public bool SkippedTheFun { get; set; }
        public bool AutoRepair { get; set; }

        public CharacterStatistics Statistics { get; set; } = new();
        public IList<EquippedItem> EquippedItems { get; set; } = new List<EquippedItem>();

        public User? User { get; set; }
    }
}
