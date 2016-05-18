<img src="NHibernate.Sessions.Operations.png" align="right" />
[![Build status](https://ci.appveyor.com/api/projects/status/3p47e7ky2w2i3tnq?svg=true)](https://ci.appveyor.com/project/shaynevanasperen/nhibernate-sessions-operations)
[![NuGet](https://buildstats.info/nuget/NHibernate.Sessions.Operations)](https://www.nuget.org/packages/NHibernate.Sessions.Operations)
NHibernate.Sessions.Operations
==============================

A [query object pattern](http://martinfowler.com/eaaCatalog/queryObject.html) built on top of
[NHibernate.Sessions](https://www.nuget.org/packages/NHibernate.Sessions/). This library makes
it easy to build and consume your own query objects, with support for "3rd level" caching and
query-result transformation. It provides an `IDatabases` interface, which you use to invoke queries
and commands. If you derive your query and command objects from the provided base classes, you
can then mock calls to the `IDatabases` interface in your unit tests.

Here's what the `IDatabases` interface looks like:

```c#
public interface IDatabases
{
	ISessionManager SessionManager { get; }

	T Query<T>(IDatabaseQuery<T> query);
	T Query<T>(ICachedDatabaseQuery<T> query);

	void Command(IDatabaseCommand command);
	T Command<T>(IDatabaseCommand<T> command);
}
```

##Creating an instance of `IDatabases`
Typically you would create an instance of `IDatabases` as a singleton during application startup
and then inject it into your application's request processing classes using your favourite
dependency injection framework. Here's how to create an instance of `IDatabases` using the provided
implementation:

```c#
var sessionManager = Configure.NHibernate
	.UsingConfigurationFactory(sessionFactoryKey => new Cfg.Configuration().Configure("~/nhibernate.database1.cfg.xml"))
	.RegisterSessionFactory("Database1")
	.BuildSessionManager();
var databases = new Databases(sessionManager);
```

Or if you want to use cached database queries, (which you'll see later) you would construct it like this:

```c#
var databases = new Databases(sessionManager, new DatabaseQueryCache(new HttpRuntimeCacheStore()));
```

The `DatabaseQueryCache` class is an implementation of `IDatabaseQueryCache`, and you can provide your
own implementation if you wish. The constructor parameter for `DatabaseQueryCache` is an implementation
of `ICacheStore`, which looks like this:

```c#
public interface ICacheStore
{
	object GetItem(string cacheKey);
	void PutItem(string cacheKey, object item, TimeSpan absoluteDuration);
}
```

As you can see above, I've used the provided implementation called `HttpRuntimeCacheStore` which simply
leverages the `HttpRuntime.Cache` object. You can also provide your own implementation of `ICacheStore` for
caching via Memcached or Redis for example.

##Using database queries and commands
Here's how to create and use database query and command objects. Parameters for database queries
and commands are provided via puplic properties on the query/command objects. For the purposes of
the following examples, consider the following poco entity class:

```c#
class Database1Poco
{
	public int Property1 { get; set; }
	public string Property2 { get; set; }
}
```

_For help on using the `ISessionManager` interface, see [NHibernate.Sessions](https://github.com/shaynevanasperen/NHibernate.Sessions)_

###1. `DatabaseQuery`

The `DatabaseQuery` class takes a generic argument representing the return type of the query. Here's
an example of a simple database query object:

```c#
class Database1PocoByProperty1 : DatabaseQuery<Database1Poco>
{
	public override Database1Poco Execute(ISessionManager sessionManager)
	{
		return sessionManager.Session.Query<Database1Poco>().SingleOrDefault(x => x.Property1 == Property1);
	}

	public int Property1 { get; set; }
}
```

And to use this database query, you would make use of the `IDatabases` interface like this:

```c#
var database1Poco = _databases.Query(new Database1PocoByProperty1 { Property1 = 1 });
```

###2. `CachedDatabaseQuery`

Here's an example of a cached database query object:

```c#
class Database1PocosByProperty2 : CachedDatabaseQuery<IReadOnlyCollection<Database1Poco>>
{
	protected override void ConfigureCache(CacheConfig cacheConfig)
	{
		cacheConfig.AbsoluteDuration = TimeSpan.FromMinutes(5);
		cacheConfig.VaryBy = Property2;
	}

	protected override IReadOnlyCollection<Database1Poco> QueryDatabase(ISessionManager sessionManager)
	{
		return sessionManager.Session.Query<Database1Poco>().Where(x => x.Property2 == Property2).ToArray();
	}

	public string Property2 { get; set; }
}
```

And to use this database query, you would make use of the `IDatabases` interface like this:

```c#
var database1Pocos = _databases.Query(new Database1PocosByProperty2 { Property2 = "value" });
```

When configuring your cache, you can set the absolute duration as a `TimeSpan` and you can vary the
cache by assigning the value(s) from the property parameter(s) of the query. When executing the query,
the `CacheConfig` class builds a cache key from the value of the `VaryBy` property. You can combine
multiple values in the `VaryBy` property by assigning an anonymous object or an array, like this:

```c#
cacheConfig.VaryBy = new { Property1, Property2 };
cacheConfig.VaryBy = new[] { Property1, Property2 };	
```

###3. `TransformedCachedDatabaseQuery`

Sometimes you want to cache only intermediate results rather than the final query result, so that
similar queries can share the intermediate results from the cache and then transform the intermediate
results into a final result. To do this, you need to derive your query object from
`TransformedCachedDatabaseQuery`, which takes two generic arguments - one for the type of the database
result (which is what will be cached) and another for the type of the transformed result:

```c#
class Database1PocoByProperty1AndProperty2 : TransformedCachedDatabaseQuery<IReadOnlyCollection<Database1Poco>, Database1Poco>
{
	protected override void ConfigureCache(CacheConfig cacheConfig)
	{
		cacheConfig.AbsoluteDuration = TimeSpan.FromMinutes(5);
		cacheConfig.VaryBy = Property2;
	}

	protected override IReadOnlyCollection<Database1Poco> QueryDatabase(ISessionManager sessionManager)
	{
		return sessionManager.Session.Query<Database1Poco>().Where(x => x.Property2 == Property2).ToArray();
	}

	protected override Database1Poco TransformDatabaseResult(IReadOnlyCollection<Database1Poco> databaseResult)
	{
		return databaseResult.SingleOrDefault(x => x.Property1 == Property1);
	}

	public int Property1 { get; set; }
	public string Property2 { get; set; }
}
```

And to use this database query, you would make use of the `IDatabases` interface like this:

```c#
var database1Poco = _databases.Query(new Database1PocoByProperty1AndProperty2 { Property1 = 1, Property2 = "value" });
```

###4. `DatabaseCommand`
There are two types of database commands - those that don't return any value, and those that do
return a value. Here is an example of a database command object that doesn't return a value:

```c#
class SaveDatabase1Poco : DatabaseCommand
{
	public override void Execute(ISessionManager sessionManager)
	{
		sessionManager.Session.Save(Database1Poco);
	}

	public Database1Poco Database1Poco { get; set; }
}
```

And to use this database command, you would make use of the `IDatabases` interface like this:

```c#
_databases.Command(new SaveDatabase1Poco { Database1Poco = new Database1Poco() });
```

Here's an example of a database command object that returns a value:

```c#
class SaveDatabase1PocoWithReturnValue : DatabaseCommand<bool>
{
	public override bool Execute(ISessionManager sessionManager)
	{
		try
		{
			sessionManager.Session.Save(Database1Poco);
			return true;
		}
		catch
		{
			return false;
		}
	}

	public Database1Poco Database1Poco { get; set; }
}
```

And to use this database command, you would make use of the `IDatabases` interface like this:

```c#
var success = _databases.Command(new SaveDatabase1PocoWithReturnValue { Database1Poco = new Database1Poco() });
```

##Mocking `IDatabases` calls in unit tests
The provided database query and command classes share a common abstract base class named
`DatabaseOperation`. Due to this class having a custom `Equals` method, which by using reflection
compares the public properties of the the instances that derive from it, you can write the
following code with your favourite mocking library (I have used Machine.Fakes for this example):

```c#
var expectedProperty1Value = 1;
The<IDatabases>().WhenToldTo(x => x.Query(new Database1PocoByProperty1 { Property1 = expectedProperty1Value })).Return(new Database1Poco());	
	
var expectedDatabase1PocoToSave = new Database1Poco();
The<IDatabases>().WasToldTo(x => x.Command(new SaveDatabase1Poco { Database1Poco = expectedDatabase1PocoToSave })).OnlyOnce();
```