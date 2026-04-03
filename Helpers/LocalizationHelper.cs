using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Helpers
{
	internal class LocalizationHelper
	{
		public static string GetCoinString(int amount, Player player)
		{
			string coinString = "";

			bool canAfford = player.CanAfford(amount);

			int remainingCost = amount;

			if (remainingCost >= Item.platinum)
			{
				int platCount = remainingCost / Item.platinum;
				coinString += $"[c/{(canAfford ? "FFDDFF" : "FF0000")}:{platCount}][i:74]";
				remainingCost -= platCount * Item.platinum;
			}

			if (remainingCost >= Item.gold)
			{
				int goldCount = remainingCost / Item.gold;
				coinString += $"[c/{(canAfford ? "FFDD22" : "FF0000")}:{goldCount}][i:73]";
				remainingCost -= goldCount * Item.gold;
			}

			if (remainingCost >= Item.silver)
			{
				int silverCount = remainingCost / Item.silver;
				coinString += $"[c/{(canAfford ? "BBBBDD" : "FF0000")}:{silverCount}][i:72]";
				remainingCost -= silverCount * Item.silver;
			}

			if (remainingCost >= Item.copper)
			{
				int copperCount = remainingCost / Item.copper;
				coinString += $"[c/{(canAfford ? "F1885E" : "FF0000")}:{copperCount}][i:71]";
				remainingCost -= copperCount * Item.copper;
			}

			return coinString;
		}
	}
}
