using UnityEngine;

namespace MPO
{
	public class BoolPropertyOverride : PropertyOverride<bool>
	{
		public override PropertyType PropertyType => PropertyType.Number;
		
		public override void Apply(MaterialPropertyBlock mpb)
		{
			mpb.SetInt(id, Value ? 1 : 0);
		}
	}
}