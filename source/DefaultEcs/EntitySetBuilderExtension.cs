﻿namespace DefaultEcs
{
    /// <summary>
    /// Provides set of static methods to create more easily rules on a <see cref="EntitySetBuilder"/> instance.
    /// </summary>
    public static class EntitySetBuilderExtension
    {
        /// <summary>
        /// Makes a rule to obsverve <see cref="Entity"/> with at least one component of type <typeparamref name="T1"/> or <typeparamref name="T2"/>.
        /// </summary>
        /// <typeparam name="T1">The first type of component.</typeparam>
        /// <typeparam name="T2">The second type of component.</typeparam>
        /// <param name="builder">The <see cref="EntitySetBuilder"/> on which to create the rule.</param>
        /// <returns>The given <see cref="EntitySetBuilder"/>.</returns>
        public static EntitySetBuilder WithAny<T1, T2>(this EntitySetBuilder builder) => builder.WithAny(typeof(T1), typeof(T2));

        /// <summary>
        /// Makes a rule to obsverve <see cref="Entity"/> with at least one component of type <typeparamref name="T1"/>, <typeparamref name="T2"/> or <typeparamref name="T3"/>.
        /// </summary>
        /// <typeparam name="T1">The first type of component.</typeparam>
        /// <typeparam name="T2">The second type of component.</typeparam>
        /// <typeparam name="T3">The third type of component.</typeparam>
        /// <param name="builder">The <see cref="EntitySetBuilder"/> on which to create the rule.</param>
        /// <returns>The given <see cref="EntitySetBuilder"/>.</returns>
        public static EntitySetBuilder WithAny<T1, T2, T3>(this EntitySetBuilder builder) => builder.WithAny(typeof(T1), typeof(T2), typeof(T3));

        /// <summary>
        /// Makes a rule to obsverve <see cref="Entity"/> with at least one component of type <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/> or <typeparamref name="T4"/>.
        /// </summary>
        /// <typeparam name="T1">The first type of component.</typeparam>
        /// <typeparam name="T2">The second type of component.</typeparam>
        /// <typeparam name="T3">The third type of component.</typeparam>
        /// <typeparam name="T4">The fourth type of component.</typeparam>
        /// <param name="builder">The <see cref="EntitySetBuilder"/> on which to create the rule.</param>
        /// <returns>The given <see cref="EntitySetBuilder"/>.</returns>
        public static EntitySetBuilder WithAny<T1, T2, T3, T4>(this EntitySetBuilder builder) => builder.WithAny(typeof(T1), typeof(T2), typeof(T3), typeof(T4));

        /// <summary>
        /// Makes a rule to obsverve <see cref="Entity"/> with at least one component of type <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/> or <typeparamref name="T5"/>.
        /// </summary>
        /// <typeparam name="T1">The first type of component.</typeparam>
        /// <typeparam name="T2">The second type of component.</typeparam>
        /// <typeparam name="T3">The third type of component.</typeparam>
        /// <typeparam name="T4">The fourth type of component.</typeparam>
        /// <typeparam name="T5">The fith type of component.</typeparam>
        /// <param name="builder">The <see cref="EntitySetBuilder"/> on which to create the rule.</param>
        /// <returns>The given <see cref="EntitySetBuilder"/>.</returns>
        public static EntitySetBuilder WithAny<T1, T2, T3, T4, T5>(this EntitySetBuilder builder) => builder.WithAny(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
    }
}