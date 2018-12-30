using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VehiclesControl
{
	//[RequireComponent(typeof(AudioSource))]
	public class AudioControl : MonoBehaviour
	{
		public AudioClip startEngineClip;
		public AudioClip idleEngineClip;
		public float minPitch = 0.82f;
		public float maxPitch = 1.6f;

		private Vehicle_Engine m_Engine;
		private AudioSource startAudioSource;
		private AudioSource idleAudioSource;

		private void Awake()
		{
			idleAudioSource = SetUpEngineAudioSource(idleEngineClip, true); ;
			if (!idleAudioSource) {
				Debug.LogError("AudioSource is not attached to" + name);
				Destroy(this);
			}

			if (!startEngineClip) {
				Debug.LogError("AudioClip 'Start Engine Clip' is not attached to" + name);
				Destroy(this);
			}
			if (!idleEngineClip) {
				Debug.LogError("AudioClip 'Idle Engine Clip' is not attached to" + name);
				Destroy(this);
			}

			m_Engine = GetComponentInChildren<Vehicle_Engine>();
			if (!m_Engine) {
				Debug.LogError("CarControl is not attached to" + name);
				Destroy(this);
			}
		}

		private void Start()
		{
			startAudioSource = SetUpEngineAudioSource(startEngineClip, false); ;
			if (!startAudioSource) {
				Debug.LogError("AudioSource is not attached to" + name);
				Destroy(this);
			} else {
				startAudioSource.Play();
			}
		}

		private void Update()
		{
			if (!startAudioSource.isPlaying && !idleAudioSource.isPlaying) {
				idleAudioSource.Play();
			}

			float pitch = ULerp(minPitch, maxPitch, m_Engine.CurrentRpm / m_Engine.maxEngineRpm);
			if (idleAudioSource.isPlaying) {
				idleAudioSource.pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
			}
		}

		private AudioSource SetUpEngineAudioSource(AudioClip clip, bool loop)
		{
			AudioSource source = gameObject.AddComponent<AudioSource>();
			source.clip = clip;
			source.volume = 1;
			source.loop = loop;
			source.time = Random.Range(0f, clip.length);
			return source;
		}

		private static float ULerp(float from, float to, float value)
		{
			return (1.0f - value) * from + value * to;
		}
	}
}
