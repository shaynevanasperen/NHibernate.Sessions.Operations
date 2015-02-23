using System;

namespace NHibernate.Sessions.Operations
{
	public interface IDatabaseQueryCache
	{
		T Get<T>(Func<T> executeQuery, string cacheKey, TimeSpan absoluteDuration, bool cacheNulls = false);
	}
}