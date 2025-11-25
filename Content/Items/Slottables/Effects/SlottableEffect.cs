using Microsoft.Xna.Framework.Graphics;
using Stubble.Core.Classes;
using System.Linq;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Items.Slottables.Effects
{
	public abstract class SlottableEffect : ModType
	{
		public int tier;

		/// <summary>
		/// Localization key for this effects tooltip
		/// </summary>
		public string TooltipKey => $"Mods.{Mod?.Name ?? "Unknown"}.UmbraSlottableEffects.{GetType().Name}.Tooltip";

		/// <summary>
		/// The tooltip for this effect. Override this if you need to format it, the localization key is in TooltipKey
		/// </summary>
		public virtual string Tooltip => Language.GetOrRegister(TooltipKey).Value;

		public sealed override void Register()
		{
			ModTypeLookup<SlottableEffect>.Register(this);
		}

		public sealed override void SetupContent()
		{
			_ = Tooltip;
		}

		public TagCompound SaveData()
		{
			TagCompound tag = new()
			{
				["type"] = FullName,
				["tier"] = tier
			};

			return tag;
		}

		public static SlottableEffect FromData(TagCompound tag)
		{
			string type = tag.GetString("type");
			if (ModContent.TryFind<SlottableEffect>(type, out var effect))
			{
				effect.tier = tag.GetInt("tier");
				return effect;
			}

			return null;
		}

		public static SlottableEffect FromNet(string type, int tier)
		{
			if (ModContent.TryFind<SlottableEffect>(type, out var effect))
			{
				effect.tier = tier;
				return effect;
			}

			return null;
		}

		/// <summary>
		/// If there are any slottables with this effect currently in the tree
		/// </summary>
		/// <returns></returns>
		public bool AnyActive()
		{
			return TreeSystem.tree.storedItems.Values.Any(n => n.ModItem is Slottable slottable && slottable.effects.Any(n => n.GetType() == GetType()));
		}

		/// <summary>
		/// The amount of doom this individual effect should add when the slottable this effect is on is in the tree
		/// </summary>
		/// <returns></returns>
		public virtual int GetDifficulty()
		{
			return 0;
		}

		/// <summary>
		/// Effects on all players that will occur when this effect is on a slottable that is in the tree
		/// </summary>
		/// <param name="player">The player to effect</param>
		public virtual void BuffPlayer(Player player) { }

		/// <summary>
		/// Effects on all enemies that spawn while this effect is on a slottable that is in the tree
		/// </summary>
		/// <param name="npc">The NPC being spawned</param>
		public virtual void OnEnemySpawn(NPC npc) { }

		/// <summary>
		/// Effects that happen in the world while this effect is on a slottable that is in the tree
		/// </summary>
		/// <param name="slot">The slot passive that the slottable is in</param>
		public virtual void UpdateSocketed(Passive slot) { }

		public SlottableEffect Clone()
		{
			return MemberwiseClone() as SlottableEffect;
		}
	}
}
