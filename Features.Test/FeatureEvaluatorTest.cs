namespace Features.Test
{
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class FeatureEvaluatorTest
    {
        [Fact]
        public void Constructor_throws_on_null_resolver()
        {
            Func<FeatureEvaluator> act = () => new FeatureEvaluator(null, Enumerable.Empty<ISharedFeatureData>());

            act.Should().Throw<ArgumentNullException>().WithMessage("*resolver*");
        }

        [Fact]
        public void Constructor_does_not_require_shared_feature_data()
        {
            var resolver = Substitute.For<IFeatureResolver>();

            Func<FeatureEvaluator> act = () => new FeatureEvaluator(resolver);
            act.Should().NotThrow();

            act = () => new FeatureEvaluator(resolver, null);
            act.Should().NotThrow();
        }

        [Fact]
        public async Task IsOnAsync_returns_result_of_resolver_IsOnAsync()
        {
            var resolver = Substitute.For<IFeatureResolver>();
            var evaluator = new FeatureEvaluator(resolver);

            resolver.IsOnAsync(Arg.Any<string>(), Arg.Any<IDictionary<string, object>>()).Returns(false);
            var result = await evaluator.IsOnAsync("ignored");
            result.Should().BeFalse();

            resolver.IsOnAsync(Arg.Any<string>(), Arg.Any<IDictionary<string, object>>()).Returns(true);
            result = await evaluator.IsOnAsync("ignored");
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsOnAsync_sends_name_to_resolver()
        {
            var resolver = Substitute.For<IFeatureResolver>();
            var evaluator = new FeatureEvaluator(resolver);
            var givenName = "Feature Name";

            await evaluator.IsOnAsync(givenName);

            await resolver.Received().IsOnAsync(givenName, Arg.Any<IDictionary<string, object>>());
        }

        [Fact]
        public async Task WithT_adds_data_for_resolver()
        {
            var resolver = Substitute.For<IFeatureResolver>();
            var evaluator = new FeatureEvaluator(resolver);
            var feature = new TestFeatureWithData(evaluator);

            var person = new Person { FirstName = "George", LastName = "Washington" };

            await feature.IsOnAsync(person);

            await resolver.Received().IsOnAsync(Arg.Any<string>(), Arg.Is<IDictionary<string, object>>(d =>
                d.Count == 2 &&
                d.ContainsKey("FirstName") &&
                d.ContainsKey("LastName") &&
                d["FirstName"].ToString() == "George" &&
                d["LastName"].ToString() == "Washington"
            ));
        }

        [Fact]
        public async Task With_adds_multiple_types_for_resolver()
        {
            var resolver = Substitute.For<IFeatureResolver>();
            var evaluator = new FeatureEvaluator(resolver);
            var feature = new TestFeatureWithData(evaluator);

            var person = new Person { FirstName = "George", LastName = "Washington" };
            var country = new Country { CountryName = "USA" };

            await feature.IsOnAsync(person, country);

            await resolver.Received().IsOnAsync(Arg.Any<string>(), Arg.Is<IDictionary<string, object>>(d =>
                d.Count == 3 &&
                d.ContainsKey("FirstName") &&
                d.ContainsKey("LastName") &&
                d.ContainsKey("CountryName") &&
                d["FirstName"].ToString() == "George" &&
                d["LastName"].ToString() == "Washington" &&
                d["CountryName"].ToString() == "USA"
            ));
        }

        [Fact]
        public async Task With_adds_multiple_types_and_later_types_override_earlier_types_for_resolver()
        {
            var resolver = Substitute.For<IFeatureResolver>();
            var evaluator = new FeatureEvaluator(resolver);
            var feature = new TestFeatureWithData(evaluator);

            var person1 = new Person { FirstName = "George", LastName = "Washington" };
            var person2 = new Person { FirstName = "John", LastName = "Adams" };
            var country = new Country { CountryName = "USA" };

            await feature.IsOnAsync(person1, country, person2);

            await resolver.Received().IsOnAsync(Arg.Any<string>(), Arg.Is<IDictionary<string, object>>(d =>
                d.Count == 3 &&
                d.ContainsKey("FirstName") &&
                d.ContainsKey("LastName") &&
                d.ContainsKey("CountryName") &&
                d["FirstName"].ToString() == "John" &&
                d["LastName"].ToString() == "Adams" &&
                d["CountryName"].ToString() == "USA"
            ));
        }

        [Fact]
        public async Task Constructor_adds_data_for_resolver()
        {
            var resolver = Substitute.For<IFeatureResolver>();
            var person = new Person { FirstName = "George", LastName = "Washington" };
            var evaluator = new FeatureEvaluator(resolver, new List<ISharedFeatureData>() { person } );
            var feature = new TestFeatureWithData(evaluator);

            await feature.IsOnAsync();

            await resolver.Received().IsOnAsync(Arg.Any<string>(), Arg.Is<IDictionary<string, object>>(d =>
                d.Count == 2 &&
                d.ContainsKey("FirstName") &&
                d.ContainsKey("LastName") &&
                d["FirstName"].ToString() == "George" &&
                d["LastName"].ToString() == "Washington"
            ));
        }

        [Fact]
        public async Task Constructor_adds_multiple_types_for_resolver()
        {
            var resolver = Substitute.For<IFeatureResolver>();
            var person = new Person { FirstName = "George", LastName = "Washington" };
            var country = new Country { CountryName = "USA" };
            var evaluator = new FeatureEvaluator(resolver, new List<ISharedFeatureData>() { person, country } );
            var feature = new TestFeatureWithData(evaluator);

            await feature.IsOnAsync();

            await resolver.Received().IsOnAsync(Arg.Any<string>(), Arg.Is<IDictionary<string, object>>(d =>
                d.Count == 3 &&
                d.ContainsKey("FirstName") &&
                d.ContainsKey("LastName") &&
                d.ContainsKey("CountryName") &&
                d["FirstName"].ToString() == "George" &&
                d["LastName"].ToString() == "Washington" &&
                d["CountryName"].ToString() == "USA"
            ));
        }

        [Fact]
        public async Task Constructor_adds_multiple_types_and_later_types_override_earlier_types_for_resolver()
        {
            var resolver = Substitute.For<IFeatureResolver>();
            var person1 = new Person { FirstName = "George", LastName = "Washington" };
            var person2 = new Person { FirstName = "John", LastName = "Adams" };
            var country = new Country { CountryName = "USA" };
            var evaluator = new FeatureEvaluator(resolver, new List<ISharedFeatureData>() { person1, country, person2 });
            var feature = new TestFeatureWithData(evaluator);

            await feature.IsOnAsync();

            await resolver.Received().IsOnAsync(Arg.Any<string>(), Arg.Is<IDictionary<string, object>>(d =>
                d.Count == 3 &&
                d.ContainsKey("FirstName") &&
                d.ContainsKey("LastName") &&
                d.ContainsKey("CountryName") &&
                d["FirstName"].ToString() == "John" &&
                d["LastName"].ToString() == "Adams" &&
                d["CountryName"].ToString() == "USA"
            ));
        }

        [Fact]
        public async Task WithT_overrides_constructor_data_for_resolver()
        {
            var resolver = Substitute.For<IFeatureResolver>();
            var person1 = new Person { FirstName = "George", LastName = "Washington" };
            var evaluator = new FeatureEvaluator(resolver, new List<ISharedFeatureData>() { person1 });
            var feature = new TestFeatureWithData(evaluator);

            var person2 = new Person { FirstName = "John", LastName = "Adams" };
            await feature.IsOnAsync(person2);

            await resolver.Received().IsOnAsync(Arg.Any<string>(), Arg.Is<IDictionary<string, object>>(d =>
                d.Count == 2 &&
                d.ContainsKey("FirstName") &&
                d.ContainsKey("LastName") &&
                d["FirstName"].ToString() == "John" &&
                d["LastName"].ToString() == "Adams"
            ));
        }

        [Fact]
        public async Task With_overrides_multiple_constructor_types_for_resolver()
        {
            var resolver = Substitute.For<IFeatureResolver>();
            var person1 = new Person { FirstName = "George", LastName = "Washington" };
            var country1 = new Country { CountryName = "Britian" };
            var evaluator = new FeatureEvaluator(resolver, new List<ISharedFeatureData>() { person1, country1 });
            var feature = new TestFeatureWithData(evaluator);

            var person2 = new Person { FirstName = "John", LastName = "Adams" };
            var country2 = new Country { CountryName = "USA" };
            await feature.IsOnAsync(person2, country2);

            await resolver.Received().IsOnAsync(Arg.Any<string>(), Arg.Is<IDictionary<string, object>>(d =>
                d.Count == 3 &&
                d.ContainsKey("FirstName") &&
                d.ContainsKey("LastName") &&
                d.ContainsKey("CountryName") &&
                d["FirstName"].ToString() == "John" &&
                d["LastName"].ToString() == "Adams" &&
                d["CountryName"].ToString() == "USA"
            ));
        }

        [Fact]
        public async Task With_overrides_partial_constructor_types_for_resolver()
        {
            var resolver = Substitute.For<IFeatureResolver>();
            var person1 = new Person { FirstName = "John", LastName = "Adams" };
            var country1 = new Country { CountryName = "Britian" };
            var evaluator = new FeatureEvaluator(resolver, new List<ISharedFeatureData>() { person1, country1 });
            var feature = new TestFeatureWithData(evaluator);

            var person2 = new Person { FirstName = "John Quincy", LastName = "Adams" };
            var country2 = new Country { CountryName = "USA" };
            await feature.IsOnAsync(person2, country2);

            await resolver.Received().IsOnAsync(Arg.Any<string>(), Arg.Is<IDictionary<string, object>>(d =>
                d.Count == 3 &&
                d.ContainsKey("FirstName") &&
                d.ContainsKey("LastName") &&
                d.ContainsKey("CountryName") &&
                d["FirstName"].ToString() == "John Quincy" &&
                d["LastName"].ToString() == "Adams" &&
                d["CountryName"].ToString() == "USA"
            ));
        }

        public class Person : ISharedFeatureData
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }
        }

        public class Country : ISharedFeatureData
        {
            public string CountryName { get; set; }
        }

        public class TestFeatureWithData : IFeature
        {
            private readonly IFeatureEvaluator _evaluator;
            public string Name => "Test Feature";

            public TestFeatureWithData(IFeatureEvaluator evaluator)
            {
                _evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
            }

            public Task<bool> IsOnAsync() => _evaluator.IsOnAsync(Name);

            public Task<bool> IsOnAsync<T>(T data) => _evaluator.With(data).IsOnAsync(Name);

            public Task<bool> IsOnAsync(params object[] data) => _evaluator.With(data).IsOnAsync(Name);
        }
    }
}
