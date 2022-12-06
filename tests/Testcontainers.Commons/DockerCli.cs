namespace DotNet.Testcontainers.Commons
{
  using System;
  using System.Diagnostics;
  using System.Text;
  using JetBrains.Annotations;

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
      private readonly ProcessStartInfo processStartInfo = new ProcessStartInfo();

      private readonly StringBuilder stdout = new StringBuilder();

      private readonly StringBuilder stderr = new StringBuilder();

      public Command(params string[] arguments)
      {
        const string executable = "docker";
        this.processStartInfo.FileName = executable;
        this.processStartInfo.Arguments = string.Join(" ", arguments);
        this.processStartInfo.RedirectStandardOutput = true;
        this.processStartInfo.RedirectStandardError = true;
        this.processStartInfo.UseShellExecute = false;
      }

      public CommandResult Execute()
      {
        DateTime startTime;

        DateTime exitTime;

        var process = new Process();
        process.StartInfo = this.processStartInfo;
        process.OutputDataReceived += this.AppendStdout;
        process.ErrorDataReceived += this.AppendStderr;

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
          process.OutputDataReceived -= this.AppendStdout;
          process.ErrorDataReceived -= this.AppendStderr;
        }

        return new CommandResult(process.ExitCode, startTime, exitTime, this.stdout.ToString(), this.stderr.ToString());
      }

      private void AppendStdout(object sender, DataReceivedEventArgs e)
      {
        this.stdout.Append(e.Data);
      }

      private void AppendStderr(object sender, DataReceivedEventArgs e)
      {
        this.stderr.Append(e.Data);
      }
    }

    [PublicAPI]
    private sealed class CommandResult
    {
      public CommandResult(int exitCode, DateTime startTime, DateTime exitTime, string stdout, string stderr)
      {
        this.ExitCode = exitCode;
        this.StartTime = startTime;
        this.ExitTime = exitTime;
        this.Stdout = stdout;
        this.Stderr = stderr;
      }

      public int ExitCode { get; }

      public DateTime StartTime { get; }

      public DateTime ExitTime { get; }

      public string Stdout { get; }

      public string Stderr { get; }
    }
  }
}
