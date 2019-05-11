using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public Text UnityAlloc;

    public Text NativeAlloc;

    public struct NativeTransform
    {
        public Vector3 localPosition;
        public Vector3 localScale;
    }

    public UnsafeList<NativeTransform> nativeTransformList;

    public void TestUnityAlloc()
    {
        float start = Time.realtimeSinceStartup;
        using (nativeTransformList = new UnsafeList<NativeTransform>(100))
        {
            
        }
        UnityAlloc.text = string.Format("耗时：{0}",Time.realtimeSinceStartup - start);
    }

    public void TestNativeAlloc()
    {
        
    }
}