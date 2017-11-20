using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GenericAccess.Data.Common;

namespace GenericAccess.Mappings.Base
{
    public static class MappingsList
    {
        private static readonly List<Mapping> Mappings;

        static MappingsList()
        {
            Mappings = new List<Mapping>();
        }

        public static bool Exist<TFrom, TTo>()
        {
            return Mappings.Any(m => m.Is<TFrom, TTo>());
        }

        public static bool Exist(Type from, Type to)
        {
            return Mappings.Any(m => m.Is(from, to));
        }

        public static IQueryable<TTo> Apply<TFrom, TTo>(IQueryable<TFrom> query)
        {
            var mapping = (IMapping<TFrom, TTo>)Mappings.FirstOrDefault(m => m.Is<TFrom, TTo>());
            return mapping.Apply(query);
        }

        public static void Add<TFrom, TTo>(Expression<Func<TFrom, TTo>> expression)
        {
            var mapping = new ExpressionMapping<TFrom, TTo>(expression);
            Mappings.Add(mapping);
        }

        public static void Add<TFrom, TTo>(Expression<Func<IQueryable<TFrom>, IContext, IQueryable<TTo>>> tranform)
        {
            Add(ContextType.Test, tranform);
        }

        public static void Add<TFrom, TTo>(ContextType contextType, Expression<Func<IQueryable<TFrom>, IContext, IQueryable<TTo>>> tranform)
        {
            var mapping = new QueryMapping<TFrom, TTo>(tranform, contextType);
            Mappings.Add(mapping);
        }
    }
}