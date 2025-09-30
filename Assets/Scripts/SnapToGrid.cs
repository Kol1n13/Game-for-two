using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class SnapToGrid : MonoBehaviour
{
    [Tooltip("Если пусто — будет найден первый Grid в сцене")]
    public Grid targetGrid;

    [Tooltip("Если хотите — перетащите Tilemap (Level1) сюда)")]
    public Tilemap targetTilemap;

    [Tooltip("Смещение если pivot спрайта не в центре")]
    public Vector3 pivotOffset = Vector3.zero;

    [Tooltip("Если true — при Snap объект станет дочерним у Grid (только в редакторе)")]
    public bool setParentToGrid = true;

    void OnEnable()
    {
        // ничего не делаем в PlayMode — чтобы не менять позицию во время игры
        if (!Application.isPlaying)
            EnsureGrid();
    }

    void EnsureGrid()
    {
        if (targetGrid == null)
            targetGrid = FindObjectOfType<Grid>();
    }

    [ContextMenu("Snap Now")]
    public void Snap()
    {
        if (targetGrid == null) return;

        Vector3 worldPos = transform.position - pivotOffset;
        Vector3Int cell = targetGrid.WorldToCell(worldPos);

        Vector3 snapped;
        if (targetTilemap != null)
            snapped = targetTilemap.GetCellCenterWorld(cell) + pivotOffset;
        else
            snapped = targetGrid.GetCellCenterWorld(cell) + pivotOffset;

        transform.position = snapped;

        #if UNITY_EDITOR
        if (setParentToGrid && targetGrid != null)
            transform.SetParent(targetGrid.transform, true);
        #endif
    }
}
