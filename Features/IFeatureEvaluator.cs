namespace Features
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// A feature evaluator that's capable of adding additional features to make writing features easy.
    /// </summary>
    public interface IFeatureEvaluator
    {
        /// <summary>
        /// Determines if the feature is currently on or off.
        /// </summary>
        /// <param name="name">The name of the feature.</param>
        /// <param name="defaultValue">Default value if feature is indeterminate.</param>
        /// <returns>The current value of the feature (on or off).</returns>
        Task<bool> IsOnAsync(string name, bool defaultValue = false);

        /// <summary>
        /// Checks if the value of a feature matches the value of the supplied argument.
        /// Throws an InvalidOperationException if the underlying provider does not support string based features.
        /// </summary>
        /// <param name="name">The name of the feature.</param>
        /// <param name="value">The value to match against.</param>
        /// <param name="comparison">The type of string comparison to use when matching. Defaults to OrdinalIgnoreCase.</param>
        /// <param name="defaultValue">Default value to use if feature is indeterminate. Defaults to false.</param>
        /// <returns>The current value (on or off) of the feature.</returns>
        Task<bool> MatchesAsync(string name, string value, StringComparison comparison = StringComparison.OrdinalIgnoreCase, bool defaultValue = false);

        /// <summary>
        /// Adds data to the feature that can be used to help determine the outcome of the toggle.
        /// </summary>
        /// <typeparam name="T">The type of data being added.</typeparam>
        /// <param name="data">The data to add.</param>
        /// <returns>A copy of the feature evaluator so calls can be chained.</returns>
        IFeatureEvaluator With<T>(T data) where T : class;

        /// <summary>
        /// Adds data to the feature that can be used to help determine the outcome of the toggle.
        /// </summary>
        /// <param name="data">The data to add.</param>
        /// <returns>A copy of the feature evaluator so calls can be chained.</returns>
        IFeatureEvaluator With(params object[] data);
    }
}
