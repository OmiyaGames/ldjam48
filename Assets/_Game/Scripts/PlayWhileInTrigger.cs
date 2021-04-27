using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace LD48
{
	/// <summary>
	/// 
	/// </summary>
	public class PlayWhileInTrigger : MonoBehaviour
	{
		enum PlayState : byte
		{
			Waiting,
			Playing,
			Paused
		}

		[SerializeField]
		bool triggerOnlyWhenHoldingKey = false;
		[SerializeField]
		string checkColliderTag = "Player";
		[SerializeField]
		PlayableDirector timeline;
		[SerializeField]
		float rampUpMagnitude = 10f;
		[SerializeField]
		float padPausing = 1f;

		float timeLastPaused = 0f;

		/// <summary>
		/// 
		/// </summary>
		bool IsPlaying
		{
			get;
			set;
		} = false;

		/// <summary>
		/// 
		/// </summary>
		float TargetSpeed
		{
			get;
			set;
		} = 0f;

		private void Start()
		{
			// Pause the timeline
			timeline.Play();
			timeline.playableGraph.GetRootPlayable(0).SetSpeed(0);
			IsPlaying = false;
			TargetSpeed = 0;

			// Fix time
			timeLastPaused = (padPausing * -2f);
		}

		private void Update()
		{
			if((IsPlaying == true) || ((Time.time - timeLastPaused) < padPausing))
			{
				// Ramp up speed
				if(TargetSpeed < 1f)
				{
					TargetSpeed += (Time.deltaTime * rampUpMagnitude);
				}

				// Clamp speed
				if(TargetSpeed > 1f)
				{
					TargetSpeed = 1f;
				}
			}
			else
			{
				// Slow down up speed
				if(TargetSpeed > 0f)
				{
					TargetSpeed -= (Time.deltaTime * rampUpMagnitude);
				}

				// Clamp speed
				if(TargetSpeed < 0f)
				{
					TargetSpeed = 0f;
				}
			}

			// Apply speed
			timeline.playableGraph.GetRootPlayable(0).SetSpeed(TargetSpeed);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		private void OnTriggerEnter(Collider other)
		{
			if(other.CompareTag(checkColliderTag)
				&& ((triggerOnlyWhenHoldingKey == false)
				|| (Inventory.Instance.Carrying?.ItemType == Item.Type.Key)))
			{
				IsPlaying = true;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		private void OnTriggerStay(Collider other)
		{
			if(triggerOnlyWhenHoldingKey == false)
			{
				return;
			}

			if(other.CompareTag(checkColliderTag)
				&& (Inventory.Instance.Carrying?.ItemType == Item.Type.Key))
			{
				IsPlaying = true;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		private void OnTriggerExit(Collider other)
		{
			if(other.CompareTag(checkColliderTag))
			{
				IsPlaying = false;
				timeLastPaused = Time.time;
			}
		}
	}
}
