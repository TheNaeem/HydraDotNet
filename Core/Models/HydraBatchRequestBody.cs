using System.Collections.Generic;

namespace HydraDotNet.Core.Models;

public class HydraBatchRequestBody
{
    public Options? options { get; set; }
    public List<Request>? requests { get; set; }

    public class Options
    {
        public bool allow_failures { get; set; } = false;
    }

    public class Request
    {
        public Headers? headers { get; set; }
    }

    public class Headers
    {
        public string? url { get; set; }
        public string? verb { get; set; }
        public object? Collection { get; set; } = null;
    }
}

