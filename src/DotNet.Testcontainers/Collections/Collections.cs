namespace DotNet.Testcontainers.Collections
{
  using System.Collections.Generic;

  public static class Collections
  {
    public static IList<T> ListOf<T>(params T[] items)
    {
      var listOf = new List<T>();

      if (items != null)
      {
        foreach (var value in items)
        {
          listOf.Add(value);
        }
      }

      return listOf;
    }
  }
}
