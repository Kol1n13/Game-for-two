using System;
using UnityEngine;

[Serializable]
public class BoxLinkManager : MonoBehaviour
{
    [SerializeField]
    private bool isPastBox = false;
    [SerializeField]
    private GameObject futureBox;
    public bool IsPastBox => isPastBox;
    public GameObject FutureBox => futureBox;
}