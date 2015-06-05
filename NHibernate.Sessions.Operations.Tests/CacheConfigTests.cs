using System;
using System.Collections.Generic;
using Machine.Fakes;
using Machine.Specifications;

namespace NHibernate.Sessions.Operations.Tests
{
	[Subject(typeof(CacheConfig))]
	class When_building_cache_key_with_no_vary_by : WithSubject<CacheConfig>
	{
		It should_return_the_query_type_name = () =>
			result.ShouldEqual(TestCacheKey.Prefix);

		Because of = () =>
			result = Subject.BuildCacheKey(TestCacheKey.Prefix);

		static string result;
	}

	[Subject(typeof(CacheConfig))]
	class When_building_cache_key_with_vary_by_having_only_nulls : WithSubject<CacheConfig>
	{
		It should_return_the_query_type_name_appended_with_an_underscore_for_each_null_property = () =>
			result.ShouldEqual(TestCacheKey.Prefix + "__");

		Establish context = () =>
			Subject.VaryBy = new { queryParameters.Null, NullToo = queryParameters.Null };

		Because of = () =>
			result = Subject.BuildCacheKey(TestCacheKey.Prefix);

		static string result;
		static QueryParameters queryParameters = new QueryParameters { Null = null };
	}

	[Subject(typeof(CacheConfig))]
	class When_building_cache_key_with_vary_by_as_an_object : WithSubject<CacheConfig>
	{
		It should_return_the_query_type_name_and_the_flattened_vary_by_property_values_ordered_by_property_name_joined_by_underscores = () =>
			result.ShouldEqual(TestCacheKey.Prefix + "_a_1__Friday_one_two_three_x_y_z_i_ii_iii_£_$_#_first_second");

		Establish context = () =>
			Subject.VaryBy = new
			{
				d = queryParameters.Enum,
				e = queryParameters.Enumerable,
				f = queryParameters.NestedEnumerable,
				a = queryParameters.String,
				b = queryParameters.Integer,
				c = queryParameters.Null,
				g = new { b = "second", a = "first" }
			};

		Because of = () =>
			result = Subject.BuildCacheKey(TestCacheKey.Prefix);

		static string result;

		static QueryParameters queryParameters = new QueryParameters
		{
			String = "a",
			Integer = 1,
			Null = null,
			Enum = DayOfWeek.Friday,
			Enumerable = new[] { "one", "two", "three" },
			NestedEnumerable = new[] { new[] { "x", "y", "z" }, new[] { "i", "ii", "iii" }, new[] { "£", "$", "#" } }
		};
	}

	[Subject(typeof(CacheConfig))]
	class When_building_cache_key_with_vary_by_as_an_enumerable : WithSubject<CacheConfig>
	{
		It should_return_the_query_type_name_and_the_flattened_vary_by_values_joined_by_underscores = () =>
			result.ShouldEqual(TestCacheKey.Prefix + "_a_1__Friday_one_two_three_x_y_z_i_ii_iii_£_$_#_first_second");

		Establish context = () =>
			Subject.VaryBy = new[]
			{
				queryParameters.String,
				queryParameters.Integer,
				queryParameters.Null,
				queryParameters.Enum,
				queryParameters.Enumerable,
				queryParameters.NestedEnumerable,
				new { b = "second", a = "first" }
			};

		Because of = () =>
			result = Subject.BuildCacheKey(TestCacheKey.Prefix);

		static string result;

		static QueryParameters queryParameters = new QueryParameters
		{
			String = "a",
			Integer = 1,
			Null = null,
			Enum = DayOfWeek.Friday,
			Enumerable = new[] { "one", "two", "three" },
			NestedEnumerable = new[] { new[] { "x", "y", "z" }, new[] { "i", "ii", "iii" }, new[] { "£", "$", "#" } }
		};
	}

	[Subject(typeof(CacheConfig))]
	class When_building_cache_key_when_vary_by_as_a_string_only : WithSubject<CacheConfig>
	{
		It should_return_the_query_type_name_and_the_vary_by_string_joined_with_an_underscore = () =>
			result.ShouldEqual(TestCacheKey.Prefix + "_varyby");

		Establish context = () =>
			Subject.VaryBy = "varyby";

		Because of = () =>
			result = Subject.BuildCacheKey(TestCacheKey.Prefix);

		static string result;
	}

	[Subject(typeof(CacheConfig))]
	class When_building_cache_key_when_vary_by_is_a_primitive_type : WithSubject<CacheConfig>
	{
		It should_return_the_query_type_name_and_the_vary_by_value_joined_with_an_underscore = () =>
			result.ShouldEqual(TestCacheKey.Prefix + "_1");

		Establish context = () =>
			Subject.VaryBy = 1;

		Because of = () =>
			result = Subject.BuildCacheKey(TestCacheKey.Prefix);

		static string result;
	}

	[Subject(typeof(CacheConfig))]
	class When_building_cache_key_when_vary_by_is_an_enum_value : WithSubject<CacheConfig>
	{
		It should_return_the_query_type_name_and_the_vary_by_value_joined_with_an_underscore = () =>
			result.ShouldEqual(TestCacheKey.Prefix + "_" + TestEnum.EnumValue);

		Establish context = () =>
			Subject.VaryBy = TestEnum.EnumValue;

		Because of = () =>
			result = Subject.BuildCacheKey(TestCacheKey.Prefix);

		static string result;

		enum TestEnum
		{
			EnumValue
		}
	}

	[Subject(typeof(CacheConfig))]
	class When_building_cache_key_when_vary_by_is_a_datetime : WithSubject<CacheConfig>
	{
		It should_return_the_query_type_name_and_the_vary_by_string_representation_joined_with_an_underscore = () =>
			result.ShouldEqual(TestCacheKey.Prefix + "_" + DateTime.Today);

		Establish context = () =>
			Subject.VaryBy = DateTime.Today;

		Because of = () =>
			result = Subject.BuildCacheKey(TestCacheKey.Prefix);

		static string result;
	}

	[Subject(typeof(CacheConfig))]
	class When_building_cache_key_when_vary_by_is_an_object_without_public_properties : WithSubject<CacheConfig>
	{
		It should_return_the_query_type_name_and_the_vary_by_tostring_value_joined_with_an_underscore = () =>
			result.ShouldEqual(TestCacheKey.Prefix + "_" + guid);

		Establish context = () =>
			Subject.VaryBy = guid;

		Because of = () =>
			result = Subject.BuildCacheKey(TestCacheKey.Prefix);

		static string result;
		static Guid guid = Guid.NewGuid();
	}

	class QueryParameters
	{
		public string String { get; set; }
		public int Integer { get; set; }
		public object Null { get; set; }
		public DayOfWeek Enum { get; set; }
		public IEnumerable<string> Enumerable { get; set; }
		public IEnumerable<IEnumerable<string>> NestedEnumerable { get; set; }
	}

	class TestCacheKey
	{
		public const string Prefix = "cachePrefix";
	}
}
