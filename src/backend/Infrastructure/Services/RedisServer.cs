using FiTrack.Application.Interfaces.InfrastructureAdapters;
using StackExchange.Redis;

namespace FiTrack.Infrastructure.Services;
public class RedisServer(IServer server) : IRedisServer
{
    public IServer Server => server;
}
