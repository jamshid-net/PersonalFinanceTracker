using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Shared.Constants;
public class AuthOptions
{
    public const int ExpireMinutes = 120; 
    public const int ExpireMinutesRefresh = 360; 
    public const string Issuer = "AuthServer"; 
    public const string Audience = "AuthClient";
    public const int MaxDeviceCount = 3;
    const string SecretKey = "F3uuiIIKJnkj78843Fh1KJH7DMMTyv12hdjsUY78N";
    public static SymmetricSecurityKey GetSymmetricSecurityKey() => new(Encoding.UTF8.GetBytes(SecretKey));
}
