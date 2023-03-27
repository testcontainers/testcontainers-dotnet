namespace DotNet.Testcontainers.Tests.Fixtures
{
  using DotNet.Testcontainers.Images;
  using Microsoft.Extensions.Logging.Abstractions;
  using Xunit;

  public sealed class IgnoreFileFixture : TheoryData<IgnoreFile, string, bool>
  {
    public IgnoreFileFixture()
    {
      var logger = NullLogger.Instance;
      var ignoreFilesAndDirectories = new IgnoreFile(new[] { "bin/", "obj/*" }, logger);
      var ignoreAllFilesAndDirectories = new IgnoreFile(new[] { "*", "!README*.md" }, logger);
      var ignoreNonRecursiveFiles = new IgnoreFile(new[] { "*/temp*" }, logger);
      var ignoreNonRecursiveNestedFiles = new IgnoreFile(new[] { "*/*/temp*" }, logger);
      var ignoreRecursiveFiles = new IgnoreFile(new[] { "**/*.txt" }, logger);
      var ignoreSingleCharacterFiles = new IgnoreFile(new[] { "temp?" }, logger);
      var ignoreExceptionFiles = new IgnoreFile(new[] { "*.md", "!README*.md", "README-secret.md" }, logger);
      Add(ignoreFilesAndDirectories, "bin/Debug", false);
      Add(ignoreFilesAndDirectories, "obj/Debug", false);
      Add(ignoreFilesAndDirectories, "README.md", true);
      Add(ignoreAllFilesAndDirectories, "bin/Debug", false);
      Add(ignoreAllFilesAndDirectories, "obj/Debug", false);
      Add(ignoreAllFilesAndDirectories, "README.md", true);
      Add(ignoreNonRecursiveFiles, "lipsum/temp", false);
      Add(ignoreNonRecursiveFiles, "lipsum/temp.txt", false);
      Add(ignoreNonRecursiveFiles, "lipsum/lorem/temp", true);
      Add(ignoreNonRecursiveNestedFiles, "lipsum/lorem/temp", false);
      Add(ignoreNonRecursiveNestedFiles, "lipsum/lorem/temp.txt", false);
      Add(ignoreNonRecursiveNestedFiles, "lipsum/temp", true);
      Add(ignoreNonRecursiveNestedFiles, "lipsum/lorem/lipsum/temp", true);
      Add(ignoreRecursiveFiles, "lipsum.txt", false);
      Add(ignoreRecursiveFiles, "lorem/lipsum/lorem/lipsum.txt", false);
      Add(ignoreRecursiveFiles, "lorem/lipsum/lorem.txt", false);
      Add(ignoreRecursiveFiles, "lorem/lipsum.txt", false);
      Add(ignoreRecursiveFiles, "lorem/lipsum/lipsum.config", true);
      Add(ignoreRecursiveFiles, "lorem/lipsum.config", true);
      Add(ignoreSingleCharacterFiles, "temp", false);
      Add(ignoreSingleCharacterFiles, "temp1", false);
      Add(ignoreSingleCharacterFiles, "temp12", true);
      Add(ignoreSingleCharacterFiles, "lipsum/temp", true);
      Add(ignoreExceptionFiles, "CODE_OF_CONDUCT.md", false);
      Add(ignoreExceptionFiles, "README-secret.md", false);
      Add(ignoreExceptionFiles, "README.md", true);
    }
  }
}
