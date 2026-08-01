namespace DotNet.Testcontainers.Images
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using Docker.DotNet.Models;
  using JetBrains.Annotations;

  /// <summary>
  /// Represents an exception that is thrown when a Docker image build has failed.
  /// </summary>
  [PublicAPI]
  public sealed class ImageBuildFailedException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageBuildFailedException" /> class.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    /// <param name="errors">The image build errors.</param>
    public ImageBuildFailedException(IImage image, IEnumerable<JSONError> errors)
      : base(CreateMessage(image, errors.ToArray()))
    {
    }

    private static string CreateMessage(IImage image, IReadOnlyCollection<JSONError> errors)
    {
      var exceptionInfo = new StringBuilder(256);
      exceptionInfo.Append($"Docker image {image.FullName} has not been created.");

      if (errors.Count > 0)
      {
        var formattedErrors = errors
          .Where(error => error != null)
          .Select(error => "    " + error.Message);

        exceptionInfo.AppendLine();
        exceptionInfo.AppendLine("  Error: ");
        exceptionInfo.Append(string.Join(Environment.NewLine, formattedErrors));
      }

      return exceptionInfo.ToString();
    }
  }
}
