using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;

namespace TerrariaServerList
{
  public class Server
  {
    [JsonConstructor]
    internal Server(int id, string name, string address, int port, bool @private, bool password,
      string location, string hostname, string map, bool is_online, int players, int maxPlayers, Version version,
      string platform, int uptime, int score, int rank, int votes, int favorited, int comments, string url,
      DateTime last_check, DateTime last_online)
    {
      ID = id;
      Name = name;
      Address = address;
      Port = port;
      Private = @private;
      PasswordProtected = password;
      Location = location;
      Hostname = hostname;
      Map = map;
      Online = is_online;
      Players = players;
      MaxPlayers = maxPlayers;
      Version = version;
      Platform = platform;
      Uptime = uptime;
      Score = score;
      Rank = rank;
      Votes = votes;
      Favorited = favorited;
      Comments = comments;
      Url = url;
      LastCheck = last_check;
      LastOnline = last_online;
    }

    /// <summary>
    /// Server ID: part of the URL after /server/.
    /// </summary>
    [JsonProperty("id")]
    public int ID { get; }

    /// <summary>
    /// Name of the server.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// IP or domain name of the server.
    /// </summary>
    public string Address { get; }

    /// <summary>
    /// Port of the server.
    /// </summary>
    public int Port { get; }

    /// <summary>
    /// True if the server is unlisted(private).
    /// </summary>
    [JsonConverter(typeof(TruthyJsonConverter))]
    public bool Private { get; }

    /// <summary>
    /// True if the server needs a password on join.
    /// </summary>
    [JsonProperty("Password")]
    [JsonConverter(typeof(TruthyJsonConverter))]
    public bool PasswordProtected { get; }

    /// <summary>
    /// Geographical location of the server.
    /// </summary>
    public string Location { get; }

    /// <summary>
    /// Name of the server host.
    /// </summary>
    public string Hostname { get; }

    /// <summary>
    /// Name of the currently hosted map.
    /// </summary>
    public string Map { get; }

    /// <summary>
    /// True if the server is reachable.
    /// </summary>
    [JsonProperty("is_online")]
    [JsonConverter(typeof(TruthyJsonConverter))]
    public bool Online { get; }

    /// <summary>
    /// Number of the currently online players.
    /// </summary>
    public int Players { get; }

    /// <summary>
    /// Maximum amount of players the server can host.
    /// </summary>
    public int MaxPlayers { get; }

    /// <summary>
    /// Terraria version of the server.
    /// </summary>
    public Version Version { get; }

    /// <summary>
    /// The OS the server is running on.
    /// </summary>
    public string Platform { get; }

    /// <summary>
    /// Uptime percentage of the server.
    /// </summary>
    public int Uptime { get; }

    /// <summary>
    /// Score of the server on the server list.
    /// </summary>
    public int Score { get; }

    /// <summary>
    /// Rank of the server on the server list.
    /// </summary>
    public int Rank { get; }

    /// <summary>
    /// Total votes for the server in the current month.
    /// </summary>
    public int Votes { get; }

    /// <summary>
    /// Total favorites for the server all time.
    /// </summary>
    public int Favorited { get; }

    /// <summary>
    /// Total amount of comments.
    /// </summary>
    public int Comments { get; }

    /// <summary>
    /// URL for the server.
    /// </summary>
    public string Url { get; }

    /// <summary>
    /// Last check time of server online status.
    /// </summary>
    [JsonProperty("last_check")]
    [JsonConverter(typeof(WeirdDateTimeConverter))]
    public DateTime LastCheck { get; }

    /// <summary>
    /// Last time the server was seen online.
    /// </summary>
    [JsonProperty("last_online")]
    [JsonConverter(typeof(WeirdDateTimeConverter))]
    public DateTime LastOnline { get; }
  }

  public class Vote
  {
    [JsonConstructor]
    internal Vote(DateTime timestamp, string nickname, ulong? steamid, bool claimed)
    {
      Date = timestamp;
      Nickname = nickname;
      SteamID = steamid;
      Claimed = claimed;
    }

    /// <summary>
    /// Date of the vote.
    /// </summary>
    [JsonProperty("timestamp")]
    [JsonConverter(typeof(EpochDateTimeConverter))]
    public DateTime Date { get; }

    /// <summary>
    /// Nickname of the voter.
    /// </summary>
    public string Nickname { get; }

    /// <summary>
    /// Steam ID of the voter.
    /// </summary>
    public ulong? SteamID { get; }

    /// <summary>
    /// True if claimed.
    /// </summary>
    [JsonConverter(typeof(TruthyJsonConverter))]
    public bool Claimed { get; }
  }

  [JsonObject]
  public class ServerVotes : IReadOnlyList<Vote>
  {
    /// <summary>
    /// Name of the server.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Address of the server.
    /// </summary>
    public string Address { get; }

    /// <summary>
    /// Port of the server.
    /// </summary>
    public int Port { get; }

    /// <summary>
    /// The starting period of the votes.
    /// </summary>
    // [JsonConverter(typeof(MonthYearDateTimeConverter))]
    public DateTime VotePeriod { get; }

    private Vote[] Votes { get; }

    [JsonConstructor]
    internal ServerVotes(string name, string address, int port, string month, Vote[] votes)
    {
      Name = name;
      Address = address;
      Port = port;
      VotePeriod = DateTime.ParseExact(month, "yyyyMM", CultureInfo.InvariantCulture);
      Votes = votes;
    }

    public Vote this[int index] => Votes[index];

    public int Count => Votes.Count(vote => vote != null);

    public IEnumerator<Vote> GetEnumerator() => ((IEnumerable<Vote>) Votes).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
  }

  [JsonObject]
  public class ServerVoters : IEnumerable<ServerVoters.Voter>
  {
    public class Voter
    {
      public Voter(string nickname, int votes)
      {
        Nickname = nickname;
        Count = votes;
      }

      /// <summary>
      /// Nickname of the voter.
      /// </summary>
      public string Nickname { get; }

      /// <summary>
      /// Total votes of the voter in the current month.
      /// </summary>
      public int Count { get; }

      public override string ToString() => Nickname + ": " + Count;
    }

    /// <summary>
    /// Name of the server.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Address of the server.
    /// </summary>
    public string Address { get; }

    /// <summary>
    /// Port of the server.
    /// </summary>
    public int Port { get; }

    /// <summary>
    /// The starting period of the votes.
    /// </summary>
    // [JsonConverter(typeof(MonthYearDateTimeConverter))]
    public DateTime VotePeriod { get; }

    private Voter[] Voters { get; }

    [JsonConstructor]
    internal ServerVoters(string name, string address, int port, string month, Voter[] voters)
    {
      Name = name;
      Address = address;
      Port = port;
      VotePeriod = DateTime.ParseExact(month, "yyyyMM", CultureInfo.InvariantCulture);
      Voters = voters;
    }

    public Voter this[int index] => Voters[index];

    public int Count => Voters.Count(vote => vote != null);

    public IEnumerator<Voter> GetEnumerator() => ((IEnumerable<Voter>) Voters).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
  }
}