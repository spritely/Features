namespace Spritely.Features
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// A default feature selector (factory) that can create new instances based on the value of a feature toggle.
    /// </summary>
    /// <typeparam name="TFeature">The feature to check for on/off state.</typeparam>
    /// <typeparam name="TOn">The type to create when the feature is on.</typeparam>
    /// <typeparam name="TOff">The type to create when the feature is off.</typeparam>
    /// <typeparam name="TCommon">The common type that both the on and the off implementations share.</typeparam>
    public class FeatureSelector<TFeature, TOn, TOff, TCommon> : IFeatureSelector<TCommon>
        where TFeature : class, IFeature
        where TOn : TCommon
        where TOff : TCommon
    {
        private readonly ILogger<FeatureSelector<TFeature, TOn, TOff, TCommon>> _logger;
        private readonly TFeature _feature;
        private readonly Func<TOn> _on;
        private readonly Func<TOff> _off;

        /// <summary>
        /// Constructs a new instance of a FeatureSelector.
        /// </summary>
        /// <param name="feature">The feature to check for on/off state.</param>
        /// <param name="on">The function to call when the feature is on to create an instance.</param>
        /// <param name="off">The function to call when the feature is off to create an instance.</param>
        /// <param name="logger">The logger for recording which feature was selected.</param>
        public FeatureSelector(TFeature feature, Func<TOn> @on, Func<TOff> off, ILogger<FeatureSelector<TFeature, TOn, TOff, TCommon>> logger)
        {
            _feature = feature ?? throw new ArgumentNullException(nameof(feature));
            _on = @on ?? throw new ArgumentNullException(nameof(@on));
            _off = off ?? throw new ArgumentNullException(nameof(off));
            _logger = logger;
;       }

        /// <summary>
        /// Creates a new instance of a type based on the value of a feature toggle.
        /// </summary>
        /// <returns>A new instance.</returns>
        public async Task<TCommon> CreateAsync()
        {
            var isOn = await _feature.IsOnAsync();
            var onText = isOn ? "on" : "off";
            var featureType = isOn ? typeof(TOn).Name : typeof(TOff).Name;
            _logger?.LogInformation($"Creating { featureType } because feature '{ _feature.Name }' is toggled '{onText}'.");

            return isOn ? (TCommon)_on() : (TCommon)_off();
        }
    }
}
