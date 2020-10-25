namespace Features
{
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
        /// <returns>The current value of the feature (on or off).</returns>
        Task<bool> IsOnAsync(string name);

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
