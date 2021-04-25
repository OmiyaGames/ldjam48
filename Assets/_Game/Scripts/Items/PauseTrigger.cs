using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD48
{
	/// <summary>
	/// 
	/// </summary>
	public class PauseTrigger : MonoBehaviour, IDisposable
	{
		/// <summary>
		/// 
		/// </summary>
		private struct ItemInfo
		{
			/// <summary>
			/// 
			/// </summary>
			public Item item;
			/// <summary>
			/// 
			/// </summary>
			public Collider collider;
		}

		readonly Dictionary<Collider, InteractiveTimelineObject> timelineObjects = new Dictionary<Collider, InteractiveTimelineObject>();
		readonly Dictionary<Collider, Rigidbody> interactiveObjects = new Dictionary<Collider, Rigidbody>();
		readonly Dictionary<Collider, ItemInfo> collectedItems = new Dictionary<Collider, ItemInfo>();
		readonly Dictionary<Item, ItemInfo> itemToInfoMap = new Dictionary<Item, ItemInfo>();
		Action<Item, Item.State, Item.State> stateChangedListener;

		/// <summary>
		/// 
		/// </summary>
		public Action<Item, Item.State, Item.State> StateChangedListener
		{
			get
			{
				if(stateChangedListener == null)
				{
					stateChangedListener = new Action<Item, Item.State, Item.State>(OnCollectedItemStateChanged);
				}
				return stateChangedListener;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			// Revert all objects
			foreach(var revertObject in timelineObjects.Values)
			{
				revertObject.IsPaused = false;
			}
			foreach(var revertObject in collectedItems.Values)
			{
				CleanUpItem(revertObject.item);
				revertObject.item.CurrentState = Item.State.Idle;
			}
			foreach(var revertObject in interactiveObjects.Values)
			{
				revertObject.isKinematic = false;
			}

			// Clear all lists
			timelineObjects.Clear();
			interactiveObjects.Clear();
			collectedItems.Clear();
			itemToInfoMap.Clear();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		private void OnTriggerStay(Collider other)
		{
			// Make sure we haven't cached this collider yet
			if((timelineObjects.ContainsKey(other) == false) && (collectedItems.ContainsKey(other) == false) && (interactiveObjects.ContainsKey(other) == false))
			{
				// Grab this collider's rigidbody (if there's any)
				Rigidbody body = other.attachedRigidbody;
				if(body != null)
				{
					// Check if this rigidbody has a script
					var timelineObject = body.GetComponent<InteractiveTimelineObject>();
					if(timelineObject != null)
					{
						// Pause the script, and add to the reference list
						timelineObject.IsPaused = true;
						timelineObjects.Add(other, timelineObject);
						return;
					}

					// Check if this rigidbody has a script
					var itemObject = body.GetComponent<Item>();
					if((itemObject != null) && (itemObject.CurrentState == Item.State.Idle))
					{
						CollectNewItem(other, itemObject);
						return;
					}

					// Otherwise, is this rigidbody moving?
					if(body.isKinematic == false)
					{
						// Freeze the rigidbody, and add to the reference list
						body.isKinematic = true;
						interactiveObjects.Add(other, body);
						return;
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		private void OnTriggerExit(Collider other)
		{
			// Check if we've cached this collider
			if(timelineObjects.ContainsKey(other) == true)
			{
				// Revert this object
				timelineObjects[other].IsPaused = false;
				timelineObjects.Remove(other);
			}
			else if(collectedItems.ContainsKey(other) == true)
			{
				// Cleanup info
				CleanUpInfo(collectedItems[other]);
			}
			else if(interactiveObjects.ContainsKey(other) == true)
			{
				// Revert this object
				interactiveObjects[other].isKinematic = false;
				interactiveObjects.Remove(other);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void OnDisable()
		{
			Dispose();
		}
		
		/// <summary>
		/// 
		/// </summary>
		private void OnDestroy()
		{
			Dispose();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <param name="_"></param>
		/// <param name="newState"></param>
		void OnCollectedItemStateChanged(Item item, Item.State _, Item.State newState)
		{
			ItemInfo info;
			if((newState != Item.State.Paused) && (itemToInfoMap.TryGetValue(item, out info) == true))
			{
				// Cleanup info
				CleanUpInfo(info);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <param name="itemObject"></param>
		private void CollectNewItem(Collider other, Item itemObject)
		{
			// Gather info on the item
			ItemInfo info = new ItemInfo()
			{
				item = itemObject,
				collider = other
			};

			// Listen to the item state changing
			itemObject.OnStateChanged += StateChangedListener;

			// Pause the script, and add to the reference list
			itemObject.CurrentState = Item.State.Paused;
			collectedItems.Add(other, info);
			itemToInfoMap.Add(itemObject, info);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		void CleanUpInfo(ItemInfo info)
		{
			// Cleanup item
			CleanUpItem(info.item);

			// Revert the item to Idle
			info.item.CurrentState = Item.State.Idle;

			// Remove the item from the respective maps
			collectedItems.Remove(info.collider);
			itemToInfoMap.Remove(info.item);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		void CleanUpItem(Item item)
		{
			// Remove this event listener
			item.OnStateChanged -= StateChangedListener;
		}
	}
}
