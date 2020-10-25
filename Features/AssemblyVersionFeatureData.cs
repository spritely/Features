namespace Spritely.Features
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Supplies the Assembly Version to all feature toggles when added to the FeatureEvaluator
    /// </summary>
    public class AssemblyVersionFeatureData : ISharedFeatureData
    {
        // Initialize one time only to avoid additional reflection penalty. 
        private static AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();

        /// <summary>
        /// The version of the currently executing assembly.
        /// </summary>
        public Version AssemblyVersion => assemblyName.Version;
    }
}
