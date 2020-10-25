namespace Features
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>
    /// Default implementation of IFeatureEvaluator that will accumulate data to be supplied to all features
    /// and makes writing an IFeature easy including adding additional feature specific data.
    /// </summary>
    public class FeatureEvaluator : IFeatureEvaluator
    {
        private readonly ConcurrentDictionary<string, object> _allData;
        private IFeatureResolver _resolver;

        /// <summary>
        /// Creates a new instance of FeatureEvaluator.
        /// </summary>
        /// <param name="resolver">The underlying feature resolver.</param>
        /// <param name="sharedData">Any data that should be added to all features.</param>
        public FeatureEvaluator(IFeatureResolver resolver, IEnumerable<ISharedFeatureData> sharedData = null)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            var sharedFeatureData = sharedData ?? Enumerable.Empty<ISharedFeatureData>();

            _allData = new ConcurrentDictionary<string, object>();

            foreach (var data in sharedFeatureData)
            {
                PopulateAllDataFrom(data);
            }
        }

        /// <summary>
        /// Determines if the feature is currently on or off.
        /// </summary>
        /// <param name="name">The name of the feature.</param>
        /// <returns>The current value of the feature (on or off).</returns>
        public Task<bool> IsOnAsync(string name) => _resolver.IsOnAsync(name, _allData);

        /// <summary>
        /// Adds data to the feature that can be used to help determine the outcome of the toggle.
        /// </summary>
        /// <typeparam name="T">The type of data being added.</typeparam>
        /// <param name="data">The data to add.</param>
        /// <returns>A copy of the feature evaluator so calls can be chained.</returns>
        public IFeatureEvaluator With<T>(T data) where T : class
        {
            PopulateAllDataFrom(data);
            return this;
        }

        /// <summary>
        /// Adds data to the feature that can be used to help determine the outcome of the toggle.
        /// </summary>
        /// <param name="data">The data to add.</param>
        /// <returns>A copy of the feature evaluator so calls can be chained.</returns>
        public IFeatureEvaluator With(params object[] data)
        {
            foreach (var d in data)
            {
                PopulateAllDataFrom(d);
            }
            
            return this;
        }

        private void PopulateAllDataFrom(object data)
        {
            var properties = data.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var value = property.GetValue(data);
                _allData.AddOrUpdate(property.Name, value, (key, oldValue) => value);
            }
        }
    }
}
