using System.Collections.Generic;
using Terraria.UI;
using Terraria.UI.Chat;
using Umbra.Core.Loaders.UILoading;

namespace Umbra.Content.GUI
{
	internal class Notification : SmartUIState
	{
		private static TextSnippet[] title;
		private static TextSnippet[] body;

		public static bool visible;

		public override bool Visible => visible;

		public override int InsertionIndex(List<GameInterfaceLayer> layers)
		{
			return layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text")) + 1;
		}

		public static void DisplayNotification(string title, string body)
		{
			Notification.title = ChatManager.ParseMessage(title, Color.White).ToArray();
			Notification.body = ChatManager.ParseMessage(body, Color.White).ToArray();
			visible = true;
		}

		public override void SafeUpdate(GameTime gameTime)
		{
			ReLogic.Graphics.DynamicSpriteFont font = Terraria.GameContent.FontAssets.MouseText.Value;

			float nameWidth = ChatManager.GetStringSize(font, title, Vector2.One).X;
			float tipWidth = ChatManager.GetStringSize(font, body, Vector2.One * 0.8f, 300).X;

			float width = Math.Max(nameWidth, tipWidth);
			float height = ChatManager.GetStringSize(font, body, Vector2.One * 0.8f, 300).Y + 36;

			var pos = new Vector2(Main.screenWidth / 2 - width / 2, Main.screenHeight / 2 - height / 2);

			var box = new Rectangle((int)pos.X, (int)pos.Y, (int)width, (int)height);

			if (Main.mouseLeft && box.Contains(Main.MouseScreen.ToPoint()))
				visible = false;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (title is null || title.Length <= 0)
				return;

			ReLogic.Graphics.DynamicSpriteFont font = Terraria.GameContent.FontAssets.MouseText.Value;

			float nameWidth = ChatManager.GetStringSize(font, title, Vector2.One).X;
			float tipWidth = ChatManager.GetStringSize(font, body, Vector2.One * 0.8f, 300).X;

			float width = Math.Max(nameWidth, tipWidth);
			float height = ChatManager.GetStringSize(font, body, Vector2.One * 0.8f, 300).Y + 36;

			var pos = new Vector2(Main.screenWidth / 2 - width / 2, Main.screenHeight / 2 - height / 2);

			Utils.DrawInvBG(Main.spriteBatch, new Rectangle((int)pos.X - 10, (int)pos.Y - 10, (int)width + 20, (int)height + 20), new Color(20, 20, 55) * 0.925f);

			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, title, pos, 0, Vector2.Zero, Vector2.One, out int hov);
			pos.Y += ChatManager.GetStringSize(font, title, Vector2.One).Y + 4;

			if (body != null && body.Length > 0)
				ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, body, pos, 0, Vector2.Zero, Vector2.One * 0.8f, out int hov2, 300);
		}
	}
}
