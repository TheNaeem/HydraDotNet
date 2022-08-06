using System;

namespace HydraDotNet.Core.Models;

public class WarnerAccount
{
    public string? id { get; set; }
    public DateTime updated_at { get; set; }
    public string? public_id { get; set; }
    public string? email { get; set; }
    public bool email_verified { get; set; }
    public object? email_pending { get; set; }
    public bool password_set { get; set; }
    public bool mfa_active { get; set; }
    public string? username { get; set; }
    public DateTime username_updated_at { get; set; }
    public int username_changes_available { get; set; }
    public DateTime date_of_birth { get; set; }
    public DateTime implied_date_of_birth { get; set; }
    public string? locale { get; set; }
    public string? country { get; set; }
    public string? territory { get; set; }
    public Avatar? avatar { get; set; }
    public bool marketing_opt_in { get; set; }
    public bool tos_consent { get; set; }
    public TwitchLink? twitch_link { get; set; }
    public object? discord_link { get; set; }
    public EpicGamesLink? epic_games_link { get; set; }
    public object? google_link { get; set; }
    public SteamLink? steam_link { get; set; }
    public object? psn_link { get; set; }
    public object? apple_link { get; set; }
    public object? nintendo_link { get; set; }
    public object? xbox_link { get; set; }
    public object? wizarding_world_link { get; set; }
    public bool completed { get; set; }
    public DateTime last_login { get; set; }
    public DateTime created_at { get; set; }
    public GameLink[]? game_links { get; set; }

    public class Avatar
    {
        public string? name { get; set; }
        public string? image_url { get; set; }
        public string? slug { get; set; }
    }

    public class EpicGamesLink
    {
        public string? auth_id { get; set; }
        public string? last_seen_username { get; set; }
        public DateTime updated_at { get; set; }
        public object? pending { get; set; }
    }

    public class GameLink
    {
        public string? game { get; set; }
        public string? public_id { get; set; }
        public string? last_seen_platform { get; set; }
        public DateTime last_game_login { get; set; }
        public DateTime? last_accessed { get; set; }
    }

    public class SteamLink
    {
        public string? auth_id { get; set; }
        public string? last_seen_username { get; set; }
        public DateTime updated_at { get; set; }
        public object? pending { get; set; }
    }

    public class TwitchLink
    {
        public string? auth_id { get; set; }
        public string? last_seen_username { get; set; }
        public DateTime updated_at { get; set; }
        public object? pending { get; set; }
        public DateTime created_at { get; set; }
    }

}
