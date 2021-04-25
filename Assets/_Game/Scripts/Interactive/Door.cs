using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD48
{
	/// <summary>
	/// 
	/// </summary>
	public class Door : IInteractable
	{
		/// <summary>
		/// 
		/// </summary>
		public enum State
		{
			OpenOnClick,
			Locked,
			Switch
		}
		const string OpenBoolField = "Is Open";

		[SerializeField]
		Animator animator;
		[SerializeField]
		State doorType;
		[SerializeField]
		[OmiyaGames.ReadOnly]
		bool isOpen = false;

		/// <summary>
		/// 
		/// </summary>
		public bool IsOpen
		{
			get => isOpen;
			set
			{
				if(isOpen != value)
				{
					isOpen = value;
					animator.SetBool(OpenBoolField, isOpen);
				}
			}
		}

		/// <inheritdoc/>
		public override bool OnHover(out HoverInfo info)
		{
			// FIXME: add a notification to the player they can open the door with a mouse click!
			info = null;
			return false;
		}

		/// <inheritdoc/>
		public override bool OnClick()
		{
			if(IsOpen == false)
			{
				if(doorType == State.OpenOnClick)
				{
					IsOpen = true;
					return true;
				}
				else if((doorType == State.Locked) && (Inventory.Instance?.Carrying?.ItemType == Item.Type.Key))
				{
					IsOpen = true;
					Inventory.Instance.DestroyCarryingItem();
					return true;
				}
			}
			return false;
		}
	}
}
