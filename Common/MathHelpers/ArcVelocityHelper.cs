namespace SpiritReforged.Common.MathHelpers;

public static class ArcVelocityHelper
{
	public static Vector2 GetArcVel(Vector2 startingPos, Vector2 targetPos, float gravity, float? minArcHeight = null, float? maxArcHeight = null, float? maxXvel = null, float? heightAboveTarget = null, float downwardsYVelMult = 1f)
	{
		Vector2 distToTravel = targetPos - startingPos;
		float maxHeight = distToTravel.Y - (heightAboveTarget ?? 0);

		if (minArcHeight != null)
			maxHeight = Math.Min(maxHeight, -(float)minArcHeight);

		if (maxArcHeight != null)
			maxHeight = Math.Max(maxHeight, -(float)maxArcHeight);

		float travelTime;
		float desiredYVel;

		if (maxHeight <= 0)
		{
			desiredYVel = -(float)Math.Sqrt(-2 * gravity * maxHeight);
			travelTime = (float)Math.Sqrt(-2 * maxHeight / gravity) + (float)Math.Sqrt(2 * Math.Max(distToTravel.Y - maxHeight, 0) / gravity); //time up, then time down
		}
		else
		{
			desiredYVel = Vector2.Normalize(distToTravel).Y * downwardsYVelMult;
			travelTime = (-desiredYVel + (float)Math.Sqrt(Math.Pow(desiredYVel, 2) - 4 * -distToTravel.Y * gravity / 2)) / gravity; //time down
		}

		if (maxXvel != null)
			return new Vector2(MathHelper.Clamp(distToTravel.X / travelTime, -(float)maxXvel, (float)maxXvel), desiredYVel);

		return new Vector2(distToTravel.X / travelTime, desiredYVel);
	}

	public static Vector2 GetArcVel(this Entity ent, Vector2 targetPos, float gravity, float? minArcHeight = null, float? maxArcHeight = null, float? maxXvel = null, float? heightabovetarget = null, float downwardsYVelMult = 1f) 
		=> GetArcVel(ent.Center, targetPos, gravity, minArcHeight, maxArcHeight, maxXvel, heightabovetarget, downwardsYVelMult);
}
