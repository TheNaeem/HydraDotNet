namespace HydraDotNet.Core.Models;

public class HydraSelectCharacterRequestBody
{
    public string? CharacterPath { get; set; }
    public Characterpreferences? CharacterPreferences { get; set; }
    public string? CharacterSlug { get; set; }
    public Equippable? Equippables { get; set; }
    public bool IsDefaultSkin { get; set; }
    public string? PartyId { get; set; }
    public string? SkinPath { get; set; }
    public string? SkinSlug { get; set; }
    public int StatBadgeCount { get; set; }
    public string? StatBadgeSlug { get; set; }
    public string? Username { get; set; }

    public class Characterpreferences
    {
        public Skindata? SkinData { get; set; }
        public Tauntdata[]? TauntDatas { get; set; }
    }

    public class Skindata
    {
        public string? AssetPath { get; set; }
        public string? Slug { get; set; }
    }

    public class Tauntdata
    {
        public string? AssetPath { get; set; }
        public string? Slug { get; set; }
    }

    public class Equippable
    {
        public Announcerpack? AnnouncerPack { get; set; }
        public Banner? Banner { get; set; }
        public Ringoutvfx? RingOutVfx { get; set; }
        public Stattracker? StatTracker { get; set; }
    }

    public class Announcerpack
    {
        public string? AssetPath { get; set; }
        public string? Slug { get; set; }
    }

    public class Banner
    {
        public string? AssetPath { get; set; }
        public string? Slug { get; set; }
    }

    public class Ringoutvfx
    {
        public string? AssetPath { get; set; }
        public string? Slug { get; set; }
    }

    public class Stattracker
    {
        public string? AssetPath { get; set; }
        public string? Slug { get; set; }
    }
}

