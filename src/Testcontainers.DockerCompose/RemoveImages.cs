namespace Testcontainers.DockerCompose;

[PublicAPI]
public enum RemoveImages
{
    None = 0,
    
    /// <summary>
    /// Remove all images.
    /// </summary>
    All,
    
    /// <summary>
    /// Remove only images that don't have a custom tag set by the `image` field.
    /// </summary>
    Local,
}