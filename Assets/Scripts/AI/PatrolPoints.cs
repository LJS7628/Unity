using UnityEngine;
using UnityEngine.UIElements;

public class PatrolPoints : MonoBehaviour
{
    [Header(" - WayPoint Settings")]
    [SerializeField]
    private bool bLoop;

    [SerializeField]
    private bool bReverse;

    [SerializeField]
    private int toIndex;

    [Header(" - Draw Settings")]
    [SerializeField]
    private float drawHeight = 0.1f;

    [SerializeField]
    private Color drawSphereColor = Color.green;

    [SerializeField]
    private Color drawLineColor =Color.magenta;

    public Vector3 GetMoveToPosition() 
    {
        Debug.Assert(toIndex >=0 && toIndex < transform.childCount);

        return transform.GetChild(toIndex).position;
    }

    //경로 업데이트
    public void UpdateNextIndex() 
    { 
        int count = transform.childCount;

        if (bReverse)
        {
            if (toIndex > 0) 
            {
                toIndex--;
                return;
            }

            if (bLoop) 
            {
                toIndex = count - 1;
                return;
            }

            bReverse = false;
            toIndex = 1;
            return ;
        }

        if (toIndex < count - 1) 
        {
            toIndex++;

            return;
        }

        if (bLoop) 
        {
            toIndex = 0;
             return;
        }

        bReverse = true;
        toIndex = count - 2;

    }
    private void OnDrawGizmos()
    {
        Gizmos.color =new Color(1,0,1,0.75f);
        Gizmos.DrawSphere(transform.position,0.3f);


        int count = transform.childCount;

        if(count <2)
            return;

        DrawSphere(0, new Color(0, 0, 1, 0.75f),0.3f);
        DrawSphere(count -1, new Color(1, 0, 0, 0.75f),0.3f);

        for (int i = 0; i < count; i++)
        {
            DrawSphere(i,drawSphereColor,0.15f);

            if(i<count-1)
                DrawLine(i,i+1);    
        }

        if(bLoop)   
            DrawLine(count-1,0);
    }

    //경로 지점 보여주기
    private void DrawSphere(int index, Color color, float size) 
    {
        Vector3 position = transform.GetChild(index).position+new Vector3(0,drawHeight,0);

        Gizmos.color= color;
        Gizmos.DrawSphere(position, size);
    }

    //경로 보여주기
    private void DrawLine(int startIndex, int endIndex)
    {
        Transform start = transform.GetChild(startIndex);
        Transform end = transform.GetChild(endIndex);

        Vector3 startPosition = start.position + new Vector3(0,drawHeight,0);
        Vector3 endPosition = end.position + new Vector3(0, drawHeight, 0);

        Gizmos.color = drawSphereColor;
        Gizmos.DrawLine(startPosition, endPosition);
    }
}
