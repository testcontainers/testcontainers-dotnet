namespace DotNet.Testcontainers.Containers.Configurations
{
  using System;
  using System.Net.Security;
  using System.Security.Cryptography.X509Certificates;

  internal static class RemoteCertificateValidation
  {
    internal static RemoteCertificateValidationCallback IgnoreAllCallback()
    {
      return (_, _, _, _) => true;
    }

    internal static RemoteCertificateValidationCallback CustomCaValidationCallback(X509Certificate2 caCertificate)
    {
      var caCertHash = caCertificate.GetCertHashString();
      if (string.IsNullOrEmpty(caCertHash))
      {
        throw new InvalidOperationException("CA cert hash is empty - verification not possible");
      }

      return (_, _, chain, errors) =>
      {
        if (errors != SslPolicyErrors.None)
        {
          return false;
        }

        foreach (var element in chain.ChainElements)
        {
          if (caCertHash.Equals(element.Certificate.GetCertHashString()))
          {
            return true;
          }
        }

        return false;
      };
    }
  }
}
