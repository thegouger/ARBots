using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TechTweaking.Bluetooth;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BluetoothMenu : MonoBehaviour
{
	public Text devicNameText;	
    public BluetoothDevice btDevice;

	void Awake ()
	{
        //Ask user to enable Bluetooth
        BluetoothAdapter.askEnableBluetooth ();

		BluetoothAdapter.OnDeviceOFF += HandleOnDeviceOff;

        //To get what device the user picked out of the devices list
        BluetoothAdapter.OnDevicePicked += HandleOnDevicePicked;

       // BluetoothAdapter.OnConnected += LoadGame;
	}

    private void LoadGame(BluetoothDevice dev)
    {
        SceneManager.LoadScene("MainScene");
    }

    void HandleOnDeviceOff (BluetoothDevice dev)
	{
		if (!string.IsNullOrEmpty (dev.Name))
			devicNameText.text = "Can't connect to " + dev.Name + ", device is OFF";
		else if (!string.IsNullOrEmpty (dev.Name)) {
			devicNameText.text = "Can't connect to " + dev.MacAddress + ", device is OFF";
		}
	}
	
	//############### UI BUTTONS RELATED METHODS #####################
	public void showDevices ()
	{
        //show a list of all devices
        BluetoothAdapter.showDevices ();//any picked device will be sent to this.HandleOnDevicePicked()
	}

    //Connect to the public global variable "device" if it's not null.
    public void connect ()
	{
		if (this.btDevice != null) {
            this.btDevice.connect();
            GlobalControl.Instance.btDevice = this.btDevice;
            SceneManager.LoadScene("MainScene");
        }
    }
	
	public void disconnect ()//Disconnect the public global variable "device" if it's not null.
	{
		if (this.btDevice != null)
            this.btDevice.close ();
	}

    //Called when device is Picked by user
    void HandleOnDevicePicked (BluetoothDevice device)
	{

        this.btDevice = device;
		devicNameText.text = device.Name;
    }

    //############### UnRegister Events  #####################
    void OnDestroy ()
	{
		BluetoothAdapter.OnDevicePicked -= HandleOnDevicePicked; 
		BluetoothAdapter.OnDeviceOFF -= HandleOnDeviceOff;
	}
}
