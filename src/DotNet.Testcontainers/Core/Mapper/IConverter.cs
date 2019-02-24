namespace DotNet.Testcontainers.Core.Mapper
{
  internal interface IConverter<TSource, TTarget>
  {
    TTarget Convert(TSource source);
  }
}
