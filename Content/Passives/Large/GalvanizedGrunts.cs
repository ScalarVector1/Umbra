using Terraria.Audio;
using Terraria.ID;
using Umbra.Core;
using Umbra.Core.PassiveTreeSystem;
using Umbra.Helpers;

namespace Umbra.Content.Passives.Large
{
    internal class GalvanizedGrunts : Passive
    {
        public override void SetDefaults()
        {
            texture = Assets.Passives.GalvanizedGrunts;
            difficulty = 20;
            size = 1;
        }

        public override void OnEnemySpawn(NPC npc)
        {
            if (!npc.boss)
            {
                npc.GetGlobalNPC<GalvanizedGruntsNPC>().active = true;
            }
        }
    }

    internal class GalvanizedGruntsNPC : GlobalNPC
    {
        public bool active;
        public int stacksRemaining;
        public bool triggered;

        public int fadeIn;

        public override bool InstancePerEntity => true;

        public override bool PreAI(NPC npc)
        {
            if (active && npc.life < npc.lifeMax / 2 && !triggered)
            {
                triggered = true;
                stacksRemaining = 10;
            }

            if (active && triggered && fadeIn < 120)
            {
                fadeIn++;
            }

            return true;
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (active)
                modifiers.Defense.Flat += stacksRemaining * 5;
        }

        public override void HitEffect(NPC npc, NPC.HitInfo hit)
        {
            if (active && stacksRemaining > 0)
            {
                stacksRemaining--;

                for (int k = 0; k < 5; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.FireworkFountain_Yellow, 0, -1, 150, default, 0.5f);
                }

                SoundEngine.PlaySound(SoundID.NPCHit4.WithPitchOffset(-0.5f), npc.Center);
            }
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (active && triggered)
            {
                ModContent.GetInstance<PixelationSystem>().QueueRenderAction("OverPlayers", () =>
                {
                    Texture2D tex = Assets.Masks.RingGlow.Value;
                    Texture2D inner = Assets.Masks.GlowSoftAlpha.Value;

                    float opacity = stacksRemaining / 10f * Eases.SwoopEase(fadeIn / 120f);
                    Color color = new Color(255, 255, 100, 0) * opacity;

                    var pulse = new Vector2(1f + 0.05f * MathF.Sin(Main.GameUpdateCount * 0.1f), 1f + 0.05f * MathF.Cos(Main.GameUpdateCount * 0.1f));
                    float largest = npc.width > npc.height ? npc.width : npc.height;

                    Vector2 scale = new Vector2(largest / tex.Width, largest / tex.Height) * pulse;
                    spriteBatch.Draw(tex, npc.Center - Main.screenPosition, null, color, 0f, tex.Size() / 2f, scale * 1.5f, 0, 0);

                    Vector2 scale2 = new Vector2(largest / inner.Width, largest / inner.Height) * pulse;
                    spriteBatch.Draw(inner, npc.Center - Main.screenPosition, null, color * 0.75f, 0f, inner.Size() / 2f, scale2 * 1.5f, 0, 0);
                });
            }
        }
    }
}
