using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace LD48
{
	/// <summary>
	/// 
	/// </summary>
	public class Inventory : MonoBehaviour
	{
		public const string ItemHoveredBoolField = "Item Hovered";
		public const string ItemCarriedBoolField = "Carrying Item";
		public const string HoveredItemChangedTrigger = "Item Changed";

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
		TextMeshProUGUI carryingItemLabel;

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
					UpdateHud(hoveredItem, value);
					hoveredItem = value;
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
					UpdateHud(hoveredInteraction, value);
					hoveredInteraction = value;
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

			// Play dropping item HUD animation
			interactiveHud.SetBool(ItemCarriedBoolField, false);

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

			// Update label, play picking up item HUD animation
			carryingItemLabel.text = Carrying.DisplayName;
			interactiveHud.SetBool(ItemCarriedBoolField, true);

			// Temporarily disable any actions
			lastSetAction = Time.time;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="oldItem"></param>
		/// <param name="newItem"></param>
		private void UpdateHud(Item oldItem, Item newItem)
		{
			// Set whether to show the hover information
			interactiveHud.SetBool(ItemHoveredBoolField, (newItem != null));

			// Check if there's an item
			if(newItem != null)
			{
				// Update text
				hoverLabel.text = newItem.DisplayName;

				// Check if hovered item simply changed
				if(oldItem != null)
				{
					// Indicate change
					interactiveHud.ResetTrigger(HoveredItemChangedTrigger);
					interactiveHud.SetTrigger(HoveredItemChangedTrigger);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="oldInteraction"></param>
		/// <param name="newInteraction"></param>
		private void UpdateHud(MonoBehaviour oldInteraction, MonoBehaviour newInteraction)
		{
			//FIXME: actually update the HUD

			// Check if we're carrying an item
			if(Carrying)
			{
				if(newInteraction == null)
				{
					// Show the drop item prompt if we're not going to interact with anything.
					interactiveHud.SetBool(ItemCarriedBoolField, true);
				}
				else if(newInteraction.enabled)
				{
					// Hide the drop item prompt if we could interact with something.
					// The interaction takes priority.
					interactiveHud.SetBool(ItemCarriedBoolField, false);
				}
			}
		}
	}
}
