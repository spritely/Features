namespace Spritely.Features.Test
{
    using Autofac;
    using Autofac.Features.ResolveAnything;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using Xunit;

    public class FeatureSelectorTest
    {
        [Fact]
        public void Constructor_throws_when_feature_is_null()
        {
            var logger = Substitute.For<ILogger<FeatureSelector<SimpleTestFeature, NewGetSomething, OldGetSomething, IGetSomething>>>();
            Func<FeatureSelector<SimpleTestFeature, NewGetSomething, OldGetSomething, IGetSomething>> act =
                () => new FeatureSelector<SimpleTestFeature, NewGetSomething, OldGetSomething, IGetSomething>(
                    null,
                    () => new NewGetSomething(),
                    () => new OldGetSomething(),
                    logger);

            act.Should().Throw<ArgumentNullException>().WithMessage("*feature*");
        }

        [Fact]
        public void Constructor_throws_when_on_is_null()
        {
            var evaluator = Substitute.For<IFeatureEvaluator>();
            var feature = new SimpleTestFeature(evaluator);
            var logger = Substitute.For<ILogger<FeatureSelector<SimpleTestFeature, NewGetSomething, OldGetSomething, IGetSomething>>>();
            Func<FeatureSelector<SimpleTestFeature, NewGetSomething, OldGetSomething, IGetSomething>> act =
                () => new FeatureSelector<SimpleTestFeature, NewGetSomething, OldGetSomething, IGetSomething>(
                    feature,
                    null,
                    () => new OldGetSomething(),
                    logger);

            act.Should().Throw<ArgumentNullException>().WithMessage("*on*");
        }

        [Fact]
        public void Constructor_throws_when_off_is_null()
        {
            var evaluator = Substitute.For<IFeatureEvaluator>();
            var feature = new SimpleTestFeature(evaluator);
            var logger = Substitute.For<ILogger<FeatureSelector<SimpleTestFeature, NewGetSomething, OldGetSomething, IGetSomething>>>();
            Func<FeatureSelector<SimpleTestFeature, NewGetSomething, OldGetSomething, IGetSomething>> act =
                () => new FeatureSelector<SimpleTestFeature, NewGetSomething, OldGetSomething, IGetSomething>(
                    feature,
                    () => new NewGetSomething(),
                    null,
                    logger);

            act.Should().Throw<ArgumentNullException>().WithMessage("*off*");
        }

        [Fact]
        public void Constructor_does_not_throw_when_logger_is_null()
        {
            var evaluator = Substitute.For<IFeatureEvaluator>();
            var feature = new SimpleTestFeature(evaluator);
            Func<FeatureSelector<SimpleTestFeature, NewGetSomething, OldGetSomething, IGetSomething>> act =
                () => new FeatureSelector<SimpleTestFeature, NewGetSomething, OldGetSomething, IGetSomething>(
                    feature,
                    () => new NewGetSomething(),
                    () => new OldGetSomething(),
                    null);

            act.Should().NotThrow();
        }

        [Fact]
        public async Task CreateAsync_creates_the_correct_type_when_feature_is_on()
        {
            var evaluator = Substitute.For<IFeatureEvaluator>();
            var feature = new SimpleTestFeature(evaluator);
            var selector = new FeatureSelector<SimpleTestFeature, NewGetSomething, OldGetSomething, IGetSomething>(
                feature,
                () => new NewGetSomething(),
                () => new OldGetSomething(),
                null);
            evaluator.IsOnAsync(Arg.Any<string>()).Returns(true);

            var getter = await selector.CreateAsync();
            var result = getter.GetSomething();

            getter.Should().BeOfType<NewGetSomething>();
            result.Should().Be("New");
        }

        [Fact]
        public async Task CreateAsync_creates_the_correct_type_when_feature_is_off()
        {
            var evaluator = Substitute.For<IFeatureEvaluator>();
            var feature = new SimpleTestFeature(evaluator);
            var selector = new FeatureSelector<SimpleTestFeature, NewGetSomething, OldGetSomething, IGetSomething>(
                feature,
                () => new NewGetSomething(),
                () => new OldGetSomething(),
                null);
            evaluator.IsOnAsync(Arg.Any<string>()).Returns(false);

            var getter = await selector.CreateAsync();
            var result = getter.GetSomething();

            getter.Should().BeOfType<OldGetSomething>();
            result.Should().Be("Old");
        }

        [Fact]
        public async Task CreateAsync_logs_on_with_the_correct_type_when_feature_is_on()
        {
            var evaluator = Substitute.For<IFeatureEvaluator>();
            var feature = new SimpleTestFeature(evaluator);
            var logger = Logger.For<FeatureSelector<SimpleTestFeature, NewGetSomething, OldGetSomething, IGetSomething>>();
            var selector = new FeatureSelector<SimpleTestFeature, NewGetSomething, OldGetSomething, IGetSomething>(
                feature,
                () => new NewGetSomething(),
                () => new OldGetSomething(),
                logger);
            evaluator.IsOnAsync(Arg.Any<string>()).Returns(true);

            var getter = await selector.CreateAsync();

             logger.Received().LogInformation(s => s.Contains("NewGetSomething") && s.Contains("Test Feature") && s.Contains("on"));
        }

        [Fact]
        public async Task CreateAsync_logs_off_with_the_correct_type_when_feature_is_off()
        {
            var evaluator = Substitute.For<IFeatureEvaluator>();
            var feature = new SimpleTestFeature(evaluator);
            var logger = Logger.For<FeatureSelector<SimpleTestFeature, NewGetSomething, OldGetSomething, IGetSomething>>();
            var selector = new FeatureSelector<SimpleTestFeature, NewGetSomething, OldGetSomething, IGetSomething>(
                feature,
                () => new NewGetSomething(),
                () => new OldGetSomething(),
                logger);
            evaluator.IsOnAsync(Arg.Any<string>()).Returns(false);

            var getter = await selector.CreateAsync();

            logger.Received().LogInformation(s => s.Contains("OldGetSomething") && s.Contains("Test Feature") && s.Contains("off"));
        }

        [Fact]
        public async Task Resolves_from_a_container()
        {
            var evaluator = Substitute.For<IFeatureEvaluator>();
            var logger = Logger.For<FeatureSelector<SimpleTestFeature, NewGetSomething, OldGetSomething, IGetSomething>>();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            containerBuilder.RegisterInstance(evaluator);
            containerBuilder.RegisterType<FeatureSelector<SimpleTestFeature, NewGetSomething, OldGetSomething, IGetSomething>>().As<IFeatureSelector<IGetSomething>>();
            containerBuilder.RegisterInstance(logger).As<ILogger<FeatureSelector<SimpleTestFeature, NewGetSomething, OldGetSomething, IGetSomething>>>();
            var container = containerBuilder.Build();

            evaluator.IsOnAsync(Arg.Any<string>()).Returns(false);
            var selector = container.Resolve<IFeatureSelector<IGetSomething>>();
            var getter = await selector.CreateAsync();
            var result = getter.GetSomething();
            getter.Should().BeOfType<OldGetSomething>();
            result.Should().Be("Old");

            evaluator.IsOnAsync(Arg.Any<string>()).Returns(true);
            selector = container.Resolve<IFeatureSelector<IGetSomething>>();
            getter = await selector.CreateAsync();
            result = getter.GetSomething();
            getter.Should().BeOfType<NewGetSomething>();
            result.Should().Be("New");
        }

        [Fact]
        public async Task SharedFeatureData_automatically_resolves_when_registered_in_container()
        {
            var expectedAssemblyVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();
            var resolver = Substitute.For<IFeatureResolver>();
            var logger = Logger.For<FeatureSelector<SimpleTestFeature, NewGetSomething, OldGetSomething, IGetSomething>>();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            containerBuilder.RegisterInstance(resolver);
            containerBuilder.RegisterType<MachineNameFeatureData>().As<ISharedFeatureData>();
            containerBuilder.RegisterType<AssemblyVersionFeatureData>().As<ISharedFeatureData>();
            containerBuilder.RegisterType<FeatureEvaluator>().As<IFeatureEvaluator>();
            containerBuilder.RegisterType<FeatureSelector<SimpleTestFeature, NewGetSomething, OldGetSomething, IGetSomething>>().As<IFeatureSelector<IGetSomething>>();
            containerBuilder.RegisterInstance(logger).As<ILogger<FeatureSelector<SimpleTestFeature, NewGetSomething, OldGetSomething, IGetSomething>>>();
            var container = containerBuilder.Build();

            resolver.IsOnAsync(Arg.Any<string>(), Arg.Any<IDictionary<string, object>>()).Returns(true);
            var selector = container.Resolve<IFeatureSelector<IGetSomething>>();
            var getter = await selector.CreateAsync();
            var result = getter.GetSomething();
            getter.Should().BeOfType<NewGetSomething>();
            result.Should().Be("New");

            await resolver.Received().IsOnAsync(Arg.Any<string>(), Arg.Is<IDictionary<string, object>>(d =>
                d.ContainsKey("MachineName") &&
                d["MachineName"].ToString() == Environment.MachineName &&
                d.ContainsKey("AssemblyVersion") &&
                d["AssemblyVersion"].ToString() == expectedAssemblyVersion
            ));
        }

        public interface IGetSomething
        {
            public string GetSomething();
        }

        public class OldGetSomething : IGetSomething
        {
            public string GetSomething() => "Old";
        }

        public class NewGetSomething : IGetSomething
        {
            public string GetSomething() => "New";
        }

        public class SimpleTestFeature : IFeature
        {
            private readonly IFeatureEvaluator _evaluator;
            public string Name => "Test Feature";

            public SimpleTestFeature(IFeatureEvaluator evaluator)
            {
                _evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
            }

            public Task<bool> IsOnAsync() => _evaluator.IsOnAsync(Name);
        }
    }
}
