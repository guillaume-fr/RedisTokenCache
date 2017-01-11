using System;
using StackExchange.Redis;
using Thinktecture.IdentityModel.Web;

namespace Moissaing.ThinktectureIdentityModel.Redis
{
  /// <summary>
  /// Implementation of token cache repository using redis storage
  /// </summary>
  public sealed class RedisTokenCacheRepository : ITokenCacheRepository, IDisposable
  {
    /// <summary>
    /// Intializes an instance of RedisTokenCacheRepository
    /// </summary>
    /// <param name="connectionString">Redis connection string, see StackExchange.Redis documentation</param>
    /// <param name="applicationName">Prefix used in cache key to avoid collisions between applications</param>
    public RedisTokenCacheRepository(string connectionString, string applicationName)
    {
        _options = connectionString;
        _applicationName = applicationName;
    }

    // options allows for more flexibility in the connection string
    private static readonly Lazy<ConfigurationOptions> ConfigOptions = new Lazy<ConfigurationOptions>(() => ConfigurationOptions.Parse(_options));
    private static readonly Lazy<ConnectionMultiplexer> Conn = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(ConfigOptions.Value));
    private static ConnectionMultiplexer _connection => Conn.Value;
    private readonly string _applicationName;
    private static string _options;

    /// <summary>
    /// Adds or updates the token in the cache
    /// </summary>
    public void AddOrUpdate(TokenCacheItem item)
    {
      if (item == null)
        throw new ArgumentNullException("item");
      var db = _connection.GetDatabase();
      TimeSpan expiry = item.Expires.ToUniversalTime() - DateTime.UtcNow;
      db.StringSet(_applicationName + item.Key, item.Token, expiry);
    }

    /// <summary>
    /// Gets a token from the cache
    /// </summary>
    public TokenCacheItem Get(string key)
    {
      var db = _connection.GetDatabase();
      var cachedValue = db.StringGet(_applicationName + key);
      return cachedValue.IsNull ? null : new TokenCacheItem { Key = key, Token = cachedValue };
    }

    /// <summary>
    /// Removes a token from the cache
    /// </summary>
    public void Remove(string key)
    {
      var db = _connection.GetDatabase();
      db.KeyDelete(_applicationName + key);
    }

    /// <summary>
    /// Release all resources associated with this object
    /// </summary>
    public void Dispose()
    {
      _connection.Dispose();
    }
  }
}
