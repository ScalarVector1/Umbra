using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using Terraria.ModLoader.IO;
using Umbra.Content.Items.Slottables.Effects;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Items.Slottables
{
	internal abstract class Slottable : ModItem
	{
		/// <summary>
		/// Total difficulty, base + effects
		/// </summary>
		public int Difficulty => baseDifficulty + effectDifficulty;

		/// <summary>
		/// Difficulty associted with this slottable innately
		/// </summary>
		public int baseDifficulty = 0;
		/// <summary>
		/// Difficulty from effects. Call CalculateEffectDifficulty to recalculate this.
		/// </summary>
		public int effectDifficulty = 0;
		/// <summary>
		/// Collection of effects tied to this slottable.
		/// </summary>
		public List<SlottableEffect> effects = [];

		public override void SetDefaults()
		{
			Item.width = 42;
			Item.height = 42;
			Item.color = Color.White;
		}

		/// <summary>
		/// What this slottable should look like when drawn in the socket on the tree
		/// </summary>
		/// <param name="spriteBatch"></param>
		/// <param name="center"></param>
		/// <param name="scale"></param>
		/// <param name="slotID"></param>
		public virtual void DrawInSlot(SpriteBatch spriteBatch, Vector2 center, float scale, int slotID)
		{

		}

		/// <summary>
		/// Effects that should happen when this slottable is socketed in the tree. Note that this wont be called on load,
		/// so this should typically be constrained to things like sound effects
		/// </summary>
		public virtual void OnSocket()
		{

		}

		/// <summary>
		/// Effects that should happen when this slottable is desocketed in the tree. Note that this wont be called on unload,
		/// so this should typically be constrained to things like sound effects
		/// </summary>
		public virtual void OnDesocket()
		{

		}

		/// <summary>
		/// Effects that should happen to every player when this socketable is in the tree
		/// </summary>
		/// <param name="player">The player to be buffed</param>
		public virtual void BuffPlayer(Player player)
		{
			foreach (SlottableEffect effect in effects)
			{
				effect.BuffPlayer(player);
			}
		}

		/// <summary>
		/// Effects that should happen to every NPC that spawns when this socketable is in the tree
		/// </summary>
		/// <param name="npc">The NPC being spawned</param>
		public virtual void OnEnemySpawn(NPC npc)
		{
			foreach (SlottableEffect effect in effects)
			{
				effect.OnEnemySpawn(npc);
			}
		}

		/// <summary>
		/// Effects that should happen passively in the world when this socketable is in the tree
		/// </summary>
		/// <param name="slot">The socket this slottable is in</param>
		public virtual void UpdateSocketed(Passive slot)
		{
			foreach (SlottableEffect effect in effects)
			{
				effect.UpdateSocketed(slot);
			}
		}

		/// <summary>
		/// Recalculates the difficulty of this slottable's effects, should be called whenever they change
		/// </summary>
		public void CalculateEffectDifficulty()
		{
			effectDifficulty = 0;

			foreach (SlottableEffect effect in effects)
			{
				effectDifficulty += effect.GetDifficulty();
			}
		}

		/// <summary>
		/// Adds an effect to this slottable
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="tier"></param>
		public void AddEffect<T>(int tier) where T : SlottableEffect
		{
			SlottableEffect effect = ModContent.GetInstance<T>().Clone();
			effect.tier = tier;

			effects.Add(effect);

			CalculateEffectDifficulty();
		}

		/// <summary>
		/// Adds an effect to this slottable
		/// </summary>
		/// <param name="effect"></param>
		/// <param name="tier"></param>
		public void AddEffect(SlottableEffect effect, int tier)
		{
			var newEffect = effect.Clone();
			newEffect.tier = tier;

			effects.Add(newEffect);

			CalculateEffectDifficulty();
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			foreach (SlottableEffect effect in effects)
			{
				tooltips.Add(new($"SlottableEffect{effect.Name}", effect.Tooltip));
			}
		}

		public override void NetSend(BinaryWriter writer)
		{
			writer.Write(effects.Count);
			for(int k = 0; k < effects.Count; k++)
			{
				writer.Write(effects[k].FullName);
				writer.Write(effects[k].tier);
			}

			writer.Write(Item.color.R);
			writer.Write(Item.color.G);
			writer.Write(Item.color.B);
		}

		public override void NetReceive(BinaryReader reader)
		{
			effects.Clear();

			int count = reader.ReadInt32();

			for(int k = 0; k < count; k++)
			{
				effects.Add(SlottableEffect.FromNet(reader.ReadString(), reader.ReadInt32()));
			}

			Item.color.R = reader.ReadByte();
			Item.color.G = reader.ReadByte();
			Item.color.B = reader.ReadByte();

			CalculateEffectDifficulty();
		}

		public override void SaveData(TagCompound tag)
		{
			tag["effects"] = effects.Select(n => n.SaveData()).ToList();
			tag["r"] = Item.color.R;
			tag["g"] = Item.color.G;
			tag["b"] = Item.color.B;
		}

		public override void LoadData(TagCompound tag)
		{
			var effectTags = tag.GetList<TagCompound>("effects").ToList();

			effects.Clear();

			foreach(var effectTag in effectTags)
			{
				var loaded = SlottableEffect.FromData(effectTag);

				if (loaded != null)
					effects.Add(loaded);
			}

			Item.color = new Color(tag.GetByte("r"), tag.GetByte("g"), tag.GetByte("b"));

			CalculateEffectDifficulty();
		}
	}
}
