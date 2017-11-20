using System;
using System.Linq;
using System.Linq.Expressions;
using GenericAccess.Data.Common;

namespace GenericAccess.Mappings.Base
{
    public enum MappingType: byte
    {
        Expression,
        Query
    }

    public interface IMapping<in TFrom, out TTo>
    {
        IQueryable<TTo> Apply(IQueryable<TFrom> query);
    }

    public abstract class Mapping
    {
        public Type From { get; protected set; }

        public Type To { get; protected set; }

        public MappingType Type { get; set; }

        protected Mapping(Type from, Type to, MappingType type)
        {
            From = from;
            To = to;
            Type = type;
        }

        public bool Is<TFirst, TSecond>()
        {
            return Is(typeof(TFirst), typeof(TSecond));
        }

        public bool Is(Type from, Type to)
        {
            return from == From
                && to == To;
        }
    }

    public class ExpressionMapping<TFrom, TTo> : Mapping, IMapping<TFrom, TTo>
    {
        public ExpressionMapping(Expression<Func<TFrom, TTo>> mapping)
            :base(typeof(TFrom), typeof(TTo), MappingType.Expression)
        {
            _mapping = mapping;
        }

        private readonly Expression<Func<TFrom, TTo>> _mapping;

        public IQueryable<TTo> Apply(IQueryable<TFrom> query)
        {
            return query.Select(_mapping);
        }
    }

    public class QueryMapping<TFrom, TTo> : Mapping, IMapping<TFrom, TTo>
    {
        private readonly ContextType _contextType;

        public QueryMapping(Expression<Func<IQueryable<TFrom>, IContext, IQueryable<TTo>>> mapping, ContextType contextType = ContextType.Test)
            : base(typeof(TFrom), typeof(TTo), MappingType.Query)
        {
            _mapping = mapping;
            _contextType = contextType;
        }

        private readonly Expression<Func<IQueryable<TFrom>, IContext, IQueryable<TTo>>> _mapping;

        public IQueryable<TTo> Apply(IQueryable<TFrom> query)
        {
            var context = ServiceLocator.GetByKey<IContext>(_contextType);
            return _mapping.Compile().Invoke(query, context);
        }
    }
}