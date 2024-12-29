using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Rendering;

using TMPro;

namespace MirzaBeig.CinematicExplosionsFree
{
    public class DemoController : MonoBehaviour
    {
        public enum Scene
        {
            Day,
            Night,
        }
        Camera camera;

        List<ParticleSystem> particleSystems;
        public Transform particleSystemsContainer;

        [Space]

        public Button buttonPrefab;
        public VerticalLayoutGroup buttonContainer;

        [Space]

        public Scene currentScene = Scene.Day;

        ReflectionProbe[] reflectionProbes;

        [Space]

        public GameObject environment;

        [Space]

        public int targetFrameRate = 60;

        void Start()
        {
            // Set scene.

            camera = Camera.main;
            Application.targetFrameRate = targetFrameRate;

            // Find all reflection probes.

            reflectionProbes = FindObjectsByType<ReflectionProbe>(
                FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            switch (currentScene)
            {
                case Scene.Day:
                    {
                        SetDay();

                        break;
                    }
                case Scene.Night:
                    {
                        SetNight();

                        break;
                    }
                default:
                    {
                        throw new System.Exception("Unknown case.");
                    }
            }

            // Assume all top-level children of container are particle systems.
            // Instantiate as many buttons as prefabs and link them.

            //particleSystems = new List<ParticleSystem> ();

            for (int i = 0; i < particleSystemsContainer.childCount; i++)
            {
                Transform childTransform = particleSystemsContainer.GetChild(i);

                // Exclude inactive.

                if (!childTransform.gameObject.activeSelf)
                {
                    continue;
                }

                ParticleSystem particleSystem = childTransform.GetComponent<ParticleSystem>();

                Button button = Instantiate(buttonPrefab, buttonContainer.transform);
                button.GetComponentInChildren<TextMeshProUGUI>().text = particleSystem.name;

                button.onClick.AddListener(() => childTransform.gameObject.SetActive(false));
                button.onClick.AddListener(() => childTransform.gameObject.SetActive(true));

                //particleSystems.Add(particleSystem);

                childTransform.gameObject.SetActive(false);
            }
        }

        void UpdateReflections()
        {
            for (int i = 0; i < reflectionProbes.Length; i++)
            {
                reflectionProbes[i].RenderProbe();
            }
        }
        void SetReflectionClearFlags(ReflectionProbeClearFlags clearFlags)
        {
            for (int i = 0; i < reflectionProbes.Length; i++)
            {
                reflectionProbes[i].clearFlags = clearFlags;
            }
        }

        // Set scene.

        public void SetNight()
        {
            currentScene = Scene.Night;

            camera.clearFlags = CameraClearFlags.SolidColor;

            RenderSettings.ambientIntensity = 0.8f;
            RenderSettings.reflectionIntensity = 0.5f;

            RenderSettings.sun.intensity = 0.0f;

            SetReflectionClearFlags(ReflectionProbeClearFlags.SolidColor);
            UpdateReflections();
        }

        public void SetDay()
        {
            currentScene = Scene.Day;

            camera.clearFlags = CameraClearFlags.Skybox;

            RenderSettings.ambientIntensity = 1.0f;
            RenderSettings.reflectionIntensity = 1.0f;

            RenderSettings.sun.intensity = 1.0f;

            SetReflectionClearFlags(ReflectionProbeClearFlags.Skybox);
            UpdateReflections();
        }

        public void SetEnvironmentActive(bool active)
        {
            environment.SetActive(active);
            UpdateReflections();
        }

        void Update()
        {
            // Toggle scenes.

            if (Input.GetKeyDown(KeyCode.F))
            {
                switch (currentScene)
                {
                    case Scene.Day:
                        {
                            SetNight();

                            break;
                        }
                    case Scene.Night:
                        {
                            SetDay();

                            break;
                        }
                    default:
                        {
                            throw new System.Exception("Unknown case.");
                        }
                }
            }

            // Toggle environment.

            if (Input.GetKeyDown(KeyCode.G))
            {
                SetEnvironmentActive(!environment.activeSelf);
            }
        }
    }
}
