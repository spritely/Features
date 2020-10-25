namespace Features
{
    /// <summary>
    /// Supplies the Machine Name to all feature toggles when added to the FeatureEvaluator
    /// </summary>
    public class MachineNameFeatureData : ISharedFeatureData
    {
        // The machine name
        public string MachineName => System.Environment.MachineName;
    }
}
