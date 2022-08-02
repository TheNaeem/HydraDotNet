using HydraDotNet.Core.Encoding;

namespace HydraDotNet.Core.Objects;

class HydraCalendarControl
{
    public HydraCalendarControl(HydraDecoder decoder)
    {
        Def = decoder.ReadValue();
        Rendered = decoder.ReadValue();
    }

    public object? Def { get; private set; }
    public object? Rendered { get; private set; }
}
