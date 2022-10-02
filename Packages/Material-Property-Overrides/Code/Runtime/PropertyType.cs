namespace MPO
{
	[System.Flags]
	public enum PropertyType
	{
		Number = 1 << 1,
		Color = 1 << 2,
		Vector = 1 << 3,
		Matrix = 1 << 4,
		Texture = 1 << 5,
	}
}