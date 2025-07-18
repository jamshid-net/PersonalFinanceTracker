using StackExchange.Redis;

namespace FiTrack.Application.Interfaces.InfrastructureAdapters;
public interface IRedisServer
{
    IServer Server { get; }
}
