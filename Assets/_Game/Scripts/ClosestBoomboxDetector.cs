using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD48
{
	/// <summary>
	/// Moves the audio source to the closes boombox
	/// </summary>
	public class ClosestBoomboxDetector : MonoBehaviour
	{
		/// <summary>
		/// 
		/// </summary>
		static ClosestBoomboxDetector Instance
		{
			get;
			set;
		}

		[SerializeField]
		Transform musicPosition;

		/// <summary>
		/// 
		/// </summary>
		HashSet<Item> AllBoomboxes
		{
			get;
		} = new HashSet<Item>();
		/// <summary>
		/// 
		/// </summary>
		public Item ClosestBoombox
		{
			get;
			private set;
		} = null;

		/// <summary>
		/// Adds a boombox into the detector.
		/// </summary>
		/// <param name="boombox"></param>
		public static void AddBoombox(Item boombox)
		{
			if(Instance)
			{
				Instance.AllBoomboxes.Add(boombox);
			}
		}

		/// <summary>
		/// Start is called before the first frame update
		/// </summary>
		void Awake()
		{
			Instance = this;
			musicPosition.gameObject.SetActive(false);
		}

		/// <summary>
		/// 
		/// </summary>
		private void OnDestroy()
		{
			Instance = null;
		}

		/// <summary>
		/// Update is called once per frame
		/// </summary>
		void Update()
		{
			// Calculate the current boombox's distance (if set)
			float closestDistanceSqr = float.MaxValue, checkDistanceSqr;
			if(ClosestBoombox != null)
			{
				closestDistanceSqr = GetSqrDistance(ClosestBoombox);
			}

			// Go through all boomboxes
			foreach(var boombox in AllBoomboxes)
			{
				// Check if the boombox is different from the current one
				if(ClosestBoombox != null)
				{
					// Skip if the same boombox as the closest one
					if(ClosestBoombox == boombox)
					{
						continue;
					}

					// Otherwise calculate distance
					checkDistanceSqr = GetSqrDistance(boombox);
					if(checkDistanceSqr < closestDistanceSqr)
					{
						// Set the closes boombox
						ClosestBoombox = boombox;
						closestDistanceSqr = checkDistanceSqr;
					}
				}
				else
				{
					// closestBoombox is null
					// Set this boombox as the closest one
					ClosestBoombox = boombox;
					closestDistanceSqr = GetSqrDistance(ClosestBoombox);
				}
			}

			// Check if a boombox was found
			if(ClosestBoombox != null)
			{
				musicPosition.position = ClosestBoombox.transform.position;
				musicPosition.gameObject.SetActive(true);
			}
			else
			{
				musicPosition.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="boombox"></param>
		/// <returns></returns>
		float GetSqrDistance(Item boombox)
		{
			return Vector3.SqrMagnitude(boombox.transform.position - transform.position);
		}
	}
}
