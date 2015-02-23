using System;
using System.Linq;
using System.Reflection;
using NHibernate.Util;

namespace NHibernate.Sessions.Operations
{
	public abstract class DatabaseOperation : IEquatable<DatabaseOperation>
	{
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (ReferenceEquals(this, obj))
				return true;

			var other = obj as DatabaseOperation;

			return other != null && Equals(other);
		}

		public bool Equals(DatabaseOperation other)
		{
			if (other.GetType() != GetType())
				return false;

			return GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).All(x =>
			{
				var thisValue = x.GetValue(this, null);
				var otherValue = x.GetValue(other, null);

				if (thisValue == null && otherValue == null) return true;

				if (ReferenceEquals(thisValue, otherValue)) return true;

				return thisValue != null && thisValue.Equals(otherValue);
			});
		}

		public override int GetHashCode()
		{
			// ReSharper disable NonReadonlyFieldInGetHashCode
			var publicPropertyValues = GetType()
				.GetProperties(BindingFlags.Instance | BindingFlags.Public)
				.Select(x => x.GetValue(this, null))
				.Where(x => x != null);
			// ReSharper restore NonReadonlyFieldInGetHashCode

			unchecked
			{
				var hash = 17;
				// ReSharper disable once AccessToModifiedClosure
				publicPropertyValues.ForEach(x => hash = hash * 23 + x.GetHashCode());
				hash = hash * 23 + GetType().GetHashCode();
				return hash;
			}
		}
	}
}