namespace DotNet.Testcontainers.Core.Mapper
{
  internal interface IConverterFactory
  {
    IConverter<TSource, TTarget> Get<TSource, TTarget>(string name = null);
  }
}
