namespace DotNet.Testcontainers.Core.Mapper
{
  internal abstract class BaseConverter<TSource, TTarget>
  {
    protected BaseConverter(string name)
    {
      this.Name = name;
    }

    public string Name { get; }

    public abstract TTarget Convert(TSource source);
  }
}
