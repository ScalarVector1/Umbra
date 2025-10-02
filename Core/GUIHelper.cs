﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbra.Core
{
    internal class GUIHelper
    {
        public static void DrawBox(SpriteBatch spriteBatch, Rectangle target, Color color)
        {
            Texture2D tex = Assets.GUI.Box.Value;

            if (color == default)
                color = new Color(49, 84, 141) * 0.9f;

            var sourceCorner = new Rectangle(0, 0, 6, 6);
            var sourceEdge = new Rectangle(6, 0, 4, 6);
            var sourceCenter = new Rectangle(6, 6, 4, 4);

            Rectangle inner = target;
            inner.Inflate(-6, -6);

            spriteBatch.Draw(tex, inner, sourceCenter, color);

            spriteBatch.Draw(tex, new Rectangle(target.X + 6, target.Y, target.Width - 12, 6), sourceEdge, color, 0, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y - 6 + target.Height, target.Height - 12, 6), sourceEdge, color, -(float)Math.PI * 0.5f, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X - 6 + target.Width, target.Y + target.Height, target.Width - 12, 6), sourceEdge, color, (float)Math.PI, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y + 6, target.Height - 12, 6), sourceEdge, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);

            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y, 6, 6), sourceCorner, color, 0, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y, 6, 6), sourceCorner, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI * 1.5f, Vector2.Zero, 0, 0);
        }

        public static void DrawBoxFancy(SpriteBatch spriteBatch, Rectangle target, Color color)
        {
            DrawBox(spriteBatch, target, color);
        }

        public static void DrawBoxSmall(SpriteBatch spriteBatch, Rectangle target, Color color)
        {
            DrawBox(spriteBatch, target, color);
        }

        public static void DrawOutline(SpriteBatch spriteBatch, Rectangle target, Color color)
        {
            Texture2D tex = Assets.GUI.Box.Value;

            if (color == default)
                color = new Color(49, 84, 141) * 0.9f;

            var sourceCorner = new Rectangle(0, 0, 6, 6);
            var sourceEdge = new Rectangle(6, 0, 4, 6);
            var sourceCenter = new Rectangle(6, 6, 4, 4);

            spriteBatch.Draw(tex, new Rectangle(target.X + 6, target.Y, target.Width - 12, 6), sourceEdge, color, 0, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y - 6 + target.Height, target.Height - 12, 6), sourceEdge, color, -(float)Math.PI * 0.5f, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X - 6 + target.Width, target.Y + target.Height, target.Width - 12, 6), sourceEdge, color, (float)Math.PI, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y + 6, target.Height - 12, 6), sourceEdge, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);

            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y, 6, 6), sourceCorner, color, 0, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y, 6, 6), sourceCorner, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI * 1.5f, Vector2.Zero, 0, 0);
        }

        public static void DrawFancyFrame(SpriteBatch spriteBatch, Rectangle target)
        {
            Texture2D tex = Assets.GUI.Frame.Value;

            Color color = Color.White;

            spriteBatch.Draw(tex, new Rectangle(target.X + 10, target.Y, target.Width - 32, 6), new Rectangle(10, 6, 2, 6), color, 0, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y + 10, 6, target.Height - 24), new Rectangle(0, 16, 6, 2), color, 0, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + 32, target.Y + target.Height - 6, target.Width - 42, 6), new Rectangle(42, tex.Height - 12, 2, 6), color, 0, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width - 6, target.Y + 10, 6, target.Height - 20), new Rectangle(tex.Width - 6, 16, 6, 2), color, 0, Vector2.Zero, 0, 0);

            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y, 10, 10), new Rectangle(0, 6, 10, 10), color, 0, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width - 32, target.Y - 6, 32, 20), new Rectangle(tex.Width - 32, 0, 32, 20), color, 0, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width - 10, target.Y + target.Height - 10, 10, 10), new Rectangle(tex.Width - 10, tex.Height - 16, 10, 10), color, 0, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y + target.Height - 14, 32, 20), new Rectangle(0, tex.Height - 20, 32, 20), color, 0, Vector2.Zero, 0, 0);
        }
    }
}
