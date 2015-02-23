using System;

namespace NHibernate.Sessions.Operations
{
	public class DatabaseQueryCache : IDatabaseQueryCache
	{
		readonly ICacheStore _cacheStore;

		public DatabaseQueryCache(ICacheStore cacheStore)
		{
			_cacheStore = cacheStore;
		}

		public T Get<T>(Func<T> executeQuery, string cacheKey, TimeSpan absoluteDuration, bool cacheNulls = false)
		{
			var item = _cacheStore.GetItem(cacheKey);

			if (item is T)
				return (T)item;

			if (item is NullToken)
				return default(T);

			item = executeQuery();

			if (cacheNulls || item != null)
				_cacheStore.PutItem(cacheKey, item ?? new NullToken(), absoluteDuration);

			return (T)item;
		}
	}

	class NullToken { }
}
