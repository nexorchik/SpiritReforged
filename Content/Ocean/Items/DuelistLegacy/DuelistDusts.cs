namespace SpiritReforged.Content.Ocean.Items.DuelistLegacy;

public class DuelistBubble : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.frame = new Rectangle(0, 0, 10, 10);
		dust.scale = Main.rand.NextFloat(.8f, 1.2f);
		dust.alpha = 10;
		dust.rotation = Main.rand.NextFloat(6.28f);
		dust.velocity *= .1f;
	}

	public override bool Update(Dust dust)
	{
		Lighting.AddLight(dust.position, Color.Cyan.ToVector3() * .5f);
		dust.noGravity = true;
		dust.position += dust.velocity;
		dust.velocity.Y -= .05f;
		dust.scale *= .98f;
		dust.alpha += 5;
		
		if (dust.scale < .5f || dust.alpha >= 255)
			dust.active = false;

		return false;
	}
}

public class DuelistBubble2 : ModDust
{
	public override string Texture => base.Texture.Split("2")[0];

	public override Color? GetAlpha(Dust dust, Color lightColor) => Color.White * ((255 - dust.alpha) / 255f);

	public override void OnSpawn(Dust dust)
	{
		dust.frame = new Rectangle(0, 0, 10, 10);
		dust.scale = Main.rand.NextFloat(1.2f, 1.6f);
		dust.alpha = 10;
		dust.rotation = Main.rand.NextFloat(6.28f);
		dust.velocity *= .1f;
	}

	public override bool Update(Dust dust)
	{
		Lighting.AddLight(dust.position, Color.Cyan.ToVector3() * .5f);
		dust.noGravity = true;
		dust.position += dust.velocity;
		dust.velocity.Y -= .05f;
		dust.velocity.X *= .98f;
		dust.scale *= .98f;
		dust.alpha += 10;

		if (dust.scale < .5f || dust.alpha >= 255)
			dust.active = false;

		return false;
	}
}
public class DuelistSmoke : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.scale *= Main.rand.NextFloat(.8f, 2f);
		dust.frame = new Rectangle(0, 0, 34, 36);
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		var gray = new Color(25, 25, 25);
		Color orange = Color.Orange;
		Color ret;

		if (dust.alpha < 60)
			ret = Color.Lerp(Color.Yellow, orange, dust.alpha / 60f);
		else if (dust.alpha < 120)
			ret = Color.Lerp(orange, gray, (dust.alpha - 60) / 60f);
		else
			ret = gray;

		return ret * ((255 - dust.alpha) / 255f);
	}

	public override bool Update(Dust dust)
	{
		var gray = new Color(25, 25, 25);
		Color orange = Color.Orange;

		if (dust.alpha < 60)
			Lighting.AddLight(dust.position, Color.Lerp(new Color(255, 50, 180), orange, dust.alpha / 60f).ToVector3());
		else if (dust.alpha < 120)
			Lighting.AddLight(dust.position, Color.Lerp(orange, gray, (dust.alpha - 60) / 60f).ToVector3());

		if (dust.velocity.Length() > 3)
			dust.velocity *= .85f;
		else
			dust.velocity *= .92f;

		if (dust.alpha > 100)
		{
			dust.scale += .013f;
			dust.alpha += 6;
		}
		else
		{
			Lighting.AddLight(dust.position, dust.color.ToVector3() * .1f);
			dust.scale *= .97f;
			dust.alpha += 12;
		}

		dust.position += dust.velocity;
		if (dust.alpha >= 255)
			dust.active = false;

		return false;
	}
}
