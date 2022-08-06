using System;

namespace HydraDotNet.Core.Models;


public class ItemSlugsArray
{
    public Body? body { get; set; }
    public object? metadata { get; set; }
    public byte? return_code { get; set; }

    public class Body
    {
        public Item[]? items { get; set; }
    }

    public class Item
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public string? slug { get; set; }
        public string? type_class { get; set; }
        public int max_count { get; set; }
        public bool client_access { get; set; }
        public Data? data { get; set; }
        public string? description { get; set; }
        public bool log_item_transactions { get; set; }
        public string[]? tags { get; set; }
        public Type_Options? type_options { get; set; }
        public Seed? seed { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

    public class Data
    {
        public string? AssetPath { get; set; }
        public bool IsUnlockedByDefault { get; set; }
        public Masteryrewards? MasteryRewards { get; set; }
        public string? DefaultSkin { get; set; }
        public int some_metadata { get; set; }
        public int XPValue { get; set; }
        public string? CharacterSlug { get; set; }
    }

    public class Masteryrewards
    {
        public MasteryReward[]? _1 { get; set; }
        public MasteryReward[]? _2 { get; set; }
        public MasteryReward[]? _3 { get; set; }
        public MasteryReward[]? _4 { get; set; }
        public MasteryReward[]? _5 { get; set; }
        public MasteryReward[]? _6 { get; set; }
        public MasteryReward[]? _7 { get; set; }
        public MasteryReward[]? _8 { get; set; }
        public MasteryReward[]? _9 { get; set; }
        public MasteryReward[]? _10 { get; set; }
        public MasteryReward[]? _11 { get; set; }
        public MasteryReward[]? _12 { get; set; }
        public MasteryReward[]? _14 { get; set; }
        public MasteryReward[]? _13 { get; set; }
        public MasteryReward[]? _0 { get; set; }
    }

    public class MasteryReward
    {
        public string? Type { get; set; }
        public string? GrantType { get; set; }
        public string? Slug { get; set; }
        public int Count { get; set; }
    }

    public class Type_Options
    {
        public string[]? source_slugs { get; set; }
        public Expiration? expiration { get; set; }
    }

    public class Expiration
    {
        public bool enabled { get; set; }
        public object? days { get; set; }
    }

    public class Seed
    {
        public bool override_none { get; set; }
        public Data1? data { get; set; }
        public Server_Data? server_data { get; set; }
    }

    public class Data1
    {
    }

    public class Server_Data
    {
    }
}


