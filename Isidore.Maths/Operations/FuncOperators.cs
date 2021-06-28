using System;
using System.Linq.Expressions;

// Based on work done by Marc Gravell

namespace Isidore.Maths
{
    /// <summary>
    /// Defines binary expression operations as functions
    /// </summary>
    /// <typeparam name="T"> Data types </typeparam>
    public static class Operator<T>
    {

        # region Fields & Properties

        static readonly Func<T, T, T> add, subtract, multiply, divide;
        static readonly Func<T, T, T> and, or, xor;
        static readonly Func<T, T> negate, abs, not;
        static readonly Func<T, T, bool> equal, notEqual, greaterThan, 
            lessThan, greaterThanOrEqual, lessThanOrEqual;

        /// <summary>
        /// Addition function delegate
        /// </summary>
        public static Func<T, T, T> Add { get { return add; } }

        /// <summary>
        /// Subtraction function delegate
        /// </summary>
        public static Func<T, T, T> Subtract { get { return subtract; } }

        /// <summary>
        /// Multiplication function delegate
        /// </summary>
        public static Func<T, T, T> Multiply { get { return multiply; } }

        /// <summary>
        /// Division function delegate
        /// </summary>
        public static Func<T, T, T> Divide { get { return divide; } }

        /// <summary>
        /// Negation function delegate
        /// </summary>
        public static Func<T, T> Negate { get { return negate; } }

        /// <summary>
        /// Absolute value function delegate
        /// </summary>
        public static Func<T, T> Absolute { get { return abs; } }

        /// <summary>
        /// Bitwise complement function delegate
        /// </summary>
        public static Func<T, T> Not { get { return not; } }

        /// <summary>
        /// Bitwise AND function delegate
        /// </summary>
        public static Func<T, T, T> And { get { return and; } }

        /// <summary>
        /// Bitwise OR function delegate
        /// </summary>
        public static Func<T, T, T> Or { get { return or; } }

        /// <summary>
        /// Bitwise XOR function delegate
        /// </summary>
        public static Func<T, T, T> Xor { get { return xor; } }

        /// <summary>
        /// Equality comparison function delegate
        /// </summary>
        public static Func<T, T, bool> Equal { get { return equal; } }

        /// <summary>
        /// Inequality comparison function delegate
        /// </summary>
        public static Func<T, T, bool> NotEqual
        { get { return notEqual; } }

        /// <summary>
        /// "Greater than" numerical comparison function delegate
        /// </summary>
        public static Func<T, T, bool> GreaterThan
        { get { return greaterThan; } }

        /// <summary>
        /// "Greater than or equal to" numerical comparison function 
        /// delegate
        /// </summary>
        public static Func<T, T, bool> GreaterThanOrEqual
        { get { return greaterThanOrEqual; } }

        /// <summary>
        /// "Less than" numerical comparison function delegate
        /// </summary>
        public static Func<T, T, bool> LessThan
        { get { return lessThan; } }

        /// <summary>
        /// "Less than or equal to" numerical comparison function delegate
        /// </summary>
        public static Func<T, T, bool> LessThanOrEqual
        { get { return lessThanOrEqual; } }

        # endregion Fields & Properties
        # region Constructors

        static Operator()
        {
            // Arithmetic operators
            add = ExpressionFunc.Create<T, T, T>(Expression.Add);
            subtract = ExpressionFunc.Create<T, T, T>(Expression.Subtract);
            multiply = ExpressionFunc.Create<T, T, T>(Expression.Multiply);
            divide = ExpressionFunc.Create<T, T, T>(Expression.Divide);
            negate = ExpressionFunc.Create<T, T>(Expression.Negate);

            // Logic operators
            not = ExpressionFunc.Create<T, T>(Expression.Not);
            and = ExpressionFunc.Create<T, T, T>(Expression.And);
            or = ExpressionFunc.Create<T, T, T>(Expression.Or);
            xor = ExpressionFunc.Create<T, T, T>(Expression.ExclusiveOr);

            // Comparison operators
            equal = ExpressionFunc.Create<T, T, bool>(Expression.Equal);
            notEqual = ExpressionFunc.Create<T, T, bool>(
                Expression.NotEqual);
            greaterThan = ExpressionFunc.Create<T, T, bool>(
                Expression.GreaterThan);
            greaterThanOrEqual = ExpressionFunc.Create<T, T, bool>(
                Expression.GreaterThanOrEqual);
            lessThan = ExpressionFunc.Create<T, T, bool>(
                Expression.LessThan);
            lessThanOrEqual = ExpressionFunc.Create<T, T, bool>(
                Expression.LessThanOrEqual);

            // Expression trees
            Expression<Func<T, T>> eabs = 
                x => greaterThanOrEqual(x, default(T)) ? x : negate(x);
            abs = eabs.Compile();
        }

        # endregion Constructors
    }

    /// <summary>
    /// Defines unary expression operations as functions
    /// </summary>
    /// <typeparam name="TinOnly"> Input data type </typeparam>
    /// <typeparam name="Tout"> Output data type </typeparam>
    public static class Operator<TinOnly, Tout>
    {

        # region Fields & Properties

        private static readonly Func<TinOnly, Tout> convert;
        private static readonly Func<Tout, TinOnly, Tout> add, subtract, 
            multiply, divide;

        /// <summary>
        /// Conversion operation
        /// </summary>
        public static Func<TinOnly, Tout> Convert
        { get { return convert; } }

        /// <summary>
        /// Addition function delegate
        /// </summary>
        public static Func<Tout, TinOnly, Tout> Add
        { get { return add; } }

        /// <summary>
        /// Subtraction function delegate
        /// </summary>
        public static Func<Tout, TinOnly, Tout> Subtract
        { get { return subtract; } }

        /// <summary>
        /// Multiplication function delegate
        /// </summary>
        public static Func<Tout, TinOnly, Tout> Multiply
        { get { return multiply; } }

        /// <summary>
        /// Division function delegate
        /// </summary>
        public static Func<Tout, TinOnly, Tout> Divide
        { get { return divide; } }

        #endregion Fields & Properties
        #region Constructor

        static Operator()
        {
            convert = ExpressionFunc.Create<TinOnly, Tout>(
                body => Expression.Convert(body, typeof(Tout)));
            add = ExpressionFunc.Create<Tout, TinOnly, Tout>(
                Expression.Add);
            subtract = ExpressionFunc.Create<Tout, TinOnly, Tout>(
                Expression.Subtract);
            multiply = ExpressionFunc.Create<Tout, TinOnly, Tout>(
                Expression.Multiply);
            divide = ExpressionFunc.Create<Tout, TinOnly, Tout>(
                Expression.Divide);
        }

        # endregion Constructor
    }
}
