using Mapster;

namespace APIEcommerce.Helpers;

/// <summary>
/// Adaptador que proporciona una interfaz compatible con AutoMapper para usar con Mapster
/// </summary>
public interface IMapper
{
  TDestination Map<TDestination>(object source);
  TDestination Map<TSource, TDestination>(TSource source);
}

/// <summary>
/// Implementación de IMapper que usa Mapster internamente
/// </summary>
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
