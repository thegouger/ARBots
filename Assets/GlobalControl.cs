using UnityEngine;
using TechTweaking.Bluetooth;

public class GlobalControl : MonoBehaviour
{
    public static GlobalControl Instance;
    public BluetoothDevice btDevice;

    void Awake()
    {

        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
}