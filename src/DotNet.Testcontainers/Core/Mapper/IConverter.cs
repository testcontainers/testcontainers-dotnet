namespace DotNet.Testcontainers.Core.Mapper
{
  internal interface IConverter<in TSource, out TTarget>
  {
    TTarget Convert(TSource source);
  }
}
