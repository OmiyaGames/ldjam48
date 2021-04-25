using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;

namespace LD48
{
	/// <summary>
	/// 
	/// </summary>
	public class Inventory : MonoBehaviour
	{
		[Header("Raycast")]
		[SerializeField]
		float scanItemDistance = 5f;
		[SerializeField]
		LayerMask itemMasks = int.MaxValue;
		[SerializeField]
		string itemTag = "Item";
		[SerializeField]
		string interactiveTag = "Interactive";
		[SerializeField]
		float gapBetweenActionSeconds = 0.5f;

		[Header("HUD")]
		[SerializeField]
		Animator interactiveHud;
		[SerializeField]
		TextMeshProUGUI hoverLabel;
		[SerializeField]
		TextMeshProUGUI itemLabel;

		[Header("Debug")]
		[SerializeField]
		[OmiyaGames.ReadOnly]
		Item carrying = null;
		[SerializeField]
		[OmiyaGames.ReadOnly]
		Item hoveredItem = null;
		[SerializeField]
		[OmiyaGames.ReadOnly]
		MonoBehaviour hoveredInteraction = null;

		Ray rayCache = new Ray();
		RaycastHit hitCache;
		float lastSetAction = 0;

		/// <summary>
		/// Indicates the item being carried
		/// </summary>
		public Item Carrying
		{
			get => carrying;
			private set
			{
				// Check if anything changed
				if(carrying != value)
				{
					carrying = value;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Item HoveredItem
		{
			get => hoveredItem;
			private set
			{
				// Check if anything changed
				if(hoveredItem != value)
				{
					hoveredItem = value;
					UpdateHud();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public MonoBehaviour HoveredInteraction
		{
			get => hoveredInteraction;
			private set
			{
				if(hoveredInteraction != null)
				{
					hoveredInteraction = value;
					UpdateHud();
				}
			}
		}

		/// <summary>
		/// Update is called once per frame.
		/// </summary>
		void Update()
		{
			// Check if we're NOT in the gap period
			if((Time.time - lastSetAction) > gapBetweenActionSeconds)
			{
				// Update ray
				rayCache.origin = transform.position;
				rayCache.direction = transform.forward;

				// Raycast for an item
				if(Physics.Raycast(rayCache, out hitCache, scanItemDistance, itemMasks, QueryTriggerInteraction.Collide) == true)
				{
					// Check if we're carrying an item
					if(Carrying == null)
					{
						// If not, check if the detected object is really an item
						if(hitCache.collider.CompareTag(itemTag) == true)
						{
							HoveredItem = hitCache.collider.GetComponent<Item>();
							return;
						}
						else if((hitCache.rigidbody != null) && (hitCache.rigidbody.CompareTag(itemTag) == true))
						{
							HoveredItem = hitCache.collider.GetComponent<Item>();
							return;
						}
					}
				}
			}

			// Otherwise, indicate we're not hovering over anything
			HoveredItem = null;
		}

		/// <summary>
		/// 
		/// </summary>
		public void OnAction(InputAction.CallbackContext context)
		{
			// Listen for mouse up
			if(context.phase == InputActionPhase.Canceled)
			{
				// Check what the intended action is
				if((Carrying == null) && (HoveredItem != null))
				{
					PickUpItem();
				}
				else if(HoveredInteraction != null)
				{
					// FIXME: do something with the interaction!

					// Temporarily disable any actions
					lastSetAction = Time.time;
				}
				else if(Carrying != null)
				{
					DropItem();
				}
			}
			else if(Carrying != null)
			{
				Carrying.IsDropLocationVisible = (context.phase == InputActionPhase.Performed);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void DropItem()
		{
			// Drop the current item we're Carrying
			Carrying.CurrentState = Item.State.Idle;
			Carrying.IsDropLocationVisible = false;
			Carrying.transform.position = Carrying.DropLocation;
			Carrying.transform.SetParent(Carrying.OriginalParent, true);

			// Set the property to null
			Carrying = null;

			// Temporarily disable any actions
			lastSetAction = Time.time;
		}

		/// <summary>
		/// 
		/// </summary>
		private void PickUpItem()
		{
			// Transfer the variable over
			Carrying = HoveredItem;
			HoveredItem = null;

			// Setup the item as in-inventory
			Carrying.CurrentState = Item.State.InInventory;
			Carrying.transform.SetParent(transform, true);
			Carrying.transform.localPosition = Vector3.zero;
			Carrying.transform.localScale = Vector3.one;
			Carrying.transform.localRotation = Quaternion.identity;

			// Temporarily disable any actions
			lastSetAction = Time.time;
		}

		/// <summary>
		/// 
		/// </summary>
		private void UpdateHud()
		{
			//FIXME: actually update the HUD
		}
	}
}
