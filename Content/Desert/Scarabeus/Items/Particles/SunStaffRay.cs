using SpiritReforged.Content.Particles;

namespace SpiritReforged.Content.Desert.Scarabeus.Items.Particles;

public class SunStaffRay(Entity parent, Entity target, Vector2 position, Color color, float rotation, float scale, float maxDistortion, Vector2 texExponentRange, int maxTime) : DissipatingImage(position, color, rotation, scale, maxDistortion, "Godray", new Vector2(1), texExponentRange, maxTime)
{
	private readonly Entity _parent = parent;
	private readonly Entity _target = target;
	private readonly Vector2 _offset = position - parent.Center;

	public override void Update()
	{
		if (!_parent.active)
		{
			Kill();
			return;
		}

		base.Update();
		if (_target.active)
			Rotation = _parent.AngleTo(_target.Center) + MathHelper.PiOver2;

		Position = _parent.Center + new Vector2(0, -500).RotatedBy(Rotation) * _scaleMod * Scale;
	}
}
