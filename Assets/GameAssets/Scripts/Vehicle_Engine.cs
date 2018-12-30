using UnityEngine;
using System.Collections;

namespace VehiclesControl
{
	// Двигатель КАМАЗ 740.60-360

	//[RequireComponent(typeof(Rigidbody))]
	[ExecuteInEditMode]
	public class Vehicle_Engine : MonoBehaviour
	{
		[Tooltip("Префаб маховика")] public Transform flywheel;
		[Tooltip("WheelCoolider маховика")] public WheelCollider flywheelCollider;
		[Space]
		[Tooltip("Мин. количество оборотов")] public float minEngineRpm = 500f;
		[Tooltip("Макс. количество оборотов")] public float maxEngineRpm = 1900f;
		[Tooltip("Обороты при которых достигается макс. мощьность")] public float maxEngintRpmTorque = 1300f;
		public AnimationCurve torque_Curve = new AnimationCurve(
			new Keyframe(0, 0),
			new Keyframe(1300, 1570),
			new Keyframe(1900, 1500)
		);
		public bool useCurve;
		[Space]
		public float maxEngineBrake = 30f;
		[Tooltip("Крутящих момент холостых оборотов")] public float engineIdleTorque = 10f;
		[Tooltip("Макс. крутящий момент")] public float maxEngineTorque = 1570;
		[Tooltip("Макс. мощьность двигателя (л/с)")] public float maxHP = 360f;

		public float CurrentRpm { get; private set; }
		public float CurrentSpeed { private get; set; }

		private Rigidbody m_RigidBody;
		private Vehicle_Gears m_Gears;

		private float currentTorque = 0;

		void Start()
		{
			if (!flywheel) {
				Debug.LogError("Game object 'flywheel' not attach in " + name);
				Destroy(this);
			}
			if (!flywheelCollider) {
				Debug.LogError("Game object 'flywheelCollider' not attach in " + name);
				Destroy(this);
			}
			m_RigidBody = GetComponentInChildren<Rigidbody>();
			if (!m_RigidBody) {
				Debug.LogError("Component 'm_RigidBody' not attach in " + name);
				Destroy(this);
			}
			m_Gears = GetComponentInParent<Vehicle_Gears>();
			if (!m_Gears) {
				Debug.LogError("Game object 'm_Gears' not attach in " + name);
				Destroy(this);
			}
		}

		void FixedUpdate()
		{
			flywheelCollider.motorTorque = currentTorque;
			flywheelCollider.brakeTorque = 0;
			UpdateCurrentRPM();

			VisualizeFlywheel();
		}

		float m_Accel = 0;
		public void AccelerateEngine(float accel)
		{
			m_Accel = accel;

			if (accel == 0) {
				currentTorque = engineIdleTorque;
			} else {
				//currentTorque = GetEngineTorque() * accel;
				currentTorque = maxEngineTorque * accel;
			}
			if (accel < 0) {
				currentTorque *= -1;
			}
		}


		public void VisualizeFlywheel()
		{
			Quaternion rot;
			Vector3 pos;
			flywheelCollider.GetWorldPose(out pos, out rot);
			flywheel.position = pos;
			flywheel.localRotation = rot;
		}

		private void UpdateCurrentRPM()
		{
			//float tmpRPM = CurrentSpeed * m_Gears.CurrentRatio * m_Gears.differentialRatio * (1000 / 60) / (flywheelCollider.radius * 2 * Mathf.PI);
			//CurrentRpm = Mathf.Lerp(CurrentRpm, tmpRPM, Time.deltaTime + 0.1f);

			CurrentRpm = 0.95f * CurrentRpm + 0.05f * Mathf.Abs(flywheelCollider.rpm);

			//if (CurrentRpm < minEngineRpm) CurrentRpm = minEngineRpm;
			if (CurrentRpm > maxEngineRpm) CurrentRpm = maxEngineRpm;
		}

		private float GetEngineTorque()
		{
			if (useCurve)
				return GetEngineTorque(CurrentRpm);
			return torque_Curve.Evaluate(CurrentRpm);
		}

		private float GetEngineTorque(float rpm)
		{
			if (rpm > maxEngineRpm) return 0;

			return maxHP * 5252 / rpm;
		}
		private float GetEngineBrakeTorque()
		{
			return Mathf.Lerp(0, maxEngineBrake, CurrentRpm / maxEngineRpm);
		}

		public float GetEngineWheelsBrakeTorque(int motorWheelsCount)
		{
			return GetEngineBrakeTorque() *  m_Gears.CurrentRatio * m_Gears.differentialRatio / motorWheelsCount;
		}

	}
}
