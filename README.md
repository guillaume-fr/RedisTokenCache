# RedisTokenCache
Redis token cache repository for Thinktecture identity : `RedisTokenCacheRepository` class that implements `ITokenCacheRepository` interface from Thinktecture.IdentityModel using Redis as backing store. 
This package is ideal for your Azure WebSites and WebRoles.

For more information about service-side token caching in Thinktecture IdentityModel : http://brockallen.com/2013/02/21/server-side-session-token-caching-in-wif-and-thinktecture-identitymodel/

## Usage (Global.asax)

```
public override void Init()
{
    PassiveModuleConfiguration.CacheSessionsOnServer();
}

protected void Application_Start()
{
    PassiveSessionConfiguration.ConfigureSessionCache(new RedisTokenCacheRepository(connectionString:"localhost", applicationName:"test"));
}
```
