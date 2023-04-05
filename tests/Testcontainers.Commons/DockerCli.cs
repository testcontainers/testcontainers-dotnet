namespace DotNet.Testcontainers.Commons;

[PublicAPI]
public static class DockerCli
{
    [PublicAPI]
    public enum DockerPlatform
    {
        /// <summary>
        /// Docker Linux engine.
        /// </summary>
        Linux,

        /// <summary>
        /// Docker Windows engine.
        /// </summary>
        Windows,
    }

    [PublicAPI]
    public enum DockerResource
    {
        /// <summary>
        /// Docker container resource.
        /// </summary>
        Container,

        /// <summary>
        /// Docker image resource.
        /// </summary>
        Image,

        /// <summary>
        /// Docker network resource.
        /// </summary>
        Network,

        /// <summary>
        /// Docker volume resource.
        /// </summary>
        Volume,
    }

    public static bool PlatformIsEnabled(DockerPlatform platform)
    {
        var commandResult = new Command("version", "--format '{{.Server.Os}}'").Execute();
        return 0.Equals(commandResult.ExitCode) && commandResult.Stdout.Contains(platform.ToString().ToLowerInvariant());
    }

    public static bool ResourceExists(DockerResource resource, string id)
    {
        var commandResult = new Command("inspect", "--type=" + resource.ToString().ToLowerInvariant(), id).Execute();
        return 0.Equals(commandResult.ExitCode);
    }

    [PublicAPI]
    private sealed class Command
    {
        private readonly ProcessStartInfo _processStartInfo = new ProcessStartInfo();

        private readonly StringBuilder _stdout = new StringBuilder();

        private readonly StringBuilder _stderr = new StringBuilder();

        public Command(params string[] arguments)
        {
            const string executable = "docker";
            _processStartInfo.FileName = executable;
            _processStartInfo.Arguments = string.Join(" ", arguments);
            _processStartInfo.RedirectStandardOutput = true;
            _processStartInfo.RedirectStandardError = true;
            _processStartInfo.UseShellExecute = false;
        }

        public CommandResult Execute()
        {
            DateTime startTime;

            DateTime exitTime;

            var process = new Process();
            process.StartInfo = _processStartInfo;
            process.OutputDataReceived += AppendStdout;
            process.ErrorDataReceived += AppendStderr;

            try
            {
                startTime = DateTime.UtcNow;
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
            finally
            {
                exitTime = DateTime.UtcNow;
                process.OutputDataReceived -= AppendStdout;
                process.ErrorDataReceived -= AppendStderr;
            }

            return new CommandResult(process.ExitCode, startTime, exitTime, _stdout.ToString(), _stderr.ToString());
        }

        private void AppendStdout(object sender, DataReceivedEventArgs e)
        {
            _stdout.Append(e.Data);
        }

        private void AppendStderr(object sender, DataReceivedEventArgs e)
        {
            _stderr.Append(e.Data);
        }
    }

    [PublicAPI]
    private sealed class CommandResult
    {
        public CommandResult(int exitCode, DateTime startTime, DateTime exitTime, string stdout, string stderr)
        {
            ExitCode = exitCode;
            StartTime = startTime;
            ExitTime = exitTime;
            Stdout = stdout;
            Stderr = stderr;
        }

        public int ExitCode { get; }

        public DateTime StartTime { get; }

        public DateTime ExitTime { get; }

        public string Stdout { get; }

        public string Stderr { get; }
    }
}