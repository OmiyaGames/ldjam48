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
		public Action<Item, State, State> OnStateChanged;

		[SerializeField]
		string displayName = "item";

		[Header("Inventory State")]
		[SerializeField]
		GameObject worldObject;
		[SerializeField]
		GameObject inventoryObject;
		[SerializeField]
		GameObject dropLocation;

		[Header("Debug")]
		[SerializeField]
		[OmiyaGames.ReadOnly]
		State currentState = State.Idle;
		[SerializeField]
		Transform originalParent;

		bool isDropLocationVisible = false;
		Collider colliderCache;

		/// <summary>
		/// 
		/// </summary>
		public Vector3 DropLocation => dropLocation.transform.position;

		/// <summary>
		/// Grab the attached collider.
		/// </summary>
		Collider Collider => OmiyaGames.Helpers.GetComponentCached(this, ref colliderCache);

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
		///  Start is called before the first frame update
		/// </summary>
		void Start()
		{
			// FIXME: anything to do
			if(originalParent == null)
			{
				Reset();
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
			originalParent = transform.parent;
		}

		/// <summary>
		/// 
		/// </summary>
		private void UpdateObjects()
		{
			// Check states
			if(CurrentState != State.InInventory)
			{
				// Activate the world object
				worldObject.SetActive(true);
				Collider.enabled = true;

				// Deactivate everything else
				inventoryObject.SetActive(false);
				dropLocation.SetActive(false);
			}
			else
			{
				// Activate the inventory object
				inventoryObject.SetActive(true);
				Collider.enabled = false;

				// Check drop location flag
				dropLocation.SetActive(IsDropLocationVisible);

				// Deactivate everything else
				worldObject.SetActive(false);
			}
		}
	}
}
