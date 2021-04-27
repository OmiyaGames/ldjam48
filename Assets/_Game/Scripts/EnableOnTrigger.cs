using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace LD48
{
	/// <summary>
	/// 
	/// </summary>
	public class EnableOnTrigger : MonoBehaviour
	{
		[SerializeField]
		string checkTag = "Player";
		[SerializeField]
		ControllableTimeline script;
		
		private void OnTriggerEnter(Collider other)
		{
			if(other.CompareTag(checkTag))
			{
				script.enabled = true;
			}
		}
	}
}
