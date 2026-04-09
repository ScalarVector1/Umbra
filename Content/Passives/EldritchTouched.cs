using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Umbra.Compat;
using Umbra.Core.Loaders;
using Umbra.Core.PassiveTreeSystem;
using Umbra.Core.PixelationSystem;
using Umbra.Helpers;

namespace Umbra.Content.Passives
{
	internal class EldritchTouched : Passive
	{
		public override void SetDefaults()
		{
			texture = Assets.Passives.EldritchTouched;
			difficulty = 5;
		}

		public override void Update()
		{
			EldritchTouchedSystem.eldritchTouchedChance += 0.01f;
		}

		public override void OnEnemySpawn(NPC npc)
		{
			if (!ExtraBossMarks.DoICountAsABoss(npc) && npc.TryGetGlobalNPC<TreeNPC>(out TreeNPC tnpc) && npc.TryGetGlobalNPC(out EldritchTouchedNPC enpc))
			{
				if (!tnpc.isSpecial && Main.rand.NextFloat() < EldritchTouchedSystem.eldritchTouchedChance)
				{
					Main.NewText(npc.GivenName + " is eldritch touched!");
					enpc.isEldritchTouched = true;

					tnpc.moreLife.Add(2f);

					tnpc.isSpecial = true;
				}
			}
		}
	}

	public class EldritchTouchedSystem : ModSystem
	{
		public static float eldritchTouchedChance;

		public override void PostUpdateEverything()
		{
			eldritchTouchedChance = 0;
		}
	}

	public class EldritchTouchedNPC : GlobalNPC
	{
		public bool isEldritchTouched;
		public TentacleMassRenderer auraTentacles;

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
		{
			return !entity.friendly && entity.lifeMax > 5 && !NPCID.Sets.CountsAsCritter[entity.type] && entity.damage > 0;
		}

		public override bool PreAI(NPC npc)
		{
			if (isEldritchTouched && !Main.dedServ)
			{
				if (!Main.projectile.Any(n => n.active && n.ModProjectile is EldritchSwipe swipe && swipe.following == npc))
				{
					Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<EldritchSwipe>(), 1, 0, npc.whoAmI);
				}

				auraTentacles ??= new(MathF.Max(npc.width, npc.height) * 0.1f, new Color(100, 255, 220) * 0.4f);
				auraTentacles.ManageCaches(npc.Center);
				auraTentacles.Update();

				Lighting.AddLight(npc.Center, Color.Teal.ToVector3());
			}

			return true;
		}

		public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (isEldritchTouched)
			{
				ModContent.GetInstance<PixelationSystem>().QueueRenderAction("UnderNPCs", () => auraTentacles?.Render());

				var eye = Assets.Extra.EldritchCore.Value;
				spriteBatch.Draw(eye, npc.Center + Vector2.UnitY * (-npc.height - 32) - Main.screenPosition, Color.White);
			}

			return true;
		}
	}

	public class EldritchSwipe : ModProjectile
	{
		public NPC following;

		public int attackTimer;

		public override string Texture => "Umbra/Assets/Invisible";

		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;
			Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 30;
			Projectile.penetrate = -1;
		}

		public override void AI()
		{
			if (following is null || !following.active)
				return;

			Projectile.timeLeft = 30;
			Vector2 target = following.Center + Vector2.UnitY * (-following.height - 32);
			Projectile.Center += Projectile.Center - target * 0.05f;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			var tex = Assets.Extra.EldritchCore.Value;
			var texGlow = Assets.Extra.EldritchCoreGlow.Value;

			Rectangle frame = new Rectangle(0, Projectile.frame * 19, 18, 18);

			Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, Lighting.GetColor((Projectile.Center / 16).ToPoint()), Projectile.rotation, Vector2.One * 9, Projectile.scale, 0);

			return false;
		}
	}

	public class TentacleMassRenderer
	{
		private List<Vector2> cache;
		public Trail trail;
		public float scale;
		public Color color;

		public float[] tentacleTimers = new float[9];

		public TentacleMassRenderer(float scale, Color color)
		{
			cache = [];
			this.scale = scale;
			this.color = color;
		}

		public void ManageCaches(Vector2 center)
		{
			Vector2 pos = center;

			cache.Clear();

			Random rand = new Random(97598372);

			for (int tentacle = 0; tentacle < 9; tentacle++)
			{
				float tentScale = 0.5f + rand.NextSingle() * 0.7f;
				float tentSpeedOffset = rand.NextSingle();
				float tentSpeed = 0.15f + MathF.Sin(Main.GameUpdateCount * 0.08f + tentSpeedOffset * MathF.Tau) * 0.05f;

				tentacleTimers[tentacle] -= tentSpeed;

				for (int tentPoint = 0; tentPoint < 10; tentPoint++)
				{
					int index = tentacle * 10 + tentPoint;
					float tentRot = tentacle / 9f * MathF.Tau;
					float dist = tentPoint * scale * tentScale;

					Vector2 offset = MathF.Sin(tentPoint / 10f) * Vector2.UnitX.RotatedBy(tentRot + MathF.PI / 2f) * MathF.Sin(tentacleTimers[tentacle] + tentPoint * 1.5f + tentacle * 1.46f) * scale * tentScale * (1f + tentSpeed * 2f);

					cache.Add(pos + Vector2.UnitX.RotatedBy(tentRot) * dist + offset);
				}
			}
		}

		public void Update()
		{
			if (trail is null || trail.IsDisposed)
			{
				trail = new Trail(Main.instance.GraphicsDevice, 90, new NoTip(), factor => (1f - factor * 9 % 1) * scale, factor =>
				{
					var prog = factor.X * 9 % 1;
					if (prog > 0.9f || prog < 0.1f)
						return Color.Transparent;

					return color;
				});
			}

			trail.Positions = cache.ToArray();
			trail.NextPosition = Vector2.Zero;
		}

		public void Render()
		{
			Effect effect = ShaderLoader.GetShader("RepeatingChain").Value;

			if (effect != null)
			{
				var world = Matrix.CreateTranslation(-Main.screenPosition.ToVector3());
				Matrix view = Matrix.Identity;
				var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

				effect.Parameters["alpha"].SetValue(1f);
				effect.Parameters["repeats"].SetValue(1f);
				effect.Parameters["transformMatrix"].SetValue(world * view * projection);
				effect.Parameters["sampleTexture"].SetValue(Assets.MagicPixel.Value);
				trail?.Render(effect);
			}
		}
	}
}
