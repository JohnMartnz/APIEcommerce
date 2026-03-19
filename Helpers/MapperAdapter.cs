using Mapster;

namespace APIEcommerce.Helpers;

public interface IMapper
{
  TDestination Map<TDestination>(object source);
  TDestination Map<TSource, TDestination>(TSource source);
}

public class MapsterAdapter : IMapper
{
  public TDestination Map<TDestination>(object source)
  {
    if (source == null)
      throw new ArgumentNullException(nameof(source));

    return source.Adapt<TDestination>();
  }

  public TDestination Map<TSource, TDestination>(TSource source)
  {
    if (source == null)
      throw new ArgumentNullException(nameof(source));

    return source.Adapt<TDestination>();
  }
}
