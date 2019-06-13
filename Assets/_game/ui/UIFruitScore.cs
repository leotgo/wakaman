using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wakaman.UI
{
    public class UIFruitScore : MonoBehaviour
    {
        [SerializeField] private float displayTime = 1.5f;
        private float currTime;

        private void Awake()
        {
            currTime = 0f;
        }

        private void Update()
        {
            currTime += Time.deltaTime;
            if (currTime > displayTime)
                gameObject.SetActive(false);
        }
    }
}
