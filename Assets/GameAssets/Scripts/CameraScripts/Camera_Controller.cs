using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Скрипт присоединяется к объекту камера
namespace CameraControl {
    public class Camera_Controller : MonoBehaviour {
        [Tooltip ("Центр вращения камеры по оси Y.")] public Transform cameraPivot;
        [Space]
        [Tooltip ("Список позиций камеры по оси Z.")] public Vector3[] zoomCameraPos;
        [Tooltip ("Позиция камеры по умолчанию.")] public int defaultCameraPos = 0;
        [Space]
        [Tooltip (" Скорость вращения камеры в режиме обзора по оси Х.")] public float rotateSpeedNormalX = 5;
        [Tooltip (" Скорость вращения камеры в режиме обзора по оси Y.")] public float rotateSpeedNormalY = 1;
        [Space]
        [Tooltip ("Скорость вращения камеры в режиме зума по оси Х.")] public float rotateSpeedZoomX = 0.5f;
        [Tooltip ("Скорость вращения камеры в режиме зума по оси Y.")] public float rotateSpeedZoomY = 0.5f;
        [Space]
        [Tooltip ("Максимальный угол наклона камеры по оси Х.")] public float verticalMax = 40;
        [Tooltip ("Минимальный угол наклона камеры по оси Х")] public float verticalMin = -30;
        [Tooltip ("Увеличение в режиме зума")] public float magnifyFov = 10;

        private int currentCameralPos; // Текущая позиция
        private Quaternion currentCameralAngle; // Текущий наклон камеры

        private float rotateSpeedX; // Текщая скорость вращения камеры по оси Х
        private float rotateSpeedY; // Текщая скорость вращения камеры по оси Y
        private readonly float defaultFov = 60; // Увеличение по умолчанию
        private bool isZoom; // Флаг включения зума

        private Transform thisTransform;

        private void Awake () {
            thisTransform = this.transform;
        }

        void Start () {
            // Скрываем курсор
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            // Устанавливаем текущие скорости вращения
            rotateSpeedY = rotateSpeedNormalX;
            rotateSpeedX = rotateSpeedNormalY;
            // Камера в точку по дефолту
            currentCameralPos = defaultCameraPos;

            // Если не установлены позиции, то устанавливаем текущее положение камеры
            if (zoomCameraPos.Length != 0) {
                thisTransform.localPosition = zoomCameraPos[currentCameralPos];
            }
        }

        // Вращение камеры
        public void ControlCamera () {
            float mouseX = -Input.GetAxis ("Mouse Y");
            float mouseY = Input.GetAxis ("Mouse X");

            if ((thisTransform.localEulerAngles.x - 360) > -verticalMax || thisTransform.localEulerAngles.x < -verticalMin) {
                thisTransform.localEulerAngles += new Vector3 (rotateSpeedX * mouseX, 0, 0);
            }

            if (thisTransform.localEulerAngles.x < 100f && thisTransform.localEulerAngles.x >= -verticalMin) {
                thisTransform.localEulerAngles = new Vector3 (-verticalMin - 0.1f, thisTransform.localEulerAngles.y, 0);
            } else if (thisTransform.localEulerAngles.x > 300f && (thisTransform.localEulerAngles.x - 360f) <= -verticalMax) {
                thisTransform.localEulerAngles = new Vector3 (360f - verticalMax + 0.1f, thisTransform.localEulerAngles.y, 0);
            }

            cameraPivot.transform.localEulerAngles += new Vector3 (0, rotateSpeedY * mouseY, 0);
        }

        void Update () {
            // Нажата клавиша Ctrl, переключаем видимость курсора
            if (Input.GetKeyDown (KeyCode.LeftControl)) {
                Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked) ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = !Cursor.visible;
            }

            // Если курсор скрыт, то обрабатываем управление камерой
            if (!Cursor.visible) {
                // Нажата правая кнопка мыши, переключаем режим зума
                if (Input.GetMouseButtonDown (1)) {
                    if (zoomCameraPos.Length != 0) {
                        isZoom = !isZoom;
                        ToggleZoom ();
                    }
                }

                ControlCamera ();
            }
        }

        // Преключение режима зум
        private void ToggleZoom () {
            if (isZoom) { // Zoom On
                thisTransform.localPosition = zoomCameraPos[zoomCameraPos.Length - 1];
                //turret.cameraState = 1; // Флаг включения зума
                GetComponent<Camera> ().fieldOfView = magnifyFov;
                currentCameralPos = zoomCameraPos.Length - 1;

                rotateSpeedY = rotateSpeedZoomX;
                rotateSpeedX = rotateSpeedZoomY;
            } else { // Zoom Off
                rotateSpeedY = rotateSpeedNormalX;
                rotateSpeedX = rotateSpeedNormalY;

                GetComponent<Camera> ().fieldOfView = defaultFov;
                //turret.cameraState = 0;
                // Востанавливаем текущие координаты камеры
                thisTransform.localPosition = zoomCameraPos[defaultCameraPos];
                currentCameralPos = defaultCameraPos;
            }
        }
    }
}