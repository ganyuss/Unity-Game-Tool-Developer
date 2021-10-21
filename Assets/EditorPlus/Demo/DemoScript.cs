using System;
using System.Linq;
using EditorPlus;
using UnityEditor;
using UnityEngine;


public class DemoScript : MonoBehaviour {
    
    [HelpBox("Try setting this to \"42\"")]
    public string TextToSet;

    [ShowIf(nameof(TextToSet), "42")] 
    public InnerClass IAmHidden;
    
    public int[] IntValues;

#if UNITY_EDITOR
    [OnEditorGUI]
    private void SumInEditor() {
        int sum = IntValues?.Sum() ?? 0;
        EditorGUILayout.LabelField($"The sum of all the elements in the {nameof(IntValues)} list is {sum}");
    }
#endif

    [Button]
    public void SetText() {
        GetComponent<TextMesh>().text = TextToSet;
    }
    
    [Serializable]
    public class InnerClass {

        [Disabled]
        public int NoOneCanEditThis = 2;

        [CustomSpace(20, 20)]
        [MinMaxSlider(-42, 42)]
        public MinMaxInt INeedSomeSpace;

        [HideInPlayMode]
        public string LookAtMe;
        
        [HideInEditMode]
        public string SURPRISE = "SURPRISE";
        
        [DisableInEditMode]
        public string AndTryPressingPlay;

        [Button("Display List Of Subclasses Of Component")]
        private void DisplayListOfSubclassesOfComponent() {
            Type targetType = typeof(Component);
            Debug.Log($"All the sub-classes of {nameof(targetType)} are " +
                      $"{string.Join(", ", TypeUtils.GetAllTypesInheritingFrom(targetType).Select(t => t.Name))}");
        }
    }
}
