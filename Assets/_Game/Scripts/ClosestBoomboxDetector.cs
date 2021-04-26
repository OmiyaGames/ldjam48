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
		static float lastBoomboxTime = 0f;
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

		AudioSource musicPlayer;

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
			musicPlayer = musicPosition.GetComponent<AudioSource>();

			// Initially make the audio disabled
			musicPosition.gameObject.SetActive(false);
		}

		/// <summary>
		/// 
		/// </summary>
		private void OnDestroy()
		{
			StopMusic();
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
				PlayMusic();
			}
			else if(musicPosition.gameObject.activeSelf)
			{
				StopMusic();
			}
		}

		private void PlayMusic()
		{
			// Check if this is the first time activating the music
			if(musicPosition.gameObject.activeSelf == false)
			{
				// Activate the game object
				musicPosition.gameObject.SetActive(true);

				// Start at a random point in the music
				musicPlayer.time = lastBoomboxTime;
				musicPlayer.Play();
			}
		}

		private void StopMusic()
		{
			// Check if audio has been activated
			if(musicPosition.gameObject.activeSelf == true)
			{
				// Track the last point of the music before stopping
				lastBoomboxTime = musicPlayer.time;
				musicPlayer.Stop();

				// Deactivate the game object
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
