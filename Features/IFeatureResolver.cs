namespace Features
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// A concrete IFeatureResolver should be implemented by a feature toggle solution isolating the rest of the project from direct dependencies.
    /// </summary>
    public interface IFeatureResolver
    {
        /// <summary>
        /// Checks if the provided feature is enabled or not.
        /// </summary>
        /// <param name="name">The name of the feature to check.</param>
        /// <param name="data">Any additional data to use in potentially determining feature status.</param>
        /// <returns>The current value (on or off) of the feature.</returns>
        Task<bool> IsOnAsync(string name, IDictionary<string, object> data);
    }
}
