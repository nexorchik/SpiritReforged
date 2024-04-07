using System;

namespace SpiritReforged.Common.TileCommon;

[AttributeUsage(AttributeTargets.Class)]
public class TileTagAttribute(params TileTags[] tags) : Attribute
{
	public TileTags[] Tags = tags;
}

public enum TileTags
{
	Indestructible,
	IndestructibleNoGround,
	VineSway,
	ChandelierSway,
	HarvestableHerb
}