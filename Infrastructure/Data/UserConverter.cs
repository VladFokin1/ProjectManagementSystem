using ProjectManagementSystem.Core.Entities;
using ProjectManagementSystem.Core.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProjectManagementSystem.Infrastructure.Data
{
    internal class UserConverter : JsonConverter<User>
    {
        public override User? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                var root = doc.RootElement;
                // Ищем свойство в camelCase ("role") или PascalCase ("Role")
                string[] rolePropertyNames = { "role", "Role" };
                JsonElement roleElement = default;
                bool roleFound = false;

                foreach (var name in rolePropertyNames)
                {
                    if (root.TryGetProperty(name, out roleElement))
                    {
                        roleFound = true;
                        break;
                    }
                }

                if (!roleFound)
                {
                    throw new JsonException("Missing 'role' property in user object");
                }

                var role = roleElement.Deserialize<Role>(options);

                return role switch
                {
                    Role.Manager => root.Deserialize<Manager>(options),
                    Role.Employee => root.Deserialize<Employee>(options),
                    _ => throw new JsonException($"Unknown role: {role}")
                };
            }
        }

        public override void Write(Utf8JsonWriter writer, User value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, options);
        }
    }
}
