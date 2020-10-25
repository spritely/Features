namespace Features
{
    /// <summary>
    /// This is a marker interface for any types that provide data that should be available to all features.
    /// All types implementing this can be discovered and added to a DI container and the entire set automatically
    /// added to the FeatureEvaluator via IEnumerable{ISharedFeatureData} parameter.
    /// </summary>
    public interface ISharedFeatureData
    {
    }
}
