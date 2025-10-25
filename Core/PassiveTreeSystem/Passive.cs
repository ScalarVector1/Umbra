using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Terraria.Achievements;
using Terraria.Localization;
using Umbra.Content.Achievements;
using Umbra.Content.GUI;
using Umbra.Content.Passives;

namespace Umbra.Core.PassiveTreeSystem
{
    public abstract class Passive : ModType
    {
        public bool active;

        /// <summary>
        /// The amount of doom this passive will provide if allocated
        /// </summary>
        public int difficulty;
        /// <summary>
        /// The texture this passive will use on the umbral tree
        /// </summary>
        public Asset<Texture2D> texture;
        /// <summary>
        /// Determines the hitbox size for the passive. 0 is for the basic nodes (example: enemy HP), 1 for notables (example: heartbreakers), and 2 for 'keystones' (example: mundanity)
        /// </summary>
        public int size;

        /// <summary>
        /// Opacity of the node on the tree, used to fade ndoes such as those from unloaded mods
        /// </summary>
        public float opacity = 1;

        public List<Passive> connections = [];

        public int ID { get; set; }

        public int Cost { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        /// <summary>
        /// The localization key which this passive attempts to get its display name from
        /// </summary>
        [JsonIgnore]
        public string NameKey => $"Mods.{Mod.Name}.UmbraPassives.{GetType().Name}.Name";
        /// <summary>
        /// The localization key which this passive attempts to get its description from
        /// </summary>
        [JsonIgnore]
        public string TooltipKey => $"Mods.{Mod.Name}.UmbraPassives.{GetType().Name}.Tooltip";

        [JsonIgnore]
        public string DisplayName => Language.GetOrRegister(NameKey).Value;
        [JsonIgnore]
        public string Tooltip => Language.GetOrRegister(TooltipKey).Value;

        [JsonIgnore]
        public Vector2 TreePos => new(X * 16, Y * 16);
        [JsonIgnore]
        public int Width => size == 0 ? 38 : size == 1 ? 50 : size == 2 ? 58 : 38;
        [JsonIgnore]
        public int Height => size == 0 ? 38 : size == 1 ? 50 : size == 2 ? 58 : 38;

        /// <summary>
        /// If more than one of these nodes can be on the tree at a time. Should indicate if the effects can stack
        /// or not. By default only small nodes are allowed to be duplicated.
        /// </summary>
        [JsonIgnore]
        public virtual bool AllowDuplicates => size == 0;

        public Passive()
        {
            SetDefaults();
        }

        /// <summary>
        /// Allows various fields about the passive to be set such as size, difficulty, and texture
        /// </summary>
        public virtual void SetDefaults()
        {
            difficulty = 1;
            texture = Assets.GUI.PassiveFrameTiny;
        }

        /// <summary>
        /// If this passive can ever be active given the current game state.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanBeActive()
        {
            return true;
        }

        /// <summary>
        /// Allows you to create effects that change the player's stats for your passive node
        /// </summary>
        /// <param name="player">The player being (de)buffed</param>
        public virtual void BuffPlayer(Player player) { }

        /// <summary>
        /// Allows you to create effects that modify enemy stats when they spawn
        /// </summary>
        /// <param name="npc">The enemy being spawned</param>
        public virtual void OnEnemySpawn(NPC npc) { }

        /// <summary>
        /// Allows you to create effects that tick every game update
        /// </summary>
        public virtual void Update() { }

        public void Draw(SpriteBatch spriteBatch, Vector2 center, float scale)
        {
            Texture2D tex = texture?.Value ?? Assets.GUI.PassiveFrameTiny.Value;

            Color color = Color.DimGray;

            if (CanAllocate(Main.LocalPlayer))
                color = Color.Lerp(Color.Gray, Color.LightGray, (float)Math.Sin(Main.timeForVisualEffects * 0.1f) * 0.5f + 0.5f);

            if (active || Tree.editing)
            {
                color = Color.White;
                spriteBatch.Draw(Assets.GUI.GlowAlpha.Value, center, null, new Color(180, 120, 255, 0) * opacity, 0, Assets.GUI.GlowAlpha.Size() / 2f, scale * (0.5f + size * 0.1f), 0, 0);
            }

            spriteBatch.Draw(tex, center, null, color * opacity, 0, tex.Size() / 2f, scale, 0, 0);
        }

        /// <summary>
        /// Called on load to generate the tree edges
        /// </summary>
        /// <param name="all"></param>
        internal void Connect(int otherID)
        {
            TreeSystem.tree.Connect(ID, otherID);
        }

        /// <summary>
        /// If this passive is able to be allocated or not
        /// </summary>
        /// <returns></returns>
        public virtual bool CanAllocate(Player player)
        {
            return
                !active &&
                player.GetModPlayer<TreePlayer>().UmbraPoints >= Cost &&
                connections.Any(n => n.active);
        }

        /// <summary>
        /// Allocates this passive and consumes its cost from the given player
        /// </summary>
        /// <param name="player"></param>
        private void Allocate(Player player)
        {
            player.GetModPlayer<TreePlayer>().UmbraPoints -= Cost;
            TreeSystem.tree.Allocate(ID);
            UmbraNet.SyncPoints(player.whoAmI);

            UmbralAcolyte.condition.Complete();
            UmbralAdept.condition.Value = Math.Max(TreeSystem.tree.difficulty, UmbralAdept.condition.Value);
			UmbralMaster.condition.Value = Math.Max(TreeSystem.tree.difficulty, UmbralMaster.condition.Value);
		}

        /// <summary>
        /// Tries to allocate this passive, returns if it was successful or not
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        internal bool TryAllocate(Player player)
        {
            if (CanAllocate(player))
            {
                Allocate(player);
                return true;
            }

            return false;
        }

        /// <summary>
        /// If this passive can be refunded or not
        /// </summary>
        /// <returns></returns>
        public virtual bool CanDeallocate(Player player)
        {
            return
                active &&
                !connections.Any(n => n.active && !n.HasPathToStartWithout(this));
        }

        internal bool HasPathToStartWithout(Passive excluded)
        {
            HashSet<Passive> visited = [];
            return HasPathToStartWithoutInternal(this, excluded, visited);
        }

        private bool HasPathToStartWithoutInternal(Passive current, Passive excluded, HashSet<Passive> visited)
        {
            if (current == null || current == excluded || !current.active || visited.Contains(current))
                return false;

            if (current is StartPoint)
                return true;

            visited.Add(current);

            foreach (Passive connection in current.connections)
            {
                if (HasPathToStartWithoutInternal(connection, excluded, visited))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Deallocates this passive and refunds half its cost to the given player.
        /// </summary>
        /// <param name="player"></param>
        private void Deallocate(Player player)
        {
            int refundAmount = (int)Math.Ceiling(Cost / 2f);
            player.GetModPlayer<TreePlayer>().UmbraPoints += refundAmount;
            TreeSystem.tree.Deallocate(ID);
            UmbraNet.SyncPoints(player.whoAmI);
        }

        /// <summary>
        /// Tries to deallocate this passive, returns if it was successful or not.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        internal bool TryDeallocate(Player player)
        {
            if (CanDeallocate(player))
            {
                Deallocate(player);
                return true;
            }

            return false;
        }

        internal Passive Clone()
        {
            var clone = MemberwiseClone() as Passive;
            clone.connections = [];
            return clone;
        }

        public sealed override void Register()
        {
            ModTypeLookup<Passive>.Register(this);
        }

        public sealed override void SetupContent()
        {
            _ = DisplayName;
            _ = Tooltip;
            SetStaticDefaults();
        }
    }
}
