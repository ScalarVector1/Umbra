using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;

namespace Umbra.Content.Projectiles
{
    internal class Spawner : ModProjectile
    {
        public float hpOverride = -1;
        public float damageOverride = -1;
        public float defenseOverride = -1;

        public ref float SpawnType => ref Projectile.ai[0];
        public ref float projScale => ref Projectile.ai[1];

        public override string Texture => "Umbra/Assets/Invisible";

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Projectile.scale = projScale;

            if (Projectile.timeLeft == 70)
                SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, Projectile.Center);

            if (Projectile.timeLeft == 30 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                var n = NPC.NewNPC(Projectile.GetSource_FromThis(), (int)Projectile.Center.X, (int)Projectile.Center.Y, (int)SpawnType);
                Main.npc[n].SpawnedFromStatue = true;
            }
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D tex = Assets.Masks.ShinyGlow.Value;
            Texture2D texRing = Assets.Masks.RingGlow.Value;

            float bright = Helpers.Eases.BezierEase(1 - (Projectile.timeLeft - 60) / 120f);

            if (Projectile.timeLeft < 20)
                bright = Projectile.timeLeft / 20f;

            float starScale = Helpers.Eases.BezierEase(1 - (Projectile.timeLeft - 90) / 30f);

            if (Projectile.timeLeft <= 90)
                starScale = 0.3f + Projectile.timeLeft / 90f * 0.7f;

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, new Color(155, 0, 255, 0) * bright, Helpers.Eases.BezierEase(Projectile.timeLeft / 160f) * 6.28f, tex.Size() / 2, starScale * 0.3f * Projectile.scale, 0, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0) * bright, Helpers.Eases.BezierEase(Projectile.timeLeft / 160f) * 6.28f, tex.Size() / 2, starScale * 0.2f * Projectile.scale, 0, 0);

            float ringBright = 1;
            if (Projectile.timeLeft > 100)
                ringBright = 1 - (Projectile.timeLeft - 100) / 20f;

            float ringScale = 1 + (Projectile.timeLeft - 50) / 70f * 0.3f;

            if (Projectile.timeLeft <= 50)
                ringScale = Helpers.Eases.BezierEase((Projectile.timeLeft - 20) / 30f);

            Main.spriteBatch.Draw(texRing, Projectile.Center - Main.screenPosition, null, new Color(155, 0, 255, 0) * ringBright * 0.8f, Projectile.timeLeft / 60f * 6.28f, texRing.Size() / 2, ringScale * 0.2f * Projectile.scale, 0, 0);
            Main.spriteBatch.Draw(texRing, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0) * ringBright * 0.5f, Projectile.timeLeft / 60f * 6.28f, texRing.Size() / 2, ringScale * 0.195f * Projectile.scale, 0, 0);

            if (Projectile.timeLeft < 30)
            {
                Texture2D tex2 = Assets.Masks.GlowSoftAlpha.Value;
                Main.spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, new Color(155, 50, 255, 0) * (Projectile.timeLeft / 30f), 0, tex2.Size() / 2, (1 - Projectile.timeLeft / 30f) * 7 * Projectile.scale, 0, 0);
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, new Color(255, 150, 150, 0) * (Projectile.timeLeft / 30f), 0, tex.Size() / 2, (1 - Projectile.timeLeft / 30f) * 1 * Projectile.scale, 0, 0);

                if (Projectile.timeLeft > 15)
                    Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, new Color(155, 100, 255, 0) * ((Projectile.timeLeft - 15) / 15f), 1.57f / 4, tex.Size() / 2, (1 - (Projectile.timeLeft - 15) / 15f) * 2 * Projectile.scale, 0, 0);
            }
        }
    }
}
