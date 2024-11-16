namespace DotNet.Testcontainers.Commons;

[PublicAPI]
public static class CommonImages
{
    public static readonly IImage Ryuk = new DockerImage("testcontainers/ryuk:0.9.0");

    public static readonly IImage Alpine = new DockerImage("alpine:3.17");

    public static readonly IImage Socat = new DockerImage("alpine/socat:1.8.0.0");

    public static readonly IImage Curl = new DockerImage("curlimages/curl:8.00.1");

    public static readonly IImage Nginx = new DockerImage("nginx:1.22");

    public static readonly IImage ServerCore = new DockerImage("mcr.microsoft.com/windows/servercore:ltsc2022");
}