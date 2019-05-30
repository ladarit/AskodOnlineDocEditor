using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AskodOnline.Editor.Helpers
{
    public static class LambdaExpressionsHelper<T> where T: class 
    {
        private static readonly log4net.ILog Log = Log4Net.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static Expression<Func<T, bool>> CreateLambdaExpression(string propertyName, object propertyValue)
        {
            try
            {
                var rowIdConstant = Expression.Constant(propertyValue);
                var param = Expression.Parameter(typeof(T));
                var property = Expression.PropertyOrField(Expression.Convert(param, typeof(T)), propertyName);
                var body = Expression.Equal(property, rowIdConstant);
                return Expression.Lambda<Func<T, bool>>(body, param);

            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }
    }
}