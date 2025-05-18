using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

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

			// Find a subclass of Passive that matches the given name
			Type passiveType = Assembly.GetExecutingAssembly()
				.GetTypes()
				.FirstOrDefault(t => !t.IsAbstract && typeof(Passive).IsAssignableFrom(t) && t.Name == typeName);

			if (passiveType == null)
				throw new JsonException($"Unknown Passive subclass: '{typeName}'.");

			// Deserialize into the resolved subclass
			var passive = (Passive)JsonSerializer.Deserialize(root.GetRawText(), passiveType, options);
			return passive;
		}

		public override void Write(Utf8JsonWriter writer, Passive value, JsonSerializerOptions options)
		{
			Type type = value.GetType();
			JsonElement json = JsonSerializer.SerializeToElement(value, type, options);

			// Manually add the "Type" discriminator
			using var doc = JsonDocument.Parse(json.GetRawText());
			writer.WriteStartObject();
			writer.WriteString("type", type.Name);

			foreach (JsonProperty property in doc.RootElement.EnumerateObject())
			{
				property.WriteTo(writer);
			}

			writer.WriteEndObject();
		}
	}
}
