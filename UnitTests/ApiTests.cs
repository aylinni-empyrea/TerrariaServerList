using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerrariaServerList;

namespace UnitTests
{
  [TestClass]
  public class ApiTests
  {
    private static readonly ServerManager _manager = new ServerManager("<your key>");

    [TestMethod]
    public async Task GetsServerInfo()
    {
      var res = await _manager.GetServerAsync();

      Assert.IsInstanceOfType(res, typeof(Server));
    }

    [TestMethod]
    public async Task GetsVotes()
    {
      var res = await _manager.GetVotesAsync();

      Assert.IsInstanceOfType(res, typeof(ServerVotes));

      foreach (var vote in res)
        Assert.IsInstanceOfType(vote, typeof(Vote));
    }

    [TestMethod]
    public async Task GetsVoters()
    {
      var res = await _manager.GetVotersAsync();

      Assert.IsInstanceOfType(res, typeof(ServerVoters));

      foreach (var voter in res)
        Assert.IsInstanceOfType(voter, typeof(ServerVoters.Voter));
    }

    [TestMethod]
    public async Task ClaimsVoteByUsername()
    {
      var res = await _manager.ClaimAsync("Ruby");

      Assert.IsInstanceOfType(res, typeof(bool));
    }

    [TestMethod]
    public async Task ChecksClaimedVote()
    {
      var res = await _manager.CheckClaimedAsync("Ruby");

      Assert.IsInstanceOfType(res, typeof(bool));
    }
  }
}