using System;
using System.Collections.Generic;
using Quarks.ObjectExtensions;

namespace NHibernate.Sessions.Operations
{
	public class CacheConfig
	{
		public TimeSpan AbsoluteDuration { get; set; }

		/// <summary>
		/// An object or enumerable representing the parameter value(s) of the query.
		/// This is used in conjunction with the name of the query class to form the cache key.
		/// For example: <c>VaryBy = new { Parameter1, Parameter2, Parameter3.[Value], Parameter4.InnerProperty }</c>
		/// or just <c>VaryBy = Parameter</c>
		/// </summary>
		public object VaryBy { get; set; }

		/// <summary>
		/// Specify whether or not null results should be cached. Defaults to false.
		/// </summary>
		public bool CacheNulls { get; set; }
		
		internal string BuildCacheKey(string prefix)
		{
			var segments = new List<object> { prefix };

			if (VaryBy == null)
				return string.Join("_", segments);

			segments.AddRange(VaryBy.Flatten());

			return string.Join("_", segments);
		}

		internal static CacheConfig None
		{
			get { return new CacheConfig { AbsoluteDuration = TimeSpan.Zero }; }
		}
	}
}