using Testcontainers.Gitlab.Models;
using Testcontainers.Gitlab.RegexPatterns;

namespace Testcontainers.Gitlab;

/// <inheritdoc cref="DockerContainer" />
/// <summary>
/// Initializes a new instance of the <see cref="GitlabContainer" /> class.
/// </summary>
/// <param name="configuration">The container configuration.</param>
[PublicAPI]
public sealed class GitlabContainer(GitlabConfiguration configuration) : DockerContainer(configuration)
{
    /// <summary>
    /// Gets the root password.
    /// </summary>
    public string Password => configuration.Password;


    /// <summary>
    /// Generate a personal access token.
    /// </summary>
    /// <param name="pat">The personal access token to create.</param>
    /// <returns></returns>
    /// <exception cref="DataMisalignedException"></exception>
    public async Task<PersonalAccessToken> GenerateAccessToken(PersonalAccessToken pat)
    {
        var scope = "[" + '\'' + pat.Scope.ToString().Replace(", ", "\', \'") + '\'' + "]";

        var command = $"token = User.find_by_username('{pat.User}')" +
            $".personal_access_tokens" +
            $".create(name: '{pat.Name}', scopes: {scope}, expires_at: {pat.ExpirationInDays}.days.from_now); " +
            $"puts token.cleartext_tokens";

        var tokenCommand = new List<string>{
            { "gitlab-rails" },
            { "runner" },
            { command }
        };

        ExecResult tokenResult = await ExecAsync(tokenCommand);

        string token;
        if (tokenResult.ExitCode == 0)
        {
            var match = GitlabRegex.GitlabPersonalAccessToken.Match(tokenResult.Stdout);
            token = match.Value;
        }
        else
        {
            throw new DataMisalignedException("Stderr: " + tokenResult.Stderr + "|" + "Stdout: " + tokenResult.Stdout);
        }
        pat.TokenInternal = token;
        return pat;
    }

    /// <summary>
    /// Generate a personal access token.
    /// </summary>
    /// <param name="name">Name of the personal access token. If left empty a GUID will be used.</param>
    /// <param name="user">The name of the user that owns this personal access token.</param>
    /// <param name="scope">The scope that will be given to the token.</param>
    /// <param name="expirationInDays">Days until the tokens expires.</param>
    /// <returns></returns>
    public async Task<PersonalAccessToken> GenerateAccessToken(string user, PersonalAccessTokenScopes scope, string name = "", int expirationInDays = 365)
        => await GenerateAccessToken(new PersonalAccessToken
        {
            Name = string.IsNullOrWhiteSpace(name) ? Guid.NewGuid().ToString() : name,
            User = user,
            Scope = scope,
            ExpirationInDays = expirationInDays
        });
}