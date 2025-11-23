using System.Text.Json;
using System.Text.Json.Serialization;
using Umbra.Content.Passives;

namespace Umbra.Core.PassiveTreeSystem
{
	internal class PassiveConverter : JsonConverter<Passive>
	{
		public override Passive Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			using var jsonDoc = JsonDocument.ParseValue(ref reader);
			JsonElement root = jsonDoc.RootElement;

			if (!root.TryGetProperty("type", out JsonElement typeProp))
				throw new JsonException("Missing Type discriminator for Passive.");

			string typeName = typeProp.GetString();
			if (string.IsNullOrWhiteSpace(typeName))
				throw new JsonException("Type discriminator cannot be null or empty.");

			// backwards compat with older versions, assume umbra as mod
			string[] split = typeName.Split('/');

			if (split.Length == 1)
				typeName = $"Umbra/{typeName}";

			// Deserialize into the resolved subclass
			if (ModContent.TryFind<Passive>(typeName, out Passive proto))
			{
				Passive passive = proto.Clone();
				passive.ID = root.GetProperty("id").GetInt32();
				passive.Cost = root.GetProperty("cost").GetInt32();
				passive.X = root.GetProperty("x").GetInt32();
				passive.Y = root.GetProperty("y").GetInt32();
				return passive;
			}
			else
			{
				var passive = ModContent.GetInstance<UnloadedPassive>().Clone() as UnloadedPassive;
				passive.savedType = typeName;
				passive.ID = root.GetProperty("id").GetInt32();
				passive.Cost = root.GetProperty("cost").GetInt32();
				passive.X = root.GetProperty("x").GetInt32();
				passive.Y = root.GetProperty("y").GetInt32();
				return passive;
			}
		}

		public override void Write(Utf8JsonWriter writer, Passive value, JsonSerializerOptions options)
		{
			string typeToWrite = value is UnloadedPassive unloaded ? unloaded.savedType : value.FullName;

			writer.WriteStartObject();
			writer.WriteString("type", typeToWrite);
			writer.WriteNumber("id", value.ID);
			writer.WriteNumber("cost", value.Cost);
			writer.WriteNumber("x", value.X);
			writer.WriteNumber("y", value.Y);
			writer.WriteEndObject();
		}
	}
}
