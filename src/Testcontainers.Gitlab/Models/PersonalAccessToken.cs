namespace Testcontainers.Gitlab.Models;

public record PersonalAccessToken
{
    public string Name { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public PersonalAccessTokenScopes Scope { get; set; } = PersonalAccessTokenScopes.None;
    public int ExpirationInDays { get; set; } = 365;
    internal string TokenInternal { get; set; } = string.Empty;
    public string Token => TokenInternal;
}
