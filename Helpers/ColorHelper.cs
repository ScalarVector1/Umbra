using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbra.Helpers
{
	internal static class ColorHelper
	{
		public static Color FromHSV(float h, float s, float v)
		{
			if (s <= 0f)
			{
				int vi = (int)(v * 255f);
				return new Color(vi, vi, vi);
			}

			float scaled = h * 6f;
			int sector = (int)scaled;
			float frac = scaled - sector;

			float p = v * (1f - s);
			float q = v * (1f - s * frac);
			float t = v * (1f - s * (1f - frac));

			float r, g, b;

			switch (sector)
			{
				case 0: r = v; g = t; b = p; break;
				case 1: r = q; g = v; b = p; break;
				case 2: r = p; g = v; b = t; break;
				case 3: r = p; g = q; b = v; break;
				case 4: r = t; g = p; b = v; break;
				default: r = v; g = p; b = q; break;
			}

			return new Color(
				(int)(r * 255f),
				(int)(g * 255f),
				(int)(b * 255f)
			);
		}
	}
}
