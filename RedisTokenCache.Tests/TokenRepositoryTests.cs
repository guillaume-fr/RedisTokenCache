using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moissaing.ThinktectureIdentityModel.Redis;
using Thinktecture.IdentityModel.Web;

namespace RedisTokenCache.Tests
{
  [TestClass]
  public class TokenRepositoryTests
  {
    private Process serverProcess;
    private const string connectionString = "localhost,connectRetry=3,connectTimeout=500";

    [TestInitialize]
    public void Initialize()
    {
      serverProcess = Process.Start(@"..\..\..\packages\Redis-32.2.6.12.1\tools\redis-server.exe");
    }

    [TestCleanup]
    public void Cleanup()
    {
      try
      {
        serverProcess.Kill();
      }
      catch { }
      serverProcess.Dispose();
    }

    [TestMethod]
    public void TestAddOrUpdateThenGet()
    {
      var repository = new RedisTokenCacheRepository(connectionString, "UnitTests");

      repository.AddOrUpdate(new TokenCacheItem() { Key = "42", Expires = DateTime.UtcNow.AddMinutes(1), Token = new byte[] { 0, 1, 2 } });
      var item = repository.Get("42");

      Assert.IsNotNull(item);
      CollectionAssert.AreEqual(new byte[] { 0, 1, 2 }, item.Token);
      Assert.AreEqual("42", item.Key);
    }
    
    [TestMethod]
    public void TestUpdateExisting()
    {
      var repository = new RedisTokenCacheRepository(connectionString, "UnitTests");

      repository.AddOrUpdate(new TokenCacheItem() { Key = "42", Expires = DateTime.UtcNow.AddMinutes(1), Token = new byte[] { 0, 1, 2 } });
      var item = repository.Get("42");
      repository.AddOrUpdate(new TokenCacheItem() { Key = "42", Expires = DateTime.UtcNow.AddMinutes(1), Token = new byte[] { 3, 4, 5 } });
      var item2 = repository.Get("42");

      Assert.IsNotNull(item);
      CollectionAssert.AreEqual(new byte[] { 0, 1, 2 }, item.Token);
      Assert.AreEqual("42", item.Key);
      Assert.IsNotNull(item2);
      CollectionAssert.AreEqual(new byte[] { 3, 4, 5 }, item2.Token);
      Assert.AreEqual("42", item2.Key);
    }

    [TestMethod]
    public void TestDeleteExisting()
    {
      var repository = new RedisTokenCacheRepository(connectionString, "UnitTests");
      repository.AddOrUpdate(new TokenCacheItem() { Key = "42", Expires = DateTime.UtcNow.AddMinutes(1), Token = new byte[] { 0, 1, 2 } });
      Assert.IsNotNull(repository.Get("42"));

      repository.Remove("42");

      Assert.IsNull(repository.Get("42"));
    }
    
    [TestMethod]
    public void TestExpiration()
    {
      var repository = new RedisTokenCacheRepository(connectionString, "UnitTests");
      repository.AddOrUpdate(new TokenCacheItem() { Key = "42", Expires = DateTime.UtcNow.AddMilliseconds(200), Token = new byte[] { 0, 1, 2 } });
      Assert.IsNotNull(repository.Get("42"));

      Thread.Sleep(400);

      Assert.IsNull(repository.Get("42"));
    }
    
    [TestMethod]
    public void TestGetMissing()
    {
      var repository = new RedisTokenCacheRepository(connectionString, "UnitTests");
      
      Assert.IsNull(repository.Get("doesnt.exists"));
    }

    [TestMethod]
    public void TestDeleteMissing()
    {
      var repository = new RedisTokenCacheRepository(connectionString, "UnitTests");

      repository.Remove("doesnt.exists");

      Assert.IsNull(repository.Get("doesnt.exists"));
    }
  }
}
