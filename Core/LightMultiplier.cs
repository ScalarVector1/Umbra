namespace Umbra.Core
{
	internal class LightMultiplier : ModSystem
	{
		public static float lightMultiplier;

		public override void ModifyLightingBrightness(ref float scale)
		{
			scale += lightMultiplier;
			lightMultiplier = 0f;
		}

		public override void PostUpdateEverything()
		{
			//lightMultiplier = 0f;
		}
	}
}
