using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD48
{
	/// <summary>
	/// 
	/// </summary>
	public class Respawn : MonoBehaviour
	{
		[SerializeField]
		Transform spawnAt;
		[SerializeField]
		Rigidbody resetBody;

		[Header("Debug")]
		[SerializeField]
		[OmiyaGames.ReadOnly]
		Vector3 originalPosition;
		[SerializeField]
		[OmiyaGames.ReadOnly]
		Quaternion originalRotation;

		/// <summary>
		/// Start is called before the first frame update
		/// </summary>
		void Start()
		{
			originalPosition = transform.position;
			originalRotation = transform.rotation;
		}

		/// <summary>
		/// 
		/// </summary>
		private void Update()
		{
			if(transform.position.y < KillPlane.YPlane)
			{
				MoveToSpawnPoint();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		void Reset()
		{
			resetBody = GetComponent<Rigidbody>();
		}

		/// <summary>
		/// 
		/// </summary>
		private void MoveToSpawnPoint()
		{
			Vector3 spawnPosition = originalPosition;
			Quaternion spawnRotation = originalRotation;

			if(spawnAt)
			{
				spawnPosition = spawnAt.position;
				spawnRotation = spawnAt.rotation;
			}

			if(resetBody != null)
			{
				// Move the rigid body
				resetBody.MovePosition(spawnPosition);
				resetBody.MoveRotation(spawnRotation);

				// Remove all momentum
				resetBody.velocity = Vector3.zero;
			}
			else
			{
				// Move the transform
				transform.position = spawnPosition;
				transform.rotation = spawnRotation;
			}
		}
	}
}
