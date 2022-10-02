using UnityEngine;

namespace MPO
{
	public class TexturePropertyOverride : PropertyOverride<Texture>
	{
		public override PropertyType PropertyType => PropertyType.Texture;
		
		public override void Apply(MaterialPropertyBlock mpb)
		{
			if (Value != null)
				mpb.SetTexture(id, Value);
		}
	}
}