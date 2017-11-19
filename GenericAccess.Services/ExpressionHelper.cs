using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GenericAccess
{
    public static class ExpressionHelper
    {

        public static Expression<Func<TEntity, int>> GetPrimaryKeySelector<TEntity>()
        {
            var property = typeof(TEntity).GetProperty("PrimaryKeySelector", BindingFlags.Static | BindingFlags.Public);
            return property.GetValue(null) as Expression<Func<TEntity, int>>;
        }

        public static Expression<Func<TEntity, bool>> Compare<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> selector, TProperty value)
        {
            var entity = selector.Parameters.First();
            var expression = Expression.Equal(selector.Body, Expression.Constant(value));
            return Expression.Lambda<Func<TEntity, bool>>(expression, entity);
        }

        public static Expression<Func<TEntity, bool>> FilterExpressionByIds<TEntity>(IEnumerable<int> ids)
        {
            var propertySelector = GetPrimaryKeySelector<TEntity>();
            var parameterExp = propertySelector.Parameters.First();
            var propertyExp = propertySelector.Body;
            MethodInfo method = typeof(Enumerable).
                GetMethods().
                Where(x => x.Name == nameof(Enumerable.Contains)).
                Single(x => x.GetParameters().Length == 2).
                MakeGenericMethod(typeof(int));
            var someValue = Expression.Constant(ids, typeof(IEnumerable<int>));
            var containsMethodExp = Expression.Call(method, someValue, propertyExp);

            return Expression.Lambda<Func<TEntity, bool>>(containsMethodExp, parameterExp);
        }

        public static Expression<Func<TEntity, bool>> PkFilterExpression<TEntity>(int id)
        {
            var pkSelector = GetPrimaryKeySelector<TEntity>();
            return Compare(pkSelector, id);
        }

        public static Expression<Func<T, bool>> GetInvertedBoolExpression<T>(string propertyName)
        {
            var instance = Expression.Parameter(typeof(T), "x");

            return Expression.Lambda<Func<T, bool>>(Expression.Not(Expression.Property(instance, propertyName)), instance);
        }
        

       
       
    }
}
