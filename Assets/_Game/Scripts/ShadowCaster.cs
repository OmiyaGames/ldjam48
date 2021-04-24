using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD48
{
	/// <summary>
	/// Repositions this mesh below the player
	/// </summary>
	[RequireComponent(typeof(MeshRenderer))]
	public class ShadowCaster : MonoBehaviour
	{
		[Header("Raycast")]
		[SerializeField]
		float maxRaycastDistance = 50f;
		[SerializeField]
		LayerMask raycastLayers = int.MaxValue;
		[SerializeField]
		float sphereRadius = 1f;

		[Header("Positioning")]
		[SerializeField]
		float distanceAboveSurface = 0.01f;

		Renderer rendererCache;
		Ray rayCache;
		RaycastHit hitCache;
		Vector3 positionCache = Vector3.zero;

		public Renderer Renderer => OmiyaGames.Helpers.GetComponentCached(this, ref rendererCache);
		Transform castFrom => transform.parent;

		private void Start()
		{
			rayCache = new Ray(castFrom.position, Vector2.down);
		}

		// Update is called once per frame
		void Update()
		{
			// Check if raycast hits anything
			rayCache.origin = castFrom.position;
			if(Physics.SphereCast(rayCache, sphereRadius, out hitCache, maxRaycastDistance, raycastLayers, QueryTriggerInteraction.Ignore) == true)
			{
				// Reposition this mesh
				positionCache = rayCache.origin;
				positionCache.y = hitCache.point.y + distanceAboveSurface;
				transform.position = positionCache;

				// Enable the mesh
				Renderer.enabled = true;
			}
			else
			{
				// Disable the mesh
				Renderer.enabled = false;
			}
		}
	}
}
