using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TerrariaServerList.Extensions;

namespace TerrariaServerList
{
  public class ServerManager : IDisposable
  {
    private const string _base = "https://terraria-servers.com/api/";
    private readonly string _key;

    /// <summary>
    /// Creates a new <see cref="ServerManager"/> object for server information and vote management.
    /// </summary>
    /// <param name="key">TerrariaServerList API key.</param>
    public ServerManager(string key)
    {
      if (UriExtensions._client == null)
      {
        UriExtensions._client = new HttpClient
        {
          DefaultRequestHeaders = { Accept = { new MediaTypeWithQualityHeaderValue("application/json") } }
        };
      }

      if (string.IsNullOrWhiteSpace(key))
        throw new ArgumentException("Key cannot be null or empty.", nameof(key));

      _key = key;
    }

    /// <summary>
    /// Returns a <see cref="Server"/> object about the given server.
    /// </summary>
    /// <returns>A <see cref="Server"/> object about the given server.</returns>
    public async Task<Server> GetServerAsync()
      => await (_base + "?object=servers&element=detail&key=" + _key).GetJsonAsync<Server>()
        .ConfigureAwait(false);

    /// <summary>
    /// Returns a <see cref="ServerVotes"/> object, which contains the votes
    /// for the server in a <see cref="ServerVoters"/> object.
    /// </summary>
    /// <param name="amount">Amount of votes to retrieve. Maximum amount of votes retrievable is 500.</param>
    /// <returns>A <see cref="ServerVotes"/> object about the given server.</returns>
    public async Task<ServerVotes> GetVotesAsync(int amount = 100)
      => await (_base + $"?object=servers&element=votes&limit={amount}&key={_key}").GetJsonAsync<ServerVotes>()
        .ConfigureAwait(false);

    /// <summary>
    /// Returns the top voters for the server.
    /// </summary>
    /// <param name="amount">Amount of votes to retrieve. Maximum amount of votes retrievable is 500.</param>
    /// <returns>A <see cref="ServerVotes"/> object about the given server.</returns>
    public async Task<ServerVoters> GetVotersAsync(int amount = 100)
      => await (_base + $"?object=servers&element=voters&limit={amount}&key={_key}").GetJsonAsync<ServerVoters>()
        .ConfigureAwait(false);

    /// <summary>
    /// Checks if the given nickname has voted in the current day.
    /// </summary>
    /// <param name="nickname">Nickname to look up.</param>
    /// <returns>null if the user hasn't voted, false if the user hasn't claimed the vote, true otherwise</returns>
    public async Task<bool?> CheckClaimedAsync(string nickname)
    {
      if (string.IsNullOrEmpty(nickname))
        throw new ArgumentNullException();

      var res = await (_base + "?object=votes&element=claim&username=" +
                       WebUtility.UrlEncode(nickname) + "&key=" + _key)
        .GetStringAsync()
        .ConfigureAwait(false);

      switch (int.Parse(res))
      {
        case 0:
        default:
          return null;
        case 1:
          return false;
        case 2:
          return true;
      }
    }

    /// <summary>
    /// Checks if the given nickname has voted in the current day.
    /// </summary>
    /// <param name="steamid">Steam ID to look up.</param>
    /// <returns>null if the user hasn't voted, false if the user hasn't claimed the vote, true otherwise</returns>
    public async Task<bool?> CheckClaimedAsync(ulong steamid)
    {
      var res = await (_base + "?object=votes&element=claim&steamid=" +
                       steamid + "&key=" + _key)
        .GetStringAsync()
        .ConfigureAwait(false);

      switch (int.Parse(res))
      {
        case 0:
        default:
          return null;
        case 1:
          return false;
        case 2:
          return true;
      }
    }

    /// <summary>
    /// Claims the vote for the current day for the given Steam ID.
    /// </summary>
    /// <param name="steamid">Steam ID to claim the vote for.</param>
    /// <returns>False if the vote isn't claimed or not found, true otherwise</returns>
    public async Task<bool> ClaimAsync(ulong steamid)
    {
      var resp =
        await new Uri(_base + $"?object=votes&element=claim&key={_key}&steamid={steamid}").PostStringAsync("")
          .ConfigureAwait(false);

      return await resp.Content.ReadAsStringAsync() == "1";
    }

    /// <summary>
    /// Claims the vote for the current day for the given nickname.
    /// </summary>
    /// <param name="username">Nickname to claim the vote for.</param>
    /// <returns>False if the vote isn't claimed or not found, true otherwise</returns>
    public async Task<bool> ClaimAsync(string username)
    {
      var resp =
        await new Uri(_base + $"?object=votes&element=claim&key={_key}&username={username}").PostStringAsync("")
          .ConfigureAwait(false);

      return await resp.Content.ReadAsStringAsync() == "1";
    }

    public void Dispose()
    {
      UriExtensions._client.Dispose();
      UriExtensions._client = null;
    }
  }
}