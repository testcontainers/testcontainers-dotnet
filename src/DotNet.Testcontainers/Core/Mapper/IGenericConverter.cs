namespace DotNet.Testcontainers.Core.Mapper
{
  internal interface IGenericConverter
  {
    TTarget Convert<TSource, TTarget>(TSource source, string name = null);
  }
}
