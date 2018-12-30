using UnityEditor;
using UnityEngine;

namespace VehicleTools {
    class VehicleSkeletonWizard : EditorWindow {
        int m_AxlesCount = 2;
        float m_Mass = 1000;
        float m_AxleStep = 2;
        float m_AxleWidth = 2;
        float m_AxleShift = -0.5f;

        bool posGroupEnabled = true;
        bool[] pos = new bool[3] { false, false, false };

        [MenuItem ("Tools/Vehicles/Create skeleton...")]
        public static void ShowWindow () {
            GetWindow (typeof (VehicleSkeletonWizard));
        }

        void OnGUI () {
            m_AxlesCount = EditorGUILayout.IntSlider ("Axles: ", m_AxlesCount, 2, 10);
            m_Mass = EditorGUILayout.FloatField ("Mass: ", m_Mass);
            EditorGUILayout.LabelField ("Расстояние между осями");
            m_AxleStep = EditorGUILayout.FloatField ("Axle step: ", m_AxleStep);
            EditorGUILayout.LabelField ("Ширина автомобиля");
            m_AxleWidth = EditorGUILayout.FloatField ("Axle width: ", m_AxleWidth);
            EditorGUILayout.LabelField ("Смещение осей вдоль локальной оси Y");
            m_AxleShift = EditorGUILayout.FloatField ("Axle shift: ", m_AxleShift);

            posGroupEnabled = EditorGUILayout.BeginToggleGroup ("Add scrips", posGroupEnabled);
            pos[0] = EditorGUILayout.Toggle ("EasySuspension", pos[0]);
            pos[1] = EditorGUILayout.Toggle ("WheelDrive", pos[1]);
            pos[2] = EditorGUILayout.Toggle ("Empty", pos[2]);
            EditorGUILayout.EndToggleGroup ();

            if (GUILayout.Button ("Generate")) {
                CreateCar ();
            }
        }

        void CreateCar () {
            var root = new GameObject ("carRoot");
            var rootBody = root.AddComponent<Rigidbody> ();
            rootBody.mass = m_Mass;

            var body = GameObject.CreatePrimitive (PrimitiveType.Cube);
            body.transform.parent = root.transform;

            float length = (m_AxlesCount - 1) * m_AxleStep;
            float firstOffset = length * 0.5f;

            body.transform.localScale = new Vector3 (m_AxleWidth, 1, length);

            for (int i = 0; i < m_AxlesCount; ++i) {
                var leftWheel = new GameObject (string.Format ("WheelCollider_{0}_L", i));
                var rightWheel = new GameObject (string.Format ("WheelCollider_{0}_R", i));

                leftWheel.AddComponent<WheelCollider> ();
                rightWheel.AddComponent<WheelCollider> ();

                leftWheel.transform.parent = root.transform;
                rightWheel.transform.parent = root.transform;

                leftWheel.transform.localPosition = new Vector3 (-m_AxleWidth * 0.5f, m_AxleShift, firstOffset - m_AxleStep * i);
                rightWheel.transform.localPosition = new Vector3 (m_AxleWidth * 0.5f, m_AxleShift, firstOffset - m_AxleStep * i);
            }

            if (pos[0]) {
                root.AddComponent<EasySuspension> ();
            }
            if (pos[1]) {
                root.AddComponent<WheelDrive> ();
            }
        }
    }
}