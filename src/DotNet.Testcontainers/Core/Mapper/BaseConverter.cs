namespace DotNet.Testcontainers.Core.Mapper
{
  internal abstract class BaseConverter<TSource, TTarget>
  {
    private readonly string name;

    protected BaseConverter(string name)
    {
      this.name = name;
    }

    public abstract TTarget Convert(TSource source);
  }
}
