using UnityEngine;
using System.Collections;

public class Vehicle_Gears : MonoBehaviour
{
	public float maxSpeed = 90;
	public float maxBackSpeed = 10;

	public float[] gearsRatio = { 4.31f, 1.0f, 2.71f, 1.88f, 1.41f, 1.13f, 0.93f };
	public float differentialRatio = 1f;
	public float shiftSpeed = 0.05f;

	public int CurrentGear { get; private set; }
	public float CurrentRatio { get; private set; }

	void Start()
	{
		CurrentGear = 1;
		CurrentRatio = gearsRatio[CurrentGear];
	}


	void Update()
	{

	}
}
