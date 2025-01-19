using UnityEngine;

public static class Extend_TransformHelpers
{
    //하이어라키 창에 있는 오브젝트의 자식 오브젝트를 편하게 찾기 위해 만듬 
    public static Transform FindChildByName(this Transform transform, string name)
    {
        Transform[] transforms = transform.GetComponentsInChildren<Transform>();

        foreach (Transform t in transforms)
        {
            if (t.name.Equals(name))
                return t;
        }

        return null;
    }
}

//에셋 참조를 위해 상대 경로 변환 함수
#if UNITY_EDITOR
public static class DirectoryHelpers
{
    public static void ToRelativePath(ref string absolutePath)
    {
        int start = absolutePath.IndexOf("/Assets/");
        Debug.Assert(start > 0, "올바른 에셋 경로 아님");

        absolutePath = absolutePath.Substring(start + 1, absolutePath.Length - start - 1);
    }
}

//파일 이름 추출
public static class FileHelpers
{
    public static string GetFileName(string assetPath)
    {
        Debug.Assert(assetPath.Length > 0, "올바른 에셋 경로 아님");

        int end = assetPath.LastIndexOf('/');

        return assetPath.Substring(end + 1, assetPath.Length - end - 1);
    }
}
#endif

//카메라 관련 함수
public static class CameraHelpers
{
    //커서 위치 구하기
    public static bool GetCursorLocation(float distance, LayerMask mask)
    {
        Vector3 position;
        Vector3 normal;

        return GetCursorLocation(out position, out normal, distance, mask);
    }

    //커서 위치 구하기 (오버 로딩)
    public static bool GetCursorLocation(out Vector3 position,float distance, LayerMask mask)
    {
        Vector3 normal;

        return GetCursorLocation(out position, out normal, distance, mask);
    }

    //커서 위치 구하기 (오버 로딩)
    public static bool GetCursorLocation(out Vector3 position, out Vector3 normal, float distance, LayerMask mask)
    {
        position = Vector3.zero;
        normal = Vector3.zero;


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distance, mask))
        {
            position = hit.point;
            normal = hit.normal;
            return true;
        }

        return false;
    }
}