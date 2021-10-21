using System;
using UnityEngine;


namespace EditorPlus {
    /// <summary>
    /// Can be used with <see cref="MinMaxSliderAttribute"/>
    /// </summary>
    [Serializable]
    public class MinMaxInt {
        public int Min;
        public int Max;
    }

    /// <summary>
    /// Can be used with <see cref="MinMaxSliderAttribute"/>
    /// </summary>
    [Serializable]
    public class MinMaxFloat {
        public float Min;
        public float Max;
    }
}
