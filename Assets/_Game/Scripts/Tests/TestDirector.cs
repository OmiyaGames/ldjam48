using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace LD48
{
	public class TestDirector : MonoBehaviour
	{
		const KeyCode PlayCode = KeyCode.E;
		const KeyCode RewindCode = KeyCode.Q;

		enum State
		{
			Paused,
			Playing,
			Rewinding
		}

		[SerializeField]
		PlayableDirector timeline;

		State state { get; set; } = State.Paused;

		// Start is called before the first frame update
		void Start()
		{
			timeline.time = 0;
			timeline.Evaluate();
			state = State.Paused;
		}

		// FIXME: use the new input system, instead of the old
		void Update()
		{
			switch(state)
			{
				case State.Paused:
					if(Input.GetKey(PlayCode) == true)
					{
						state = State.Playing;
					}
					else if(Input.GetKey(RewindCode) == true)
					{
						state = State.Rewinding;
					}
					break;
				case State.Playing:
					if(Input.GetKey(PlayCode) == false)
					{
						state = State.Paused;
					}
					break;
				case State.Rewinding:
					if(Input.GetKey(RewindCode) == false)
					{
						state = State.Paused;
					}
					break;
			}
		}

		void FixedUpdate()
		{
			double newTime = timeline.time;
			switch(state)
			{
				case State.Playing:
					newTime += Time.deltaTime;
					if(newTime > timeline.playableAsset.duration)
					{
						newTime = timeline.playableAsset.duration;
					}
					timeline.time = newTime;
					timeline.Evaluate();
					break;
				case State.Rewinding:
					newTime -= Time.deltaTime;
					if(newTime < 0)
					{
						newTime = 0;
					}
					timeline.time = newTime;
					timeline.Evaluate();
					break;
			}
		}
	}
}
