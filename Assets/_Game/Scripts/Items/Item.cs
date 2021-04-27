using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD48
{
	/// <summary>
	/// 
	/// </summary>
	//[RequireComponent(typeof(Collider))]
	public class Item : MonoBehaviour
	{
		/// <summary>
		/// 
		/// </summary>
		public enum State
		{
			Idle,
			InInventory,
			Paused
		}
		/// <summary>
		/// 
		/// </summary>
		public enum Type
		{
			Other,
			Key,
			Boombox
		}
		/// <summary>
		/// 
		/// </summary>
		public Action<Item, State, State> OnStateChanged;

		[SerializeField]
		string displayName = "item";
		[SerializeField]
		Type itemType = Type.Other;

		[Header("Inventory State")]
		[SerializeField]
		GameObject worldObject;
		[SerializeField]
		GameObject inventoryObject;
		[SerializeField]
		GameObject dropLocation;
		[SerializeField]
		MonoBehaviour[] toggleScripts;

		[Header("Debug")]
		[SerializeField]
		[OmiyaGames.ReadOnly]
		State currentState = State.Idle;
		[SerializeField]
		Transform originalParent;

		bool isDropLocationVisible = false;
		bool? wasKinematic = null;
		Collider colliderCache;

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		public Vector3 DropLocation => dropLocation.transform.position;
		/// <summary>
		/// Grab the attached collider.
		/// </summary>
		public Collider Collider => OmiyaGames.Helpers.GetComponentCached(this, ref colliderCache);
		/// <summary>
		/// 
		/// </summary>
		public Type ItemType => itemType;
		/// <summary>
		/// 
		/// </summary>
		public string DisplayName => displayName;
		/// <summary>
		/// 
		/// </summary>
		public State CurrentState
		{
			get => currentState;
			set
			{
				if(currentState != value)
				{
					// Trigger event
					OnStateChanged?.Invoke(this, currentState, value);
					currentState = value;
					UpdateObjects();
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public bool IsDropLocationVisible
		{
			private get => isDropLocationVisible;
			set
			{
				if(isDropLocationVisible != value)
				{
					isDropLocationVisible = value;
					UpdateObjects();
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public Transform OriginalParent
		{
			get => originalParent;
			private set => originalParent = value;
		}
		#endregion

		/// <summary>
		/// Start is called before the first frame update
		/// </summary>
		void Start()
		{
			if(OriginalParent == null)
			{
				Reset();
			}
			if(itemType == Type.Boombox)
			{
				ClosestBoomboxDetector.AddBoombox(this);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void OnDestroy()
		{
			// Clean up any events
			OnStateChanged = null;
		}

		/// <summary>
		/// 
		/// </summary>
		private void Reset()
		{
			OriginalParent = transform.parent;
		}

		/// <summary>
		/// 
		/// </summary>
		private void UpdateObjects()
		{
			// First, check if wasKinematic is set
			if((wasKinematic == null) && Collider.attachedRigidbody)
			{
				// if not, set it now
				wasKinematic = Collider.attachedRigidbody.isKinematic;
			}

			// Check states
			if(CurrentState != State.InInventory)
			{
				SetupWorldMode();
			}
			else
			{
				SetupInventoryMode();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void SetupInventoryMode()
		{
			// Activate the inventory object
			inventoryObject.SetActive(true);

			// Check drop location flag
			dropLocation.SetActive(IsDropLocationVisible);

			// Make the rigidbody immobile
			if(Collider.attachedRigidbody)
			{
				Collider.attachedRigidbody.isKinematic = true;
			}

			// Deactivate everything else
			worldObject.SetActive(false);
			Collider.enabled = false;
			foreach(var script in toggleScripts)
			{
				script.enabled = false;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void SetupWorldMode()
		{
			// Activate the world object
			worldObject.SetActive(true);
			Collider.enabled = true;

			// Activate scripts
			foreach(var script in toggleScripts)
			{
				script.enabled = true;
			}

			// Update Rigidbody
			if(Collider.attachedRigidbody)
			{
				if(CurrentState == State.Paused)
				{
					// Make the rigidbody immobile
					Collider.attachedRigidbody.isKinematic = true;
				}
				else if(wasKinematic.HasValue)
				{
					// Revert the rigidbody
					Collider.attachedRigidbody.isKinematic = wasKinematic.Value;
				}
			}

			// Deactivate everything else
			inventoryObject.SetActive(false);
			dropLocation.SetActive(false);
		}
	}
}
