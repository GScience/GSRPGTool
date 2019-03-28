using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RPGTool.System
{
    public class Subwindow : MonoBehaviour
    {
        public CanvasGroup fader;
        public Canvas canvas;

        [HideInInspector]
        public bool result;
        [HideInInspector]
        public bool closed;

        void Awake()
        {

        }

        public void Close()
        {
            closed = true;
        }

        protected virtual void Update()
        {
            if (closed)
                Destroy(gameObject, 0.1f);
        }
    }
}
