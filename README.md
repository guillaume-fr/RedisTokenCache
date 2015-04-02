# RedisTokenCache
When using WIF server side token caching is almost mandatory as the token quickly weight more than 4KB which is over the cookie limit for some web browser and might be slow to upload on each request. Thinktecture provides an EntityFramework implementation to store tokens on server side. RedisTokenCache allows you to use Redis to store tokens, it's ideal in an Azure Cloud environment (WebRole, WebSite...) and provides auto cleanup on expiry.

[NuGet package available](https://www.nuget.org/packages/Moissaing.ThinktectureIdentityModel.Redis)

This library provides one class : `RedisTokenCacheRepository` that implements `ITokenCacheRepository` interface from Thinktecture.IdentityModel using Redis as backing store.

For more information about server-side token caching in Thinktecture IdentityModel : http://brockallen.com/2013/02/21/server-side-session-token-caching-in-wif-and-thinktecture-identitymodel/

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
