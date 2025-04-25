
namespace SpiritReforged.Common.WorldGeneration.Micropasses.Passes.CaveEntrances;

internal class ArchwayEntrance : CaveEntrance
{
	public override CaveEntranceType Type => CaveEntranceType.Archways;

	public override void Generate(int x, int y)
	{
		WormDig(x, y);
	}

	private void WormDig(int x, int y)
	{

	}

	public override bool ModifyOpening(ref int x, ref int y, bool isCavinator) => isCavinator;
}
