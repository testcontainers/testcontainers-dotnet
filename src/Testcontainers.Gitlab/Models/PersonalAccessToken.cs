namespace Testcontainers.Gitlab.Models;

/// <summary>
/// The personal access token that is used to authenticate against the API from gitlab.
/// </summary>
public record PersonalAccessToken
{
    /// <param name="name">Name of the personal access token. If left empty a GUID will be used.</param>
    /// <param name="user">The name of the user that owns this personal access token.</param>
    /// <param name="scope">The scope that will be given to the token.</param>
    /// <param name="expirationInDays">Days until the tokens expires.</param>
    /// <summary>
    /// Name of the personal access token. If left empty a GUID will be used.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// The name of the user that owns this personal access token.
    /// </summary>
    public string User { get; set; } = string.Empty;
    /// <summary>
    /// The scope that will be given to the token.
    /// </summary>
    public PersonalAccessTokenScopes Scope { get; set; } = PersonalAccessTokenScopes.None;
    /// <summary>
    /// Days until the tokens expires.
    /// </summary>
    public int ExpirationInDays { get; set; } = 365;
    /// <summary>
    /// Internal token that is used to set the token publically. 
    /// </summary>
    internal string TokenInternal { get; set; } = string.Empty;
    /// <summary>
    /// The token that will be generated.
    /// </summary>
    public string Token => TokenInternal;
}
