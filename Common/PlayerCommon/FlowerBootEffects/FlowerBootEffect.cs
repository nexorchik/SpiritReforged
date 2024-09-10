namespace SpiritReforged.Common.PlayerCommon.FlowerBootEffects;

internal abstract class FlowerBootEffect : ModType
{
	protected sealed override void Register() => ModTypeLookup<FlowerBootEffect>.Register(this);

	/// <summary>
	/// Checks if <see cref="PlaceOn(int, int, Player)"/> can be called for the given tile. This tile is always the ground under what will be spawned.
	/// </summary>
	/// <param name="x">X coordinate.</param>
	/// <param name="y">Y coordinate of the ground.</param>
	/// <param name="player">Player that is running this.</param>
	/// <returns>If this can place flowers.</returns>
	public abstract bool CanPlaceOn(int x, int y, Player player);

	/// <summary>
	/// Run actual placement effects. This is run ABOVE the tile checked in <see cref="CanPlaceOn(int, int, Player)"/>.<br/>
	/// This is always guarded by the following check:<br/>
	/// <c>tile.HasTile AND item.CanPlaceOn(x, y + 1, Player) AND !current.HasTile AND current.LiquidAmount == 0</c><br/>
	/// If this does not fit desired parameters, use <see cref="RunManually(int, int, Player, out bool)"/>.<br/><br/>
	/// If this returns true, the following will automatically be run:
	/// <code>
	/// tile.CopyPaintAndCoating(Main.tile[x, y + 1]);
	///	if (Main.netMode == NetmodeID.MultiplayerClient)
	///		NetMessage.SendTileSquare(-1, x, y);
	/// </code><br/>
	/// Basically, the newly placed tile will copy the paint of the ground and sync.
	/// </summary>
	/// <param name="x">X coordinate.</param>
	/// <param name="y">Y coordinate above the ground.</param>
	/// <param name="player">The player that is running this.</param>
	/// <returns>If a tile was successfully placed.</returns>
	public virtual bool PlaceOn(int x, int y, Player player) => false;

	/// <summary>
	/// Runs before all other checks.<br/>
	/// Return false to NOT override behaviour.
	/// Return true to override behaviour and stop <see cref="FlowerBootsPlayer.CheckAllEffects(int, int)"/> from continuing.
	/// Return null to override behaviour but only skip this flower boot effect.<br/>
	/// </summary>
	/// <param name="x">X postion.</param>
	/// <param name="y">Y position ABOVE the ground.</param>
	/// <param name="player">The player running this.</param>
	/// <returns>If this should override default behaviour and how to function if not successful.</returns>
	public virtual bool? RunManually(int x, int y, Player player) => false;
}
