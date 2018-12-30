using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CameraControl {
	public class Camera_Switch : MonoBehaviour {
		public GameObject[] objects;
		public Text text;

		private int m_CurrentActiveObject = 0;

		private void OnEnable () {
			if (text)
				text.text = objects[m_CurrentActiveObject].name;
		}

		private void Update () {
			if (Input.GetKey (KeyCode.Tab)) {
				NextCamera ();
			}
			// Нажата клавиша Ctrl, переключаем видимость курсора
			if (Input.GetKeyDown (KeyCode.LeftControl)) {
				Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked) ? CursorLockMode.None : CursorLockMode.Locked;
				Cursor.visible = !Cursor.visible;
			}
		}

		public void NextCamera () {
			int nextactiveobject = m_CurrentActiveObject + 1 >= objects.Length ? 0 : m_CurrentActiveObject + 1;

			for (int i = 0; i < objects.Length; i++) {
				objects[i].SetActive (i == nextactiveobject);
			}

			m_CurrentActiveObject = nextactiveobject;
			if (text)
				text.text = objects[m_CurrentActiveObject].name + " - " + m_CurrentActiveObject;
		}
	}
}