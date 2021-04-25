using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD48
{
	/// <summary>
	/// 
	/// </summary>
	public class KillPlane : MonoBehaviour
	{
		/// <summary>
		/// The kill plane instance.
		/// </summary>
		public static KillPlane Instance
		{
			get;
			private set;
		} = null;

		/// <summary>
		/// The Y-Axis position of the Kill Plane
		/// </summary>
		public static float YPlane
		{
			get => (Instance ? Instance.transform.position.y : float.MinValue);
		}

		private void Awake()
		{
			Instance = this;
		}

		private void OnDestroy()
		{
			Instance = null;
		}
	}
}
