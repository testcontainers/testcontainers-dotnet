namespace TestContainers.Mosquitto;

internal static class StringBuilderExtensions
{
	public static StringBuilder AppendUnixLine(this StringBuilder sb, string value = "")
	{
		return sb
			.Append(value)
			.Append('\n');
	}
}
