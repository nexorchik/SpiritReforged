using MonoMod.Utils;
using SpiritReforged.Common.NPCCommon;
using SpiritReforged.Common.WorldGeneration.Micropasses.Passes;
using System.Linq;
using System.Reflection;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace SpiritReforged.Content.Forest.Misc;

[AutoloadHead]
public class WorldFrog : ModNPC
{
	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[Type] = 10;
		NPCID.Sets.NoTownNPCHappiness[Type] = true;

		NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers()
		{ Hide = true });
	}

	public override void SetDefaults()
	{
		NPC.CloneDefaults(NPCID.TownSlimeBlue);
		NPC.townNPC = false;
	}

	public override void SetChatButtons(ref string button, ref string button2)
	{
		if (UpdaterSystem.Instance.AnyTask())
			button = "Update";
	}

	public override void OnChatButtonClicked(bool firstButton, ref string shopName)
	{
		UpdaterSystem.RunFirstTask(out string report);
		Main.npcChatText = report;
	}

	public override bool CanChat() => true;
	public override string GetChat()
	{
		return base.GetChat();
	}

	public override bool CheckActive() => false;
	public override void HitEffect(NPC.HitInfo hit)
	{
		if (Main.dedServ)
			return;

		if (NPC.life <= 0)
		{
			for (int i = 1; i < 6; i++)
			{
				int goreType = Mod.Find<ModGore>(nameof(Hiker) + i).Type;
				Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), NPC.velocity, goreType);
			}
		}

		for (int d = 0; d < 8; d++)
			Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(NPC.getRect()), DustID.Blood,
				Main.rand.NextVector2Unit() * 1.5f, 0, default, Main.rand.NextFloat(1f, 1.5f));
	}

	public override void FindFrame(int frameHeight)
	{
		NPC.frame.Width = 36;

		bool moving = NPC.velocity.X != 0;
		int lastFrame = moving ? 5 : Main.npcFrameCount[Type];

		if (!moving)
		{
			if (NPC.frameCounter > 0.15f || Main.rand.NextBool(50))
				NPC.frameCounter += 0.15f;
		}
		else
		{
			NPC.frameCounter += 0.15f;
		}

		NPC.frameCounter = NPC.frameCounter % lastFrame;

		NPC.frame.X = moving ? NPC.frame.Width : 0;
		NPC.frame.Y = (int)NPC.frameCounter * frameHeight;
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		NPC.spriteDirection = NPC.direction;

		var texture = TextureAssets.Npc[Type].Value;
		var effects = (NPC.spriteDirection == 1) ? SpriteEffects.FlipHorizontally : default;

		var frame = NPC.frame with { Width = NPC.frame.Width - 2, Height = NPC.frame.Height - 2 };
		var origin = new Vector2(frame.Width / 2, frame.Height);
		var position = NPC.Bottom - screenPos + new Vector2(0, NPC.gfxOffY);

		spriteBatch.Draw(texture, position, frame, NPC.DrawColor(drawColor), NPC.rotation, origin, NPC.scale, effects, 0);
		return false;
	}
}

internal class UpdaterSystem : ModSystem
{
	/// <summary> Stores the mod version that a particular delegate task belongs to. </summary>
	[AttributeUsage(AttributeTargets.Method)]
	private class VerAttribute(string Version) : Attribute
	{
		public string Version = Version;
	}

	private delegate void TaskDelegate(out string report);

	public static UpdaterSystem Instance { get; private set; }

	public static Version LastVersion { get; private set; }
	private static readonly Dictionary<Version, TaskDelegate> Tasks = [];

	#region tModLoader hooks
	public override void Load() => Instance = this;

	public override void PostWorldGen() => LastVersion = SpiritReforgedMod.Instance.Version;
	public override void ClearWorld() => LastVersion = null;

	public override void OnWorldLoad()
	{
		if (Main.netMode == NetmodeID.SinglePlayer && AnyTask()) //Only spawn in singleplayer to avoid potential complications
			NPC.NewNPC(new EntitySource_SpawnNPC(), Main.spawnTileX * 16, Main.spawnTileY * 16, ModContent.NPCType<WorldFrog>());
	}

	public override void SaveWorldData(TagCompound tag)
	{
		if (LastVersion != null)
			tag[nameof(LastVersion)] = LastVersion;
	}
	public override void LoadWorldData(TagCompound tag) => LastVersion = tag.Get<Version>(nameof(LastVersion));
	//public override void NetSend(BinaryWriter writer) => writer.Write(LastVersion.ToString());
	//public override void NetReceive(BinaryReader reader) => LastVersion = new(reader.ReadString());
	#endregion

	public bool AnyTask()
	{
		if (Tasks.Count == 0)
		{
			foreach (var method in GetType().GetMethods(BindingFlags.Static | BindingFlags.NonPublic))
			{
				if (method.GetCustomAttribute<VerAttribute>() is VerAttribute ver && method.TryCreateDelegate<TaskDelegate>() is TaskDelegate dele)
					Tasks.Add(new Version(ver.Version), dele);
			}
		}

		return Tasks.Any(x => Lower(x.Key));
		static bool Lower(Version value) => LastVersion is null || LastVersion < value;
	}

	/// <summary> Runs the next update task and adjusts <see cref="LastVersion"/> accordingly.<br/>
	/// <see cref="AnyTask"/> must return true for this to be called safely. </summary>
	public static void RunFirstTask(out string report)
	{
		var task = Tasks.OrderBy(x => x.Key).First();
		task.Value.Invoke(out string reportKey);

		report = Language.GetTextValue("Mods.SpiritReforged.NPCs.WorldFrog.Reports." + reportKey);
		LastVersion = task.Key;
	}

	[Ver("0.1.1.0")]
	private static void CavesAndClubs(out string report)
	{
		ModContent.GetInstance<PotsMicropass>().Run(new(), null);
		report = "CavesAndClubs";
	}
}