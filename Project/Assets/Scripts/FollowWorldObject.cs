using UnityEngine;

public class FollowWorldObject : MonoBehaviour
{
    public Transform target;
    public Vector3 worldOffset = Vector3.zero;
    public Vector2 screenOffset = Vector2.zero;

    private Camera mainCamera;
    private RectTransform rectTransform;

    void Start()
    {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
        
        if (target == null)
        {
            // 如果没有指定目标，就跟随创建时的世界位置
            target = new GameObject("DummyTarget").transform;
            target.position = transform.position;
        }
    }

    void Update()
    {
        if (mainCamera != null && target != null)
        {
            // 将世界坐标转换为屏幕坐标
            Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position + worldOffset);
            
            // 应用屏幕偏移
            screenPos.x += screenOffset.x;
            screenPos.y += screenOffset.y;
            
            rectTransform.position = screenPos;
        }
    }

    void OnDestroy()
    {
        // 清理临时创建的目标对象
        if (target != null && target.name == "DummyTarget")
        {
            Destroy(target.gameObject);
        }
    }
}