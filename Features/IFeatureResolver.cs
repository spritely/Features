namespace Features
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// A concrete IFeatureResolver should be implemented by a feature toggle solution isolating the rest of the project from direct dependencies.
    /// </summary>
    public interface IFeatureResolver
    {
        /// <summary>
        /// Checks if the feature is enabled or not.
        /// </summary>
        /// <param name="name">The name of the feature to check.</param>
        /// <param name="data">Any additional data to use in potentially determining feature status.</param>
        /// <param name="defaultValue">Default value to use if feature is indeterminate. Defaults to false.</param>
        /// <returns>The current value (on or off) of the feature.</returns>
        Task<bool> IsOnAsync(string name, IDictionary<string, object> data, bool defaultValue = false);

        /// <summary>
        /// Checks if the value of a feature matches the value of the supplied argument.
        /// This should throw an InvalidOperationException if the underlying provider does not support string based features.
        /// </summary>
        /// <param name="name">The name of the feature.</param>
        /// <param name="data">Any additional data to use in potentially determining feature status.</param>
        /// <param name="value">The value to match against.</param>
        /// <param name="comparison">The type of string comparison to use when matching. Defaults to Ordinal.</param>
        /// <param name="defaultValue">Default value to use if feature is indeterminate. Defaults to false.</param>
        /// <returns>The current value (on or off) of the feature.</returns>
        Task<bool> MatchesAsync(string name, IDictionary<string, object> data, string value, StringComparison comparison = StringComparison.Ordinal, bool defaultValue = false);
    }
}
