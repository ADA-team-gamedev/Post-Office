using System;

public interface IPickable
{
	public Action OnPickUpItem { get; set; }

	public Action OnDropItem { get; set; }
}
