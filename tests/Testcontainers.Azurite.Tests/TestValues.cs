namespace Testcontainers.Azurite;

internal static class TestValues
{
    /// <summary>
    /// The account name for internal tests.
    /// </summary>
    public static readonly string AccountName = "testaccount1";

    /// <summary>
    /// The account key for internal tests.
    /// </summary>
    /// <remarks>Base64 encoded "Hello World" string.</remarks>
    public static readonly string AccountKey = "SGVsbG8gV29ybGQ=";
}
