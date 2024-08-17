using System;
using UnityEngine;

public class Level : MonoBehaviour
{
    public event Action onCompleted;

    public void CompleteLevel()
    {
        onCompleted?.Invoke();
    }
}