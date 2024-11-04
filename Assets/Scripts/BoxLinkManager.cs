using System;
using UnityEngine;

[Serializable]
public class BoxLinkManager : MonoBehaviour
{
    [SerializeField]
    private bool isPastBox = false;
    [SerializeField]
    private GameObject futureBox;
    private Vector3 futurePos;

    public bool IsPastBox => isPastBox;
    public GameObject FutureBox => futureBox;

    // Добавим set-аксессор для FuturePos
    public Vector3 FuturePos
    {
        get => futurePos;
        set => futurePos = value;
    }

    private void Start()
    {
        if (isPastBox) futurePos = futureBox.transform.position;
    }
}