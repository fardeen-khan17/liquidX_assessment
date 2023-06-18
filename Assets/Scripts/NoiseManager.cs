using UnityEngine;
using System;

public class NoiseManager : MonoBehaviour
{
    public static NoiseManager Instance { get; private set; }

    public event Action<Vector3> OnNoiseMade;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MakeNoise(Vector3 position)
    {
        OnNoiseMade?.Invoke(position);
    }
}
