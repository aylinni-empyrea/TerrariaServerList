using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerrariaServerList;
using TerrariaServerList.Extensions;

namespace UnitTests
{
  [TestClass]
  public class HttpExtensionTests
  {
    private static Uri _base = new Uri("https://terraria-servers.com/api/");
    private const string _key = "<your key>";

    [TestMethod]
    public async Task GetsJson()
    {
      var point = new UriBuilder(_base)
      {
        Query = $"object=servers&element=detail&key={_key}"
      };

      var ret = await point.Uri.GetJsonAsync<Server>();

      Assert.IsInstanceOfType(ret, typeof(Server));
    }

    [TestMethod]
    public async Task PostsJson()
    {
      const string _point = "https://requestb.in/11fjgh31";

      await new Uri(_point).PostJsonAsync(new {foo = "bar", baz = "bat"});
    }
  }
}