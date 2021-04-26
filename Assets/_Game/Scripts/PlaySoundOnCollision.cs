using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD48
{
	/// <summary>
	/// 
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public class PlaySoundOnCollision : MonoBehaviour
	{
		[SerializeField]
		OmiyaGames.Audio.SoundEffect sfx;

		Collider cache;
		public Collider Collider => OmiyaGames.Helpers.GetComponentCached(this, ref cache);

		private void OnCollisionEnter(Collision collision)
		{
			// Confirm this collider isn't kinematic
			if(Collider.attachedRigidbody?.isKinematic == false)
			{
				sfx.Play();
			}
		}
	}
}