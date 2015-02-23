using System;

namespace NHibernate.Sessions.Operations
{
	public interface ICacheStore
	{
		object GetItem(string cacheKey);
		void PutItem(string cacheKey, object item, TimeSpan absoluteDuration);
	}
}