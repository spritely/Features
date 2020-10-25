namespace Spritely.Features
{
    /// <summary>
    /// Supplies the Machine Name to all feature toggles when added to the FeatureEvaluator
    /// </summary>
    public class MachineNameFeatureData : ISharedFeatureData
    {
        /// <summary>
        /// The name of this machine from the environment.
        /// </summary>
        public string MachineName => System.Environment.MachineName;
    }
}
