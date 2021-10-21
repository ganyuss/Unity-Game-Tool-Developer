using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DebugKeyController : MonoBehaviour
{
    [Serializable]
    private struct KeyListener {
        public KeyCode KeyCode;
        public UnityEvent OnKeyPress;
    }

    [SerializeField]
    private List<KeyListener> keyListeners;

#if UNITY_EDITOR
    void Update() {
        foreach (KeyListener listener in keyListeners) {
            if (Input.GetKeyDown(listener.KeyCode)) {
                listener.OnKeyPress.Invoke();
            }
        }
    }
#endif
}
