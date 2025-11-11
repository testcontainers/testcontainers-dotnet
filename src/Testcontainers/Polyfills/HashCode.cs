#if NETFRAMEWORK
namespace DotNet.Testcontainers.Polyfills
{
  public struct HashCode
  {
    public static int Combine<T1, T2>(T1 value1, T2 value2)
    {
      var h1 = value1.GetHashCode();
      var h2 = value2.GetHashCode();
      return ((h1 << 5) + h1) ^ h2;
    }
  }
}
#endif
