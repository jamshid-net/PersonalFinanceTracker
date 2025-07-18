using FiTrack.Domain.Common;
using FiTrack.Domain.Entities.Auth;

namespace FiTrack.Domain.Events;
public class AuthUserCreatedEvent(AuthUser authUser) : BaseEvent
{
    public AuthUser AuthUser => authUser;
}
