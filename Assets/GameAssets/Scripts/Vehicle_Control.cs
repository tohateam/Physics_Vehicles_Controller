using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VehiclesControl
{
	#region Data **************************************************************************************
	[Serializable]
	public class Axle_Info
	{
		public GameObject leftWheel;
		public WheelCollider leftWheelCollider;
		[Space]
		public GameObject rightWheel;
		public WheelCollider rightWheelCollider;

		public bool motor;
		public bool steering;
	}
	#endregion

	//[RequireComponent (typeof (Rigidbody))]
	public class Vehicle_Control : MonoBehaviour
	{
		[Tooltip("Оси шасси")] public List<Axle_Info> axleInfos;

		public bool shoowGUI;

		public float CurrentSpeed { get; private set; }
		//public int MotorWheels { get; private set; }

		private Rigidbody m_RigidBody;
		private Vehicle_Engine m_Engine;
		private Vehicle_Gears m_Gears;

		private int motorWheelsCount = 0;
		private void Start()
		{
			m_RigidBody = GetComponentInChildren<Rigidbody>();
			if (!m_RigidBody) {
				Debug.LogError("Rigidbody is not attach in " + name);
				Destroy(this);
			}
			m_Engine = GetComponentInChildren<Vehicle_Engine>();
			if (!m_Engine) {
				Debug.LogError("Vehicle_Engine is not attach in " + name);
				Destroy(this);
			}
			m_Gears = GetComponentInChildren<Vehicle_Gears>();
			if (!m_Gears) {
				Debug.LogError("Vehicle_Gears is not attach in " + name);
				Destroy(this);
			}
			if (axleInfos.Count != 0) {
				GetMotorWheels();
			}
		}

		private void Update()
		{
			float steer = Input.GetAxis("Horizontal");
			float accel = Input.GetAxis("Vertical");
			float handbrake = Input.GetAxis("Jump");

			m_Engine.AccelerateEngine(accel);
			UpdateSpeed();
			CapSpeed();
			m_Engine.CurrentSpeed = CurrentSpeed;
		}

		private void CapSpeed()
		{
			if (CurrentSpeed > m_Gears.maxSpeed)
				m_RigidBody.velocity = (int)(m_Gears.maxSpeed / 3.6f) * m_RigidBody.velocity.normalized;

			//if (car.transmission.currentGear == -1) {
			//	float reverseSpeed = car.transmission.GetCurrentGearSpeedRang(); //speedRange[engine.transmission.currentGear];
			//	if (CurrentSpeed > reverseSpeed)
			//		m_RigidBody.velocity = (int)(reverseSpeed / 3.6f) * m_RigidBody.velocity.normalized;
			//}
		}

		private void UpdateSpeed()
		{
			CurrentSpeed = Mathf.Abs(GetCarSpeedVector3().z) * 3.6f;
			if (CurrentSpeed < 0.0001f)
				CurrentSpeed = 0;
		}
		private Vector3 GetCarSpeedVector3()
		{
			return transform.InverseTransformDirection(m_RigidBody.velocity);
		}

		private void GetMotorWheels()
		{
			if (axleInfos.Count != 0) {
				for (int i = 0; i < axleInfos.Count; i++) {
					if (axleInfos[i].motor)
						motorWheelsCount++;
				}
			}
		}

		public void OnGUI()
		{
			if (shoowGUI) {
				GUILayout.BeginArea(new Rect(20, 40, 200, 160), GUI.skin.window);
				GUILayout.Label("Speed: " + CurrentSpeed);
				GUILayout.Label("RPM: " + m_Engine.CurrentRpm);
				GUILayout.Label("Gear: " + m_Gears.CurrentGear);

				GUILayout.EndArea();
			}
		}

	}
}