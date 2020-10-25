namespace Spritely.Features
{
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a feature selector or factory that can create new instances based on the value of a feature toggle.
    /// </summary>
    /// <typeparam name="TCommon">The common type that both the on and the off implementations share.</typeparam>
    public interface IFeatureSelector<TCommon>
    {
        /// <summary>
        /// Creates a new instance of a type based on the value of a feature toggle.
        /// </summary>
        /// <returns>A new instance.</returns>
        Task<TCommon> CreateAsync();
    }
}
