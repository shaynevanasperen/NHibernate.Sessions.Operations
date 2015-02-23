using System;
using Machine.Fakes;
using Machine.Specifications;

namespace NHibernate.Sessions.Operations.Tests
{
	[Subject(typeof(DatabaseQueryCache))]
	class When_getting_an_item_and_it_is_not_in_the_underlying_store : WithSubject<DatabaseQueryCache>
	{
		It should_execute_the_query = () =>
			queryExecuted.ShouldBeTrue();

		It should_return_the_query_result = () =>
			result.ShouldEqual(QueryResult);

		It should_put_the_query_result_in_the_underlying_store = () =>
			cacheStore.WasToldTo(x => x.PutItem(CacheKey, QueryResult, absoluteDuration));

		Because of = () =>
			result = Subject.Get(query, CacheKey, absoluteDuration);

		Establish context = () =>
		{
			cacheStore = An<ICacheStore>();
			Subject = new DatabaseQueryCache(cacheStore);
		};

		static int query()
		{
			queryExecuted = true;
			return QueryResult;
		}

		static bool queryExecuted;
		const int QueryResult = 301;
		static TimeSpan absoluteDuration = TimeSpan.FromMinutes(10);
		const string CacheKey = "CacheKey";
		static ICacheStore cacheStore;
		static int result;
	}

	[Subject(typeof(DatabaseQueryCache))]
	class When_getting_an_item_and_it_is_in_the_underlying_store : WithSubject<DatabaseQueryCache>
	{
		It should_not_execute_the_query = () =>
			queryExecuted.ShouldBeFalse();

		It should_return_the_cached_result = () =>
			result.ShouldEqual(CacheResult);

		It should_not_put_the_query_result_in_the_underlying_store = () =>
			cacheStore.WasNotToldTo(x => x.PutItem(Machine.Fakes.Param.IsAny<string>(), Machine.Fakes.Param.IsAny<object>(), Machine.Fakes.Param.IsAny<TimeSpan>()));

		Because of = () =>
			result = Subject.Get(query, CacheKey, absoluteDuration);

		Establish context = () =>
		{
			cacheStore = An<ICacheStore>();
			cacheStore.WhenToldTo(x => x.GetItem(CacheKey)).Return(CacheResult);
			Subject = new DatabaseQueryCache(cacheStore);
		};

		static int query()
		{
			queryExecuted = true;
			return 0;
		}

		static bool queryExecuted;
		const int CacheResult = 820;
		static TimeSpan absoluteDuration = TimeSpan.FromMinutes(10);
		const string CacheKey = "CacheKey";
		static ICacheStore cacheStore;
		static int result;
	}

	[Subject(typeof(DatabaseQueryCache))]
	class When_getting_an_item_and_it_is_in_the_underlying_store_but_with_wrong_type : WithSubject<DatabaseQueryCache>
	{
		It should_execute_the_query = () =>
			queryExecuted.ShouldBeTrue();

		It should_return_the_query_result = () =>
			result.ShouldEqual(QueryResult);

		It should_put_the_query_result_in_the_underlying_store = () =>
			cacheStore.WasToldTo(x => x.PutItem(CacheKey, QueryResult, absoluteDuration));

		Because of = () =>
			result = Subject.Get(query, CacheKey, absoluteDuration);

		Establish context = () =>
		{
			cacheStore = An<ICacheStore>();
			cacheStore.WhenToldTo(x => x.GetItem(CacheKey)).Return("wrong type");
			Subject = new DatabaseQueryCache(cacheStore);
		};

		static int query()
		{
			queryExecuted = true;
			return QueryResult;
		}

		static bool queryExecuted;
		const int QueryResult = 827;
		static TimeSpan absoluteDuration = TimeSpan.FromMinutes(10);
		const string CacheKey = "CacheKey";
		static ICacheStore cacheStore;
		static int result;
	}

	[Subject(typeof(DatabaseQueryCache))]
	class When_getting_an_item_and_it_has_previously_been_cached_as_a_null_marker : WithSubject<DatabaseQueryCache>
	{
		It should_not_execute_the_query = () =>
			queryExecuted.ShouldBeFalse();

		It should_return_the_default_for_the_query_result_type = () =>
			result.ShouldEqual(0);

		It should_not_put_anything_in_the_underlying_store = () =>
			cacheStore.WasNotToldTo(x => x.PutItem(CacheKey, QueryResult, absoluteDuration));

		Because of = () =>
			result = Subject.Get(query, CacheKey, absoluteDuration);

		Establish context = () =>
		{
			cacheStore = An<ICacheStore>();
			cacheStore.WhenToldTo(x => x.GetItem(CacheKey)).Return(new NullToken());
			Subject = new DatabaseQueryCache(cacheStore);
		};

		static int query()
		{
			queryExecuted = true;
			return QueryResult;
		}

		static bool queryExecuted;
		const int QueryResult = 827;
		static TimeSpan absoluteDuration = TimeSpan.FromMinutes(10);
		const string CacheKey = "CacheKey";
		static ICacheStore cacheStore;
		static int result;
	}

	[Subject(typeof(DatabaseQueryCache))]
	class When_getting_an_item_and_it_is_not_in_the_underlying_store_and_query_returns_null_and_cache_nulls_flag_is_false : WithSubject<DatabaseQueryCache>
	{
		It should_execute_the_query = () =>
			queryExecuted.ShouldBeTrue();

		It should_return_the_query_result = () =>
			result.ShouldEqual(QueryResult);

		It should_not_put_anything_in_the_underlying_store = () =>
			cacheStore.WasNotToldTo(x => x.PutItem(Machine.Fakes.Param.IsAny<string>(), Machine.Fakes.Param.IsAny<object>(), Machine.Fakes.Param.IsAny<TimeSpan>()));

		Because of = () =>
			result = Subject.Get(query, CacheKey, absoluteDuration);

		Establish context = () =>
		{
			cacheStore = An<ICacheStore>();
			Subject = new DatabaseQueryCache(cacheStore);
		};

		static object query()
		{
			queryExecuted = true;
			return QueryResult;
		}

		static bool queryExecuted;
		const object QueryResult = null;
		static TimeSpan absoluteDuration = TimeSpan.FromMinutes(10);
		const string CacheKey = "CacheKey";
		static ICacheStore cacheStore;
		static object result;
	}

	[Subject(typeof(DatabaseQueryCache))]
	class When_getting_an_item_and_it_is_not_in_the_underlying_store_and_query_returns_null_and_cache_nulls_flag_is_true : WithSubject<DatabaseQueryCache>
	{
		It should_execute_the_query = () =>
			queryExecuted.ShouldBeTrue();

		It should_return_the_query_result = () =>
			result.ShouldEqual(QueryResult);

		It should_put_a_null_marker_in_the_underlying_store = () =>
			cacheStore.WasToldTo(x => x.PutItem(Machine.Fakes.Param.IsAny<string>(), Machine.Fakes.Param.IsAny<NullToken>(), Machine.Fakes.Param.IsAny<TimeSpan>()));

		Because of = () =>
			result = Subject.Get(query, CacheKey, absoluteDuration, true);

		Establish context = () =>
		{
			cacheStore = An<ICacheStore>();
			Subject = new DatabaseQueryCache(cacheStore);
		};

		static object query()
		{
			queryExecuted = true;
			return QueryResult;
		}

		static bool queryExecuted;
		const object QueryResult = null;
		static TimeSpan absoluteDuration = TimeSpan.FromMinutes(10);
		const string CacheKey = "CacheKey";
		static ICacheStore cacheStore;
		static object result;
	}
}
