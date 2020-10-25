namespace Spritely.Features.Test
{
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using Xunit;

    public class AssemblyVersionFeatureDataTest
    {
        [Fact]
        public async Task FeatureEvaluator_with_AssemblyVersionFeatureData_sends_MachineName_to_resolver()
        {
            var resolver = Substitute.For<IFeatureResolver>();
            var featureData = new MachineNameFeatureData();
            var evaluator = new FeatureEvaluator(resolver, new List<ISharedFeatureData>() { new AssemblyVersionFeatureData() });
            var feature = new TestAssemblyVersionFeatureData(evaluator);
            var expectedValue = Assembly.GetEntryAssembly().GetName().Version.ToString();

            await feature.IsOnAsync();

            await resolver.Received().IsOnAsync(Arg.Any<string>(), Arg.Is<IDictionary<string, object>>(d =>
                d.ContainsKey("AssemblyVersion") &&
                d["AssemblyVersion"].ToString() == expectedValue
            ));
        }

        public class TestAssemblyVersionFeatureData : IFeature
        {
            private readonly IFeatureEvaluator _evaluator;
            public string Name => "Test Feature";

            public TestAssemblyVersionFeatureData(IFeatureEvaluator evaluator)
            {
                _evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
            }

            public Task<bool> IsOnAsync() => _evaluator.IsOnAsync(Name);
        }
    }
}
