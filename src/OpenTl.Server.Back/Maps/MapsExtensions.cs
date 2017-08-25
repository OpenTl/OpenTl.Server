namespace OpenTl.Server.Back.Maps
{
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

    using OpenTl.Schema;

    public static class MapsExtensions
    {
        public static IMappingExpression<TSource, TDest> IgnoreAllUnmapped<TSource, TDest>(this IMappingExpression<TSource, TDest> expression)
        {
            expression.ForAllMembers(opt => opt.Ignore());
            return expression;
        }

        public static void MapEnumerableToVector<TSource, TDestination>(this IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<IEnumerable<TSource>, TVector<TDestination>>()
               .ConstructUsing(items => new TVector<TDestination>(items.Select(Mapper.Map<TSource, TDestination>).ToArray()))
               .ForMember(vector => vector.Items, expression => expression.Ignore());
        }
    }
}