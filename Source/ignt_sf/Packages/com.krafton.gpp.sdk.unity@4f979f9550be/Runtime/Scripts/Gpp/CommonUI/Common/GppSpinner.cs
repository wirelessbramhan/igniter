using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gpp.CommonUI
{
    public class GppSpinner : MonoBehaviour
    {
        [SerializeField]
        private float rotationSpeed = 360;

        private RectTransform rect;
        // Start is called before the first frame update
        void Start()
        {
            rect = GetComponent<RectTransform>();
        }
        // Update is called once per frame
        void Update()
        {
            transform.Rotate(Vector3.back * rotationSpeed * Time.deltaTime);
        }
    }
}

