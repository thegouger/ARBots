using UnityEngine;
using UnityEngine.UI;
using TechTweaking.Bluetooth;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

public class BluetoothController : MonoBehaviour
{
    private BluetoothDevice btDevice;
    public Text btStatusText;
    public bool btEnabled;

    private enum ROBOT_TURN_DIR {
        NONE = 0,
        LEFT = 1,
        RIGHT = 2
    };

    private enum ROBOT_MOVE_DIR
    {
        STOP = 0,
        FORWARD = 1,
        BACKWARD = 2
    };

    private const byte MAX_SPEED = 100;

    private float prevSendTime;
    private float btSendInterval = 0.15f;

    private Queue<byte[]> cmdQueue = new Queue<byte[]>();

    // Initialization
    void Awake()
    {
        btStatusText.text = "Bluetooth status : Connecting...";
        //btEnable = true;

        if (btEnabled)
        {
            //Ask user to enable Bluetooth
            BluetoothAdapter.enableBluetooth();

            btDevice = new BluetoothDevice();

            // Identify Bluetooth device
            btDevice.Name = "HC-06";
            //btDevice.MacAddress = "20:16:06:15:66:74";

            btDevice.setEndByte(10);

            btStatusText.text = "Bluetooth status : Connecting...";
            // Connect to Bluetooth device
            btDevice.connect(attempts: 10, time: 1000, allowDiscovery: false);

            if (btDevice != null)
            {
                btStatusText.text = "Bluetooth status : Connected";
            }
            else
            {
                btStatusText.text = "Bluetooth status : Cannot Connect";
            }
        }
        else
        {
            btStatusText.text = "Bluetooth status : Disabled";
        }
    }

    // Program start
    void Start()
    {
        prevSendTime = 0;
    }

    // When this object is destroyed
    void OnDestroy()
    {
        if (btDevice != null)
        {
            btDevice.close();
        }
    }

    // Update is called once per frame
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (btEnabled)
        {
            if (Time.time - prevSendTime > btSendInterval)  
            {
                prevSendTime = Time.time;

                Vector2 joystickInput = new Vector2(CrossPlatformInputManager.GetAxis("JoystickX"), CrossPlatformInputManager.GetAxis("JoystickY"));

                if (joystickInput.x > 0.3)
                {
                    TurnRobot(ROBOT_TURN_DIR.RIGHT);
                }
                else if (joystickInput.x < -0.3)
                {
                    TurnRobot(ROBOT_TURN_DIR.LEFT);
                }
                else
                {
                    TurnRobot(ROBOT_TURN_DIR.NONE);
                }

                if (joystickInput.y > 0.2 || joystickInput.y < -0.2)
                {
                    int speed = (int)(joystickInput.y * MAX_SPEED);
                    MoveRobot(speed);
                }
                else
                {
                    MoveRobot(0);
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (btEnabled)
        {
            if (cmdQueue.Count > 0)
            {
                byte[] cmd = cmdQueue.Dequeue();

            }
        }
    }

    private void TurnRobot(ROBOT_TURN_DIR dir)
    {
        byte[] cmd = { (byte)'t', (byte)dir };

        cmdQueue.Enqueue(cmd);
    }

    private void MoveRobot(int speed)
    {
        byte[] cmd = new byte[3];
        cmd[0] = (byte) 'm';

        if (speed == 0)
        {
            cmd[1] = (byte) ROBOT_MOVE_DIR.STOP;
            cmd[2] = 0;
        }
        else
        {
            if (speed < 0)
            {
                cmd[1] = (byte) ROBOT_MOVE_DIR.BACKWARD;
                speed *= -1;
            }
            else
            {
                cmd[1] = (byte) ROBOT_MOVE_DIR.FORWARD;
            }
            
            // capping the speed
            if (speed > MAX_SPEED)
            {
                speed = MAX_SPEED;
            }
            cmd[2] = (byte) speed;
        }
        cmdQueue.Enqueue(cmd);
    }
}
