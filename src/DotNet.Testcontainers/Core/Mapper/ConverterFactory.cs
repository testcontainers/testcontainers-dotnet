namespace DotNet.Testcontainers.Core.Mapper
{
  using System;
  using System.Collections.Generic;
  using static LanguageExt.Prelude;

  internal class ConverterFactory : IConverterFactory
  {
    private readonly Dictionary<Tuple<string, Type>, Func<object>> converters = new Dictionary<Tuple<string, Type>, Func<object>>();

    public void Register<TSource, TTarget>(Func<object> constructor, string name = null)
    {
      var key = Key<TSource>(name);
      this.converters.Add(new Tuple<string, Type>(key, typeof(TTarget)), constructor);
    }

    public IConverter<TSource, TTarget> Get<TSource, TTarget>(string name = null)
    {
      var key = Key<TSource>(name);
      var constructor = this.converters[new Tuple<string, Type>(key, typeof(TTarget))];
      return (IConverter<TSource, TTarget>)constructor();
    }

    private static string Key<TSource>(string name)
    {
      return Optional(name).Match(
        Some: value => $"{value}/{typeof(TSource)}",
        None: () => $"{typeof(TSource)}");
    }
  }
}
