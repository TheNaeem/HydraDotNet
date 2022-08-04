using System;

namespace HydraDotNet.Core.Models;


public class EpicExternalAuthResponse
{
    public string? scope { get; set; }
    public string? token_type { get; set; }
    public string? access_token { get; set; }
    public string? refresh_token { get; set; }
    public string? id_token { get; set; }
    public int expires_in { get; set; }
    public DateTime? expires_at { get; set; }
    public int? refresh_expires_in { get; set; }
    public DateTime? refresh_expires_at { get; set; }
    public string? account_id { get; set; }
    public string? client_id { get; set; }
    public string? application_id { get; set; }
    public string? selected_account_id { get; set; }
}

