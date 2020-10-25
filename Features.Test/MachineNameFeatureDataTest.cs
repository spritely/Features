namespace Features.Test
{
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class MachineNameFeatureDataTest
    {
        [Fact]
        public async Task FeatureEvaluator_with_MachineNameFeatureData_sends_MachineName_to_resolver()
        {
            var resolver = Substitute.For<IFeatureResolver>();
            var featureData = new MachineNameFeatureData();
            var evaluator = new FeatureEvaluator(resolver, new List<ISharedFeatureData>() { new MachineNameFeatureData() });
            var feature = new TestEnvironmentFeatureData(evaluator);

            await feature.IsOnAsync();

            await resolver.Received().IsOnAsync(Arg.Any<string>(), Arg.Is<IDictionary<string, object>>(d =>
                d.ContainsKey("MachineName") &&
                d["MachineName"].ToString() == Environment.MachineName
            ));
        }

        public class TestEnvironmentFeatureData : IFeature
        {
            private readonly IFeatureEvaluator _evaluator;
            public string Name => "Test Feature";

            public TestEnvironmentFeatureData(IFeatureEvaluator evaluator)
            {
                _evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
            }

            public Task<bool> IsOnAsync() => _evaluator.IsOnAsync(Name);
        }
    }
}
