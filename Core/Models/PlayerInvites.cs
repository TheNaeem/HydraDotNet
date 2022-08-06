namespace HydraDotNet.Core.Models;


public class PlayerInvites
{
    public int total { get; set; }
    public int page { get; set; }
    public int page_size { get; set; }
    public Result[]? results { get; set; }

    public class Result
    {
        public string? id { get; set; }
        public string? sent_from { get; set; }
        public string? sent_to { get; set; }
        public Account? account { get; set; }
        public string? state { get; set; }
    }

    public class Account
    {
        public string? public_id { get; set; }
        public string? username { get; set; }
        public Avatar? avatar { get; set; }
    }

    public class Avatar
    {
        public string? name { get; set; }
        public string? image_url { get; set; }
    }
}
