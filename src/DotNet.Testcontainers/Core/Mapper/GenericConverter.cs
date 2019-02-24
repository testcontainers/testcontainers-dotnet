namespace DotNet.Testcontainers.Core.Mapper
{
  internal class GenericConverter : IGenericConverter
  {
    private readonly ConverterFactory factory;

    public GenericConverter(ConverterFactory factory)
    {
      this.factory = factory;
    }

    public TTarget Convert<TSource, TTarget>(TSource source, string name = null)
    {
      return this.factory.Get<TSource, TTarget>(name).Convert(source);
    }
  }
}
