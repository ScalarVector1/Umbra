using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;

namespace Umbra.Content.GUI.Config
{
	internal abstract class BaseUIRepositionElement : ConfigElement
	{
		public abstract ref Vector2 modifying { get; }

		public BaseUIRepositionElement()
		{
			Width.Set(0, 1f);
			Recalculate();
			var dims = GetDimensions().ToRectangle();

			float ratio = Main.screenHeight / (float)Main.screenWidth;
			Height.Set(dims.Width * ratio + 24 + 16 * ratio, 0);
			Recalculate();
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			float ratio = Main.screenHeight / (float)Main.screenWidth;

			var dims = GetDimensions().ToRectangle();
			dims.Height = (int)(dims.Width * ratio);

			DrawBox(spriteBatch, GetDimensions().ToRectangle(), new Color(0.2f, 0.25f, 0.6f) * 0.2f);
			Utils.DrawBorderString(spriteBatch, Label, dims.TopLeft() + Vector2.One * 8, Color.White, 0.8f);
			Utils.DrawBorderString(spriteBatch, $"{Math.Round(modifying.X)}, {Math.Round(modifying.Y)}", dims.TopRight() + new Vector2(-8, 8), Color.White, 0.8f, 1f);

			Rectangle preview = dims;
			preview.Y += 24;
			preview.Inflate(-16, (int)(-16 * ratio));

			bool mouseOver = preview.Contains(Main.MouseScreen.ToPoint());

			preview.Inflate(4, 4);
			DrawBox(spriteBatch, preview, mouseOver ? Color.Orange : Color.DarkGray);
			preview.Inflate(-4, -4);

			spriteBatch.Draw(Assets.MagicPixel.Value, preview, new Color(0.2f, 0.2f, 0.2f));
			spriteBatch.Draw(Main.screenTarget, preview, Color.White * 0.25f);

			PostDraw(spriteBatch, preview);
		}

		public virtual void PostDraw(SpriteBatch spriteBatch, Rectangle preview) { }

		public static void DrawBox(SpriteBatch sb, Rectangle target, Color color)
		{
			Texture2D tex = Assets.GUI.WhiteBox.Value;

			if (target.Width < 12 || target.Height < 12)
				return;

			if (target.Width < 32 || target.Height < 32)
			{
				int min = target.Width > target.Height ? target.Height : target.Width;
				color *= (min - 12) / 20f;
			}

			var sourceCorner = new Rectangle(0, 0, 6, 6);
			var sourceEdge = new Rectangle(6, 0, 4, 6);
			var sourceCenter = new Rectangle(6, 6, 4, 4);

			Rectangle inner = target;
			inner.Inflate(-4, -4);

			sb.Draw(tex, inner, sourceCenter, color);

			sb.Draw(tex, new Rectangle(target.X + 6, target.Y, target.Width - 12, 6), sourceEdge, color, 0, Vector2.Zero, 0, 0);
			sb.Draw(tex, new Rectangle(target.X, target.Y - 6 + target.Height, target.Height - 12, 6), sourceEdge, color, -(float)Math.PI * 0.5f, Vector2.Zero, 0, 0);
			sb.Draw(tex, new Rectangle(target.X - 6 + target.Width, target.Y + target.Height, target.Width - 12, 6), sourceEdge, color, (float)Math.PI, Vector2.Zero, 0, 0);
			sb.Draw(tex, new Rectangle(target.X + target.Width, target.Y + 6, target.Height - 12, 6), sourceEdge, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);

			sb.Draw(tex, new Rectangle(target.X, target.Y, 6, 6), sourceCorner, color, 0, Vector2.Zero, 0, 0);
			sb.Draw(tex, new Rectangle(target.X + target.Width, target.Y, 6, 6), sourceCorner, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);
			sb.Draw(tex, new Rectangle(target.X + target.Width, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI, Vector2.Zero, 0, 0);
			sb.Draw(tex, new Rectangle(target.X, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI * 1.5f, Vector2.Zero, 0, 0);
		}

		public override void Update(GameTime gameTime)
		{
			float ratio = Main.screenHeight / (float)Main.screenWidth;

			Width.Set(0, 1f);
			Recalculate();
			var dims = GetDimensions().ToRectangle();

			Height.Set(dims.Width * ratio + 24 + 16 * ratio, 0);
			Recalculate();

			Parent.Height.Set(Height.Pixels + 6, 0);
			Parent.Recalculate();

			dims = GetDimensions().ToRectangle();

			Rectangle preview = dims;
			preview.Inflate(-16, (int)(-16 * ratio / 2));
			preview.Y += 32;
			preview.Height -= 32;

			preview.Height = (int)(preview.Width * ratio);

			if (preview.Contains(Main.MouseScreen.ToPoint()) && Main.mouseLeft)
			{
				Vector2 relativePos = Main.MouseScreen - preview.TopLeft();
				modifying = relativePos / preview.Size() * Main.ScreenSize.ToVector2();
				SetObject(modifying);
			}
		}
	}
}