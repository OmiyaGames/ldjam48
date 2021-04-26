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

		public event System.Action<Door, bool> OnOpenChanged;
		public event System.Action<Door, HoverInfo> OnHovered;

		[SerializeField]
		Animator animator;
		[SerializeField]
		State doorType;
		[SerializeField]
		Rigidbody[] activateOnUnlock;
		[SerializeField]
		OmiyaGames.Audio.SoundEffect soundEffect;

		[Header("Hover Display")]
		[SerializeField]
		HoverInfo infoOnCanOpen;
		[SerializeField]
		HoverInfo infoOnLocked;
		[SerializeField]
		HoverInfo infoOnSwitch;

		[Header("Debug")]
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
					OnOpenChanged?.Invoke(this, isOpen);

					if(isOpen)
					{
						soundEffect.Play();
					}
				}
			}
		}

		/// <inheritdoc/>
		public override bool OnHover(Inventory source, out HoverInfo info)
		{
			// Setup default return value
			info = null;

			// Only show hover instructions if door is closed
			if(IsOpen == false)
			{
				// Check the door type
				switch(doorType)
				{
					case State.Locked:
						// Check if we're carrying a key
						if(Inventory.Instance?.Carrying?.ItemType == Item.Type.Key)
						{
							// Indicate we can open the door
							info = infoOnCanOpen;
						}
						else
						{
							// Otherwise, indicate the door is locked
							info = infoOnLocked;
						}
						OnHovered?.Invoke(this, info);
						return true;
					case State.Switch:
						info = infoOnSwitch;
						OnHovered?.Invoke(this, info);
						return true;
					default:
					case State.OpenOnClick:
						info = infoOnCanOpen;
						OnHovered?.Invoke(this, info);
						return true;
				}
			}
			return false;
		}

		/// <inheritdoc/>
		public override bool OnClick(Inventory source)
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

					// Destroy the key
					Inventory.Instance.DestroyCarryingItem();

					// Activate rigidbodies
					foreach(var body in activateOnUnlock)
					{
						body.isKinematic = false;
					}
					return true;
				}
			}
			return false;
		}
	}
}
