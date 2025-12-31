using System.Collections;
using ignt.sports.cricket.core;
using UnityEngine;
using UnityEngine.UI;

namespace com.krafton.fantasysports.UI
{
    [RequireComponent(typeof(PanelSwitchHandler))]
    public class GridView : MonoBehaviour
    {
        [SerializeField] private PageType pageType = PageType.none;
        [SerializeField] private GridDataSO viewData;
        [SerializeField] private bool showLayout = true, isLoaded = false;
        [SerializeField] private Color DefaultColor = Color.white;
        [SerializeField] private RectTransform layoutRoot;
        [SerializeField] private ViewElementBase[] allElements;

        void OnValidate()
        {
            if (gameObject.activeInHierarchy && gameObject.activeSelf)
            {
                DebugView();
            }
        }

        [ContextMenu("Init View")]
        private void DebugView()
        {
            isLoaded = false;
            Init();
        }

        /// <summary>
        /// Initialize method for Setup and config
        /// </summary>
        /// <param name="isLoaded"></param>
        private void Init()
        {
            if (!isLoaded)
            {
                Setup();
                Configure();
            }
        }

        /// <summary>
        /// Gets references and checks hierarchy (top down)
        /// </summary>
        protected void Setup()
        {
            layoutRoot = transform.GetChild(0).GetComponent<RectTransform>();
            layoutRoot.gameObject.name = "root_" + gameObject.name;

            if (viewData && layoutRoot)
            {
                ConfigureLayout(viewData, layoutRoot);
            }
        }

        /// <summary>
        /// Configures the element for the correct Data (if any)
        /// </summary>
        protected void Configure()
        {
            //Debug to check layout
            allElements = GetComponentsInChildren<ViewElementBase>();

            for (int i = 0; i < allElements.Length; i++)
            {
                
            }
        }

        /// <summary>
        /// Configures H/V Layout Group components
        /// </summary>
        /// <param name="config">Configuration SO</param>
        private void ConfigureLayout(GridDataSO config, RectTransform target)
        {
            if (config)
            {
                if (target.TryGetComponent<HorizontalOrVerticalLayoutGroup>(out var root))
                {
                    root.spacing = config.Spacing;
                    root.padding = config.Padding;

                    for (int i = 0; i < root.transform.childCount; i++)
                    {
                        var child = root.transform.GetChild(i);

                        if (child.TryGetComponent<HorizontalOrVerticalLayoutGroup>(out var section))
                        {
                            section.spacing = config.InnerSpacing;
                            section.padding = new RectOffset(0, 0, 0, 0);
                            section.gameObject.name = "section" + (i + 1);
                            
                            
                            ConfigureCells(child, showLayout);
                        }
                    }
                }

                isLoaded = true;
            }
        }

        /// <summary>
        /// Configures H/V Layout Group components
        /// </summary>
        /// <param name="config">Configuration SO</param>
        private IEnumerator ConfigureLayoutAsync(GridDataSO config, RectTransform target)
        {
            if (config)
            {
                if (target.TryGetComponent<HorizontalOrVerticalLayoutGroup>(out var root))
                {
                    root.spacing = config.Spacing;
                    root.padding = config.Padding;

                    for (int i = 0; i < root.transform.childCount; i++)
                    {
                        var child = root.transform.GetChild(i);

                        if (child.TryGetComponent<HorizontalOrVerticalLayoutGroup>(out var section))
                        {
                            section.spacing = config.InnerSpacing;
                            section.padding = new RectOffset(0, 0, 0, 0);
                            section.gameObject.name = "section" + (i + 1);
                            
                            
                            StartCoroutine(ConfigureCellsAsync(child, showLayout));
                        }
                    }
                }

                yield return null;
                isLoaded = true;
            }
        }


        /// <summary>
        /// Configures individual grid cells to show or hide layout
        /// </summary>
        /// <param name="section"></param>
        /// <param name="showLayout"></param>
        private bool ConfigureCells(Transform section, bool showLayout)
        {
            Color cellColor = Color.clear;

            if (showLayout)
            {
                cellColor = DefaultColor;
            }

            if (section.childCount > 0)
            {
                for (int i = 0; i < section.childCount; i++)
                {
                    if (section.GetChild(i).TryGetComponent<Image>(out var cell))
                    {
                        cell.color = cellColor;
                        cell.gameObject.name = "cell" + (i + 1);
                    }
                }
            }

            //Debug.Log("cells configured for " + gameObject.name);
            return true;
        }

        /// <summary>
        /// Configures individual grid cells to show or hide layout
        /// </summary>
        /// <param name="section"></param>
        /// <param name="showLayout"></param>
        private IEnumerator ConfigureCellsAsync(Transform section, bool showLayout)
        {
            yield return new WaitUntil(() => isLoaded);

            Color cellColor = Color.clear;

            if (showLayout)
            {
                cellColor = DefaultColor;
            }

            if (section.childCount > 0)
            {
                for (int i = 0; i < section.childCount; i++)
                {
                    if (section.GetChild(i).TryGetComponent<Image>(out var cell))
                    {
                        cell.color = cellColor;
                        cell.gameObject.name = "cell" + (i + 1);
                    }
                }
            }

            yield return null;
            Debug.Log("cells configured for " + gameObject.name);
        }

    }
}
