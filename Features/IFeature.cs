namespace Spritely.Features
{
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a feature that can be turned on or off.
    /// </summary>
    public interface IFeature
    {
        /// <summary>
        /// The name of the feature.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Determines if the feature is currently on or off.
        /// </summary>
        /// <returns>Teh current value of the feature (on or off).</returns>
        Task<bool> IsOnAsync();
    }
}
