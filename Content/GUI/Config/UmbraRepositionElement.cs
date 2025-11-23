using Umbra.Content.Configs;

namespace Umbra.Content.GUI.Config
{
	internal class UmbraRepositionElement : BaseUIRepositionElement
	{
		public override ref Vector2 modifying => ref ModContent.GetInstance<UIConfig>().UmbraIconPosition;

		public override void PostDraw(SpriteBatch spriteBatch, Rectangle preview)
		{
			bool mouseOver = preview.Contains(Main.MouseScreen.ToPoint());
			Color flashColor = !mouseOver ? Color.White : Color.Lerp(Color.Orange, Color.White, 0.5f + (float)Math.Sin(Main.timeForVisualEffects * 0.2f) * 0.5f);

			Texture2D tex = Assets.GUI.BarEmpty.Value;
			spriteBatch.Draw(tex, preview.TopLeft() + modifying / Main.ScreenSize.ToVector2() * preview.Size(), null, flashColor, 0, tex.Size() / 2f, preview.Width / (float)Main.screenWidth, 0, 0);

			if (mouseOver)
			{
				if (!Main.gameMenu)
				{
					Main.playerInventory = true;
					typeof(Main).GetMethod("DrawInventory", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(Main.instance, new object[] { });
				}

				Main.graphics.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, (int)(Main.screenWidth * Main.UIScale), (int)(Main.screenHeight * Main.UIScale));
				spriteBatch.Draw(tex, modifying, null, flashColor, 0, Vector2.Zero, 1f, 0, 0);
			}
		}
	}
}