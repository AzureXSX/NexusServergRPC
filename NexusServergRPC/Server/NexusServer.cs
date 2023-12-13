using Grpc.Core;
using Microsoft.AspNetCore.Hosting.Server;
using NexusServergRPC.Auth;
using NexusServergRPC.Entity;
using NexusServergRPC.Models;
using NexusServergRPC.RequestProcessing;
using System.Collections.Generic;
namespace NexusServergRPC.Server;

public class Nexus_Server
{
    private const int Port = 50051;

    public List<object> users = new List<object>();

    private List<NexusToken> tokens = new List<NexusToken>();

    public List<NexusToken> Tokens { get { return tokens; } }

    private readonly NexusTokenIssuer _issuer;


    public void AddToken(NexusToken token)
    {
        tokens.Add(token);
    }

    public void RemoveToken(string token)
    {
        int index = tokens.FindIndex(x => x.Token == token);

        tokens.RemoveAt(index);

        Console.WriteLine("TOKEN EXPIRED " + tokens.Count);
    }

    public Nexus_Server(NexusTokenIssuer issuer)
    {
        _issuer = issuer;
    }


}