using System;
using System.Web;
using Quarks;

namespace NHibernate.Sessions.Operations
{
	public class HttpRuntimeCacheStore : ICacheStore
	{
		public object GetItem(string cacheKey)
		{
			return HttpRuntime.Cache[cacheKey];
		}

		public void PutItem(string cacheKey, object item, TimeSpan absoluteDuration)
		{
			HttpRuntime.Cache.Insert(cacheKey, item, null, SystemTime.Now.Add(absoluteDuration), TimeSpan.Zero);
		}
	}
}
