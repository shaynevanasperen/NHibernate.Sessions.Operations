using System;
using System.Collections.Generic;
using System.Linq;

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

			segments.AddRange(flatten(VaryBy));

			return string.Join("_", segments);
		}

		static IEnumerable<object> flatten(object source)
		{
			if (source.CanBeRepresentedAsString())
				yield return source;
			else
			{
				foreach (var flattened in source.ToEnumerable().SelectMany(flatten)) {
					yield return flattened;
				}
			}
		}

		internal static CacheConfig None
		{
			get { return new CacheConfig { AbsoluteDuration = TimeSpan.Zero }; }
		}
	}

	static class Extensions
	{
		internal static bool CanBeRepresentedAsString(this object source)
		{
			return source == null ||
				source is string || 
				source is Enum ||
				source is DateTime ||
				source.GetType().IsPrimitive || 
				!source.GetType().GetProperties().Any(); //NOTE: if there are no public properties, assume that the ToString() value is unique enough
		}

		internal static IEnumerable<object> ToEnumerable(this object source)
		{
			var sourceEnumerable = source as IEnumerable<object>;
			return sourceEnumerable ?? source.GetType().GetProperties().OrderBy(x => x.Name).Select(x => x.GetValue(source, null));
		}
	}
}