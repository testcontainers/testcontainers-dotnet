namespace DotNet.Testcontainers.Tests.Fixtures
{
  using DotNet.Testcontainers.Images.Archives;
  using Xunit;

  internal class IgnoreFileTestFixture : TheoryData<IgnoreFile, string, bool>
  {
    private static readonly IgnoreFile IgnoreNonRecursiveFiles = new IgnoreFile("*/temp*");

    private static readonly IgnoreFile IgnoreNonRecursiveNestedFiles = new IgnoreFile("*/*/temp*");

    private static readonly IgnoreFile IgnoreRecursiveFiles = new IgnoreFile("**/*.txt");

    private static readonly IgnoreFile IgnoreSingleCharacterFiles = new IgnoreFile("temp?");

    private static readonly IgnoreFile IgnoreExceptionFiles = new IgnoreFile("*.md", "!README*.md", "README-secret.md");

    public IgnoreFileTestFixture()
    {
      this.Add(IgnoreNonRecursiveFiles, "lipsum/temp", false);
      this.Add(IgnoreNonRecursiveFiles, "lipsum/temp.txt", false);
      this.Add(IgnoreNonRecursiveFiles, "lipsum/lorem/temp", true);
      this.Add(IgnoreNonRecursiveNestedFiles, "lipsum/lorem/temp", false);
      this.Add(IgnoreNonRecursiveNestedFiles, "lipsum/lorem/temp.txt", false);
      this.Add(IgnoreNonRecursiveNestedFiles, "lipsum/temp", true);
      this.Add(IgnoreNonRecursiveNestedFiles, "lipsum/lorem/lipsum/temp", true);
      this.Add(IgnoreRecursiveFiles, "lipsum.txt", false);
      this.Add(IgnoreRecursiveFiles, "lorem/lipsum/lorem/lipsum.txt", false);
      this.Add(IgnoreRecursiveFiles, "lorem/lipsum/lorem.txt", false);
      this.Add(IgnoreRecursiveFiles, "lorem/lipsum.txt", false);
      this.Add(IgnoreRecursiveFiles, "lorem/lipsum/lipsum.config", true);
      this.Add(IgnoreRecursiveFiles, "lorem/lipsum.config", true);
      this.Add(IgnoreSingleCharacterFiles, "temp", false);
      this.Add(IgnoreSingleCharacterFiles, "temp1", false);
      this.Add(IgnoreSingleCharacterFiles, "temp12", true);
      this.Add(IgnoreSingleCharacterFiles, "lipsum/temp", true);
      this.Add(IgnoreExceptionFiles, "CODE_OF_CONDUCT.md", false);
      this.Add(IgnoreExceptionFiles, "README-secret.md", false);
      this.Add(IgnoreExceptionFiles, "README.md", true);
    }
  }
}
