using System.ComponentModel.DataAnnotations.Schema;
using FiTrack.Domain.Common;

namespace FiTrack.Domain.ValueObjects;
[ComplexType]
public class MultiLanguage : ValueObject
{
    public required string Uz { get; set; } = string.Empty;
    public required string Ru { get; set; } = string.Empty;
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Uz;
        yield return Ru;
    }
}
