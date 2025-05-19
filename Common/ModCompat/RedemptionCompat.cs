
namespace SpiritReforged.Common.ModCompat;

public static class MoRHelper
{
	private static readonly Mod redemption = CrossMod.Redemption.Instance;

	public const short Arcane = 1;
	public const short Fire = 2;
	public const short Water = 3;
	public const short Ice = 4;
	public const short Earth = 5;
	public const short Wind = 6;
	public const short Thunder = 7;
	public const short Holy = 8;
	public const short Shadow = 9;
	public const short Nature = 10;
	public const short Poison = 11;
	public const short Blood = 12;
	public const short Psychic = 13;
	public const short Celestial = 14;
	public const short Explosive = 15;

	public const string NPCType_Skeleton = "Skeleton";
	public const string NPCType_SkeletonHumanoid = "SkeletonHumanoid";
	public const string NPCType_Humanoid = "Humanoid";
	public const string NPCType_Undead = "Undead";
	public const string NPCType_Spirit = "Spirit";
	public const string NPCType_Plantlike = "Plantlike";
	public const string NPCType_Demon = "Demon";
	public const string NPCType_Cold = "Cold";
	public const string NPCType_Hot = "Hot";
	public const string NPCType_Wet = "Wet";
	public const string NPCType_Dragonlike = "Dragonlike";
	public const string NPCType_Inorganic = "Inorganic";
	public const string NPCType_Robotic = "Robotic";
	public const string NPCType_Armed = "Armed";
	public const string NPCType_Hallowed = "Hallowed";
	public const string NPCType_Dark = "Dark";
	public const string NPCType_Blood = "Blood";
	public const string NPCType_Slime = "Slime";

	// ------------------------------------------------------------------------------------------------------
	// These go in SetStaticDefaults()
	public static void AddItemToBluntSwing(this Item item)
	{
		if (!CrossMod.Redemption.Enabled)
			return;
		redemption.Call("addItemToBluntSwing", item.type);
	}
	public static void AddElement(this Entity entity, int ElementID, bool projsInheritElements = false)
	{
		if (!CrossMod.Redemption.Enabled)
			return;
		if (entity is Item item)
			redemption.Call("addElementItem", ElementID, item.type, projsInheritElements);
		else if (entity is NPC npc)
			redemption.Call("addElementNPC", ElementID, npc.type);
		else if (entity is Projectile proj)
			redemption.Call("addElementProj", ElementID, proj.type, projsInheritElements);
	}
	public static void AddNPCElementList(this NPC npc, string TypeString)
	{
		if (!CrossMod.Redemption.Enabled)
			return;
		redemption.Call("addNPCToElementTypeList", TypeString, npc.type);
	}
	// ------------------------------------------------------------------------------------------------------
	// These are dynamic, so they can go in SetDefaults or wherever you want to update them
	// Keep in mind they don't get reset, so not required to put in an Update method that happens every frame
	public static void OverrideElement(this Entity entity, int ElementID, int overrideID = 1)
	{
		if (!CrossMod.Redemption.Enabled)
			return;
		if (entity is Item item)
			redemption.Call("elementOverrideItem", item, ElementID, overrideID);
		else if (entity is NPC npc)
			redemption.Call("elementOverrideNPC", npc, ElementID, overrideID);
		else if (entity is Projectile proj)
			redemption.Call("elementOverrideProj", proj, ElementID, overrideID);
	}
	public static void OverrideElementMultiplier(this NPC npc, int ElementID, float value = 1, bool dontSetMultipliers = false)
	{
		if (!CrossMod.Redemption.Enabled)
			return;
		redemption.Call("elementMultiplier", npc, ElementID, value, dontSetMultipliers);
	}
	public static void NoBossMultiplierCap(this NPC npc, bool uncap = true)
	{
		if (!CrossMod.Redemption.Enabled)
			return;
		redemption.Call("uncapBossElementMultiplier", npc, uncap);
	}
	public static void HideElementIcon(this Item item, int ElementID, bool hidden = true)
	{
		if (!CrossMod.Redemption.Enabled)
			return;
		redemption.Call("hideElementIcon", item, ElementID, hidden);
	}
	public static bool Decapitation(NPC target, ref int damageDone, ref bool crit, int chance = 200)
	{
		if (!CrossMod.Redemption.Enabled)
			return false;
		return (bool)redemption.Call("decapitation", target, damageDone, crit, chance);
	}
	public static bool SetSlashBonus(this Item item, bool setBonus = true)
	{
		if (!CrossMod.Redemption.Enabled)
			return false;
		return (bool)redemption.Call("setSlashBonus", item, setBonus);
	}
	public static bool SetAxeBonus(this Item item, bool setBonus = true)
	{
		if (!CrossMod.Redemption.Enabled)
			return false;
		return (bool)redemption.Call("setAxeBonus", item, setBonus);
	}
	public static bool SetAxeBonus(this Projectile proj, bool setBonus = true)
	{
		if (!CrossMod.Redemption.Enabled)
			return false;
		return (bool)redemption.Call("setAxeProj", proj, setBonus);
	}
	public static bool SetHammerBonus(this Item item, bool setBonus = true)
	{
		if (!CrossMod.Redemption.Enabled)
			return false;
		return (bool)redemption.Call("setHammerBonus", item, setBonus);
	}
	public static bool SetHammerBonus(this Projectile proj, bool setBonus = true)
	{
		if (!CrossMod.Redemption.Enabled)
			return false;
		return (bool)redemption.Call("setHammerProj", proj, setBonus);
	}
	// Items already have "ItemID.Sets.Spears[Item.type]" to set them as spears
	public static bool SetSpearBonus(this Projectile proj, bool setBonus = true)
	{
		if (!CrossMod.Redemption.Enabled)
			return false;
		return (bool)redemption.Call("setSpearProj", proj, setBonus);
	}
	// ------------------------------------------------------------------------------------------------------
	public static bool HasElement(this Entity entity, int ElementID)
	{
		if (!CrossMod.Redemption.Enabled)
			return false;
		if (entity is Item item)
			return (bool)redemption.Call("hasElementItem", item, ElementID);
		else if (entity is NPC npc)
			return (bool)redemption.Call("elementOverrideNPC", npc, ElementID);
		else if (entity is Projectile proj)
			return (bool)redemption.Call("elementOverrideProj", proj, ElementID);
		return false;
	}
	public static int GetFirstElement(this Entity entity, bool ignoreExplosive = false)
	{
		if (!CrossMod.Redemption.Enabled)
			return 0;
		if (entity is Item item)
			return (int)redemption.Call("getFirstElementItem", item, ignoreExplosive);
		else if (entity is NPC npc)
			return (int)redemption.Call("getFirstElementNPC", npc, ignoreExplosive);
		else if (entity is Projectile proj)
			return (int)redemption.Call("getFirstElementProj", proj, ignoreExplosive);
		return 0;
	}
}