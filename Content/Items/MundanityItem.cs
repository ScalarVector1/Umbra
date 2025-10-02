using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;
using Umbra.Content.Passives.Large;
using Umbra.Core.PassiveTreeSystem;

namespace Umbra.Content.Items
{
    internal class MundanityItem : ModItem
    {
        public override string Texture => "Umbra/Assets/Items/MundanityItem";

        public override void Load()
        {
            On_ItemSlot.LeftClick_ItemArray_int_int += HandleSpecialItemInteractions;
            On_ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += DrawSpecial;
            On_ItemSlot.RightClick_ItemArray_int_int += NoSwapCurse;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.rare = ItemRarityID.Gray;
            Item.value = 0;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(n => n.Mod == "Terraria" && n.Name == "Equipable");
        }

        public override void UpdateEquip(Player player)
        {
            if (!TreeSystem.tree.AnyActive<Mundanity>())
                Item.TurnToAir();
        }

        private void HandleSpecialItemInteractions(On_ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
        {
            if (inv[slot].ModItem is MundanityItem && context == 10)
            {
                ItemLoader.CanEquipAccessory(Main.mouseItem, slot, true);
                return;
            }

            orig(inv, context, slot);
        }

        private void NoSwapCurse(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
        {
            Player Player = Main.player[Main.myPlayer];

            for (int i = 0; i < Player.armor.Length; i++)
            {
                if (Player.armor[i].ModItem is MundanityItem && ItemSlot.ShiftInUse && inv[slot].accessory)
                    return;
            }

            if (inv == Player.armor)
            {
                Item swaptarget = Player.armor[slot - 10];

                if (context == 11 && swaptarget.ModItem is MundanityItem)
                    return;
            }

            orig(inv, context, slot);
        }

        private void DrawSpecial(On_ItemSlot.orig_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color orig, SpriteBatch sb, Item[] inv, int context, int slot, Vector2 position, Color color)
        {
            if (inv[slot].ModItem is MundanityItem && context == 10)
            {
                Texture2D back = Assets.GUI.MundaneSlot.Value;
                Color backcolor = (!Main.expertMode && slot == 8) ? Color.White * 0.25f : Color.White * 0.75f;

                sb.Draw(back, position, null, backcolor, 0f, default, Main.inventoryScale, SpriteEffects.None, 0f);
                RedrawItem(sb, inv, position, slot, color);
            }
            else
            {
                orig(sb, inv, context, slot, position, color);
            }
        }

        //this is vanilla code. Only reasonable alternative is likely porting all drawing to IL.
        internal static void RedrawItem(SpriteBatch sb, Item[] inv, Vector2 position, int slot, Color color)
        {
            Item item = inv[slot];
            Vector2 scaleVector = Vector2.One * 52 * Main.inventoryScale;
            Texture2D popupTex = Terraria.GameContent.TextureAssets.Item[item.type].Value;
            Rectangle source = popupTex.Frame(1, 1, 0, 0);
            Color currentColor = color;
            float scaleFactor2 = 1f;
            ItemSlot.GetItemLight(ref currentColor, ref scaleFactor2, item, false);
            float scaleFactor = 1f;

            if (source.Width > 32 || source.Height > 32)
                scaleFactor = (source.Width <= source.Height) ? (32f / source.Height) : (32f / source.Width);

            scaleFactor *= Main.inventoryScale;
            Vector2 drawPos = position + scaleVector / 2f - source.Size() * scaleFactor / 2f;
            Vector2 origin = source.Size() * (scaleFactor2 / 2f - 0.5f);

            if (ItemLoader.PreDrawInInventory(item, sb, drawPos, source, item.GetAlpha(currentColor), item.GetColor(color), origin, scaleFactor * scaleFactor2))
                sb.Draw(popupTex, drawPos, source, Color.White, 0f, origin, scaleFactor * scaleFactor2, SpriteEffects.None, 0f);

            ItemLoader.PostDrawInInventory(item, sb, drawPos, source, item.GetAlpha(currentColor), item.GetColor(color), origin, scaleFactor * scaleFactor2);
        }
    }
}
