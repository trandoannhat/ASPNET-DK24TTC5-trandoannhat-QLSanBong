using System.Text.Json.Serialization;

namespace QLSanBong.Domain.Enums;

[Flags]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRole
{
    None = 0,
    Admin = 1,
    Manager = 2,
    Staff = 4,
    Customer = 8
}