namespace DotNet.Testcontainers.Tests.Fixtures
{
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Images;
  using Xunit;

  public class DockerIgnoreFileFixture : TheoryData<IgnoreFile, string, bool>
  {
    public DockerIgnoreFileFixture()
    {
      var logger = TestcontainersSettings.Logger;

      var ignoreFileDoesNotExist = new DockerIgnoreFile("/mypath/", ".dockerignore", logger, new FileSystemFixture()
      {
        Exists = false,
      });

      var ignoreDirectory = new DockerIgnoreFile("/mypath/", ".dockerignore", logger, new FileSystemFixture()
      {
        Exists = true,
        DirectoryFullName = "/mypath",
        Lines =
        {
          "bin/",
        },
      });

      var ignoreDirectoryDockerIgnoreAndDockerFile = new DockerIgnoreFile("/mypath/", ".dockerignore", logger, new FileSystemFixture()
      {
        Lines =
        {
          "bin/",
          ".dockerignore",
          "Dockerfile",
        },
        Exists = true,
        DirectoryFullName = "/mypath",
      });

      var dockerIgnoreFile = ".dockerignore";
      var dockerfile = "Dockerfile";
      var nestedDockerfile = "/lipsum/lorem/Dockerfile";
      var binDirectory = "bin";

      this.Add(ignoreFileDoesNotExist, dockerIgnoreFile, true);
      this.Add(ignoreFileDoesNotExist, dockerfile, true);
      this.Add(ignoreFileDoesNotExist, nestedDockerfile, true);
      this.Add(ignoreFileDoesNotExist, binDirectory, true);

      this.Add(ignoreDirectory, dockerIgnoreFile, true);
      this.Add(ignoreDirectory, dockerfile, true);
      this.Add(ignoreDirectory, nestedDockerfile, true);
      this.Add(ignoreDirectory, binDirectory, false);

      this.Add(ignoreDirectoryDockerIgnoreAndDockerFile, dockerIgnoreFile, true);
      this.Add(ignoreDirectoryDockerIgnoreAndDockerFile, dockerfile, true);
      this.Add(ignoreDirectoryDockerIgnoreAndDockerFile, nestedDockerfile, true);
      this.Add(ignoreDirectory, binDirectory, false);
    }
  }
}
