using System;
using System.Linq.Expressions;

namespace Isidore.Maths
{
    /// <summary>
    /// Uses lambda and parameter expressions to provide function delegates
    /// that are formed at runtime.  
    /// </summary>
    public class ExpressionFunc
    {
        /// <summary>
        /// Creates an operator delegate compiled at runtime for an unary
        /// expression.
        /// </summary>
        /// <typeparam name="TArg1"> Input data type </typeparam>
        /// <typeparam name="TResult"> Output data type </typeparam>
        /// <param name="expression"> Unary expression delegate </param>
        /// <returns> Runtime compiled delegate </returns>
        public static Func<TArg1, TResult> Create<TArg1, TResult>(
               Func<Expression, UnaryExpression> expression)
        {
            // Identifies the "value" parameter the expression tree
            ParameterExpression value = Expression.Parameter(typeof(TArg1),
                "value");
            // Creates the compile time lambda expression
            try
            {
                return Expression.Lambda<Func<TArg1, TResult>>
                    (expression(value), value).Compile();
            }
            catch (Exception ex)
            {
                string msg = ex.Message; // avoid capture of ex itself
                return delegate { throw new 
                    InvalidOperationException(msg); };
            }
        }

        /// <summary>
        /// Creates an operator delegate compiled at runtime for an 
        /// binary expression.
        /// </summary>
        /// <typeparam name="TArg1"> First input data type </typeparam>
        /// <typeparam name="TArg2"> Second input data type </typeparam>
        /// <typeparam name="TResult"> Output data type </typeparam>
        /// <param name="expression"> Binary expression delegate </param>
        /// <returns> Runtime compiled delegate </returns>
        public static Func<TArg1, TArg2, TResult> Create<TArg1, TArg2, 
            TResult>( Func<Expression, Expression, BinaryExpression>
            expression)
        {
            // Identifies the "value" parameters the expression trees
            ParameterExpression value0 = 
                Expression.Parameter(typeof(TArg1), "value0");
            ParameterExpression value1 = 
                Expression.Parameter(typeof(TArg2), "value1");
            // Creates the compile time lambda expression
            try
            {
                try
                {
                    // Matching types
                    return Expression.Lambda<Func<TArg1, TArg2, TResult>>
                        (expression(value0, value1), value0, 
                        value1).Compile();
                }
                catch(InvalidOperationException)
                {
                    // If the input types don't match the output, 
                    // adds converts to the output type
                    Expression castVal0 = typeof(TArg1) == 
                        typeof(TResult) ? 
                        value0 : (Expression)Expression.Convert(value0, 
                        typeof(TResult));
                    Expression castVal1 = 
                        typeof(TArg2) == typeof(TResult) ?  
                        value1 : (Expression)Expression.Convert(value1, 
                        typeof(TResult));
                    return Expression.Lambda<Func<TArg1, TArg2, TResult>>
                        (expression(castVal0, castVal1), value0, value1).
                        Compile();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return delegate {
                    throw new InvalidOperationException(msg); };
            }
        }
    }
}
