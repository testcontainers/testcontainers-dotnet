namespace Testcontainers.MongoDb;

/// <summary>
/// A MongoDb shell command that evaluates a JavaScript expression.
/// </summary>
internal sealed class MongoDbShellCommand : List<string>
{
    private const string AuthFormat = "{0} --username '{1}' --password '{2}' --quiet --eval '{3}'";
    private const string NoAuthFormat = "{0} --quiet --eval '{1}'";

    private const string Sanitize = "'\"'\"'";

    private readonly StringBuilder _mongoDbShellCommand = new StringBuilder();

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbShellCommand" /> class.
    /// </summary>
    /// <remarks>
    /// The legacy mongo shell was deprecated in MongoDb 5.0 and removed in MongoDb 6.0:
    /// https://www.mongodb.com/docs/mongodb-shell/#the-mdb-shell-versus-the-legacy-mongo-shell.
    /// </remarks>
    /// <param name="js">The JavaScript expression.</param>
    /// <param name="username">The MongoDb username.</param>
    /// <param name="password">The MongoDb password.</param>
    public MongoDbShellCommand(string js, string username, string password)
    {
        var sanitizedJs = js.Replace("'", Sanitize);

        if (username != null && password != null)
        {
            var sanitizedUsername = username.Replace("'", Sanitize);
            var sanitizedPassword = password.Replace("'", Sanitize);
            _mongoDbShellCommand.AppendFormat(AuthFormat, "mongosh", sanitizedUsername, sanitizedPassword, sanitizedJs);
            _mongoDbShellCommand.Append(" || ");
            _mongoDbShellCommand.AppendFormat(AuthFormat, "mongo", sanitizedUsername, sanitizedPassword, sanitizedJs);
        }
        else
        {
            _mongoDbShellCommand.AppendFormat(NoAuthFormat, "mongosh", sanitizedJs);
            _mongoDbShellCommand.Append(" || ");
            _mongoDbShellCommand.AppendFormat(NoAuthFormat, "mongo", sanitizedJs);
        }

        Add("/bin/sh");
        Add("-c");
        Add(_mongoDbShellCommand.ToString());
    }
}