﻿using System;
using System.Collections.Generic;
using Crpg.GameMod.Api.Models.Users;

namespace Crpg.GameMod.Api.Models
{
    // Copy of Crpg.Application.Games.Models.GameUserUpdate
    internal class CrpgGameUserUpdate
    {
        public Platform Platform { get; set; }
        public string PlatformUserId { get; set; } = string.Empty;
        public string CharacterName { get; set; } = string.Empty;
        public CrpgUserReward? Reward { get; set; }
        public IList<CrpgUserBrokenItem> BrokenItems { get; set; } = Array.Empty<CrpgUserBrokenItem>();
    }
}
