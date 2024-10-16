namespace Testcontainers.DockerCompose
{
    /// <summary>
    /// Docker Compose mode.
    /// </summary>
    public enum DockerComposeMode
    {
        /// <summary>
        /// The local Docker Compose mode utilizes the Docker Compose CLI to execute the configuration.
        /// </summary>
        Local,

        /// <summary>
        /// The remote Docker Compose mode utilizes a sidecar container to execute the configuration.
        /// </summary>
        Remote,
    }
}