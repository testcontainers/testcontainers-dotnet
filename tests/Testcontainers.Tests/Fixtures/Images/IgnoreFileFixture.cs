namespace DotNet.Testcontainers.Tests.Fixtures
{
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Images;
  using Xunit;

  public sealed class IgnoreFileFixture : TheoryData<IgnoreFile, string, bool>
  {
    public IgnoreFileFixture()
    {
      var logger = TestcontainersSettings.Logger;
      var ignoreNonRecursiveFiles = new IgnoreFile(new[] { "*/temp*" }, logger);
      var ignoreNonRecursiveNestedFiles = new IgnoreFile(new[] { "*/*/temp*" }, logger);
      var ignoreRecursiveFiles = new IgnoreFile(new[] { "**/*.txt" }, logger);
      var ignoreSingleCharacterFiles = new IgnoreFile(new[] { "temp?" }, logger);
      var ignoreExceptionFiles = new IgnoreFile(new[] { "*.md", "!README*.md", "README-secret.md" }, logger);
      this.Add(ignoreNonRecursiveFiles, "lipsum/temp", false);
      this.Add(ignoreNonRecursiveFiles, "lipsum/temp.txt", false);
      this.Add(ignoreNonRecursiveFiles, "lipsum/lorem/temp", true);
      this.Add(ignoreNonRecursiveNestedFiles, "lipsum/lorem/temp", false);
      this.Add(ignoreNonRecursiveNestedFiles, "lipsum/lorem/temp.txt", false);
      this.Add(ignoreNonRecursiveNestedFiles, "lipsum/temp", true);
      this.Add(ignoreNonRecursiveNestedFiles, "lipsum/lorem/lipsum/temp", true);
      this.Add(ignoreRecursiveFiles, "lipsum.txt", false);
      this.Add(ignoreRecursiveFiles, "lorem/lipsum/lorem/lipsum.txt", false);
      this.Add(ignoreRecursiveFiles, "lorem/lipsum/lorem.txt", false);
      this.Add(ignoreRecursiveFiles, "lorem/lipsum.txt", false);
      this.Add(ignoreRecursiveFiles, "lorem/lipsum/lipsum.config", true);
      this.Add(ignoreRecursiveFiles, "lorem/lipsum.config", true);
      this.Add(ignoreSingleCharacterFiles, "temp", false);
      this.Add(ignoreSingleCharacterFiles, "temp1", false);
      this.Add(ignoreSingleCharacterFiles, "temp12", true);
      this.Add(ignoreSingleCharacterFiles, "lipsum/temp", true);
      this.Add(ignoreExceptionFiles, "CODE_OF_CONDUCT.md", false);
      this.Add(ignoreExceptionFiles, "README-secret.md", false);
      this.Add(ignoreExceptionFiles, "README.md", true);
    }
  }
}
