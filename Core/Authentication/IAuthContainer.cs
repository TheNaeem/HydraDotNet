namespace HydraDotNet.Core.Authentication;

public interface IAuthContainer
{
    string AccessToken { get; }

    public void UpdateToken();
}

