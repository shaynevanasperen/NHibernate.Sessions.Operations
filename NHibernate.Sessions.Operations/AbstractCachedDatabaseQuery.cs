using System;

namespace NHibernate.Sessions.Operations
{
	public abstract class AbstractCachedDatabaseQuery<T> : DatabaseOperation
	{
		protected abstract void ConfigureCache(CacheConfig cacheConfig);

		protected abstract T QueryDatabase(ISessionManager sessionManager);

		public virtual string CacheKeyPrefix
		{
			get { return GetType().FullName; }
		}

		protected T GetDatabaseResult(ISessionManager sessionManager, IDatabaseQueryCache databaseQueryCache = null)
		{
			var cacheConfig = CacheConfig.None;
			ConfigureCache(cacheConfig);

			if (databaseQueryCache == null || cacheConfig.AbsoluteDuration == TimeSpan.Zero)
				return QueryDatabase(sessionManager);

			return databaseQueryCache.Get(() => QueryDatabase(sessionManager), cacheConfig.BuildCacheKey(CacheKeyPrefix), cacheConfig.AbsoluteDuration, cacheConfig.CacheNulls);
		}
	}
}