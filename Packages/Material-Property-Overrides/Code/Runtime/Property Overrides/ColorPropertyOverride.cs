using UnityEngine;

namespace MPO
{
	public class ColorPropertyOverride : PropertyOverride<Color>
	{
		public override PropertyType PropertyType => PropertyType.Color;
		
		public override void Apply(MaterialPropertyBlock mpb)
		{
			mpb.SetColor(id, Value);
		}
	}
}