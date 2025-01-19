using UnityEngine;

public static class Extend_TransformHelpers
{
    //���̾��Ű â�� �ִ� ������Ʈ�� �ڽ� ������Ʈ�� ���ϰ� ã�� ���� ���� 
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

//���� ������ ���� ��� ��� ��ȯ �Լ�
#if UNITY_EDITOR
public static class DirectoryHelpers
{
    public static void ToRelativePath(ref string absolutePath)
    {
        int start = absolutePath.IndexOf("/Assets/");
        Debug.Assert(start > 0, "�ùٸ� ���� ��� �ƴ�");

        absolutePath = absolutePath.Substring(start + 1, absolutePath.Length - start - 1);
    }
}

//���� �̸� ����
public static class FileHelpers
{
    public static string GetFileName(string assetPath)
    {
        Debug.Assert(assetPath.Length > 0, "�ùٸ� ���� ��� �ƴ�");

        int end = assetPath.LastIndexOf('/');

        return assetPath.Substring(end + 1, assetPath.Length - end - 1);
    }
}
#endif

//ī�޶� ���� �Լ�
public static class CameraHelpers
{
    //Ŀ�� ��ġ ���ϱ�
    public static bool GetCursorLocation(float distance, LayerMask mask)
    {
        Vector3 position;
        Vector3 normal;

        return GetCursorLocation(out position, out normal, distance, mask);
    }

    //Ŀ�� ��ġ ���ϱ� (���� �ε�)
    public static bool GetCursorLocation(out Vector3 position,float distance, LayerMask mask)
    {
        Vector3 normal;

        return GetCursorLocation(out position, out normal, distance, mask);
    }

    //Ŀ�� ��ġ ���ϱ� (���� �ε�)
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