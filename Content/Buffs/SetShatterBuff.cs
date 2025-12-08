using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbra.Content.Buffs
{
	internal class SetShatterBuff : ModBuff
	{
		public override string Texture => "Umbra/Assets/Buffs/SetShatterBuff";

		public override void Load()
		{
			On_Player.UpdateArmorSets += DontUpdateWhileShattered;
		}

		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
		}

		private void DontUpdateWhileShattered(On_Player.orig_UpdateArmorSets orig, Player self, int i)
		{
			if (self.HasBuff<SetShatterBuff>())
				return;

			orig(self, i);
		}
	}
}
