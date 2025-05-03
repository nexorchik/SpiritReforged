using System.Linq;

namespace SpiritReforged.Common.BuffCommon;

/// <summary> A stacking buff definition designed for NPCs. </summary>
public abstract class StackingBuff : ILoadable
{
	#region handler
	private static readonly Dictionary<Type, StackingBuff> Loaded = [];
	public Mod Mod { get; private set; }

	/// <summary> Creates a new instance from template T. </summary>
	public static StackingBuff NewBuff<T>() where T : StackingBuff => Loaded[typeof(T)].MemberwiseClone() as StackingBuff;

	public void Unload() { }
	public void Load(Mod mod)
	{
		Mod = mod;
		Loaded.Add(GetType(), this);

		Load();
	}
	#endregion

	public byte MaxStacks { get; protected set; } = 1;
	public byte stacks;
	public int duration;

	public virtual void Load() { }
	/// <summary> Called whenever this buff is added to an NPC. Can also be used to set defaults. </summary>
	public virtual void OnAdded() { }
	/// <summary> Called whenever this buff is removed from an NPC. </summary>
	public virtual void OnRemoved(bool timedOut) { }
	public virtual void UpdateEffects(NPC npc) { }
	public virtual void DrawDisplay(SpriteBatch spriteBatch, Vector2 position) { }
}

internal class StackingNPC : GlobalNPC
{
	public override bool InstancePerEntity => true;

	private readonly HashSet<StackingBuff> stackingBuffs = [];

	/// <param name="stack"> The number of stacks to add. </param>
	/// <param name="replace"> Whether this buff should add stacks to a previously applied buff of the same type. </param>
	internal void AddBuff<T>(int duration, byte stack, bool replace = false) where T : StackingBuff
	{
		var inst = StackingBuff.NewBuff<T>();
		inst.OnAdded();

		if (replace)
		{
			stackingBuffs.RemoveWhere(x => x.GetType() == typeof(T));
		}
		else
		{
			foreach (var item in stackingBuffs)
			{
				if (item.GetType() == typeof(T))
				{
					stack += item.stacks;

					stackingBuffs.Remove(item);
					break;
				}
			}
		}

		inst.duration = duration;
		inst.stacks = Math.Min(stack, inst.MaxStacks);

		stackingBuffs.Add(inst);
	}

	internal bool RemoveBuff<T>() where T : StackingBuff
	{
		bool value = false;

		foreach (var item in stackingBuffs)
		{
			if (item.GetType() == typeof(T))
			{
				value = stackingBuffs.Remove(item);
				item.OnRemoved(false);

				break;
			}
		}

		return value;
	}

	internal bool HasBuff<T>(out T value) where T : StackingBuff
	{
		if (stackingBuffs.FirstOrDefault(x => x.GetType() == typeof(T)) is StackingBuff def)
		{
			value = def as T;
			return true;
		}

		value = null;
		return false;
	}

	public override void UpdateLifeRegen(NPC npc, ref int damage)
	{
		List<StackingBuff> removals = [];

		foreach (var buff in stackingBuffs)
		{
			buff.UpdateEffects(npc);

			if (--buff.duration <= 0)
				removals.Add(buff);
		}

		foreach (var entry in removals)
		{
			stackingBuffs.Remove(entry);
			entry.OnRemoved(true);
		}
	}

	public override bool? DrawHealthBar(NPC npc, byte hbPosition, ref float scale, ref Vector2 position)
	{
		const int padding = 16;
		var pos = position + new Vector2(padding * (stackingBuffs.Count - 1) * -0.5f, 22);

		foreach (var buff in stackingBuffs)
		{
			buff.DrawDisplay(Main.spriteBatch, pos);
			pos.X += padding;
		}

		return null;
	}
}

internal static class StackingHelper
{
	public static void AddStackingBuff<T>(this NPC npc, int duration, byte stack = 1) where T : StackingBuff
	{
		if (npc.TryGetGlobalNPC(out StackingNPC sNPC))
			sNPC.AddBuff<T>(duration, stack);
	}

	public static bool RemoveStackingBuff<T>(this NPC npc) where T : StackingBuff
	{
		if (npc.TryGetGlobalNPC(out StackingNPC sNPC))
			return sNPC.RemoveBuff<T>();

		return false;
	}

	public static bool HasStackingBuff<T>(this NPC npc, out T value) where T : StackingBuff
	{
		if (npc.TryGetGlobalNPC(out StackingNPC sNPC) && sNPC.HasBuff(out value))
			return true;

		value = null;
		return false;
	}
}