using UnityEngine;
using UnityEngine.UI;
using TechTweaking.Bluetooth;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

public class BluetoothController : MonoBehaviour
{
    public BluetoothDevice btDevice;
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

    private const byte MAX_SPEED = 255;
    private const byte MAX_FORWARD_SPEED = 125;
    private const byte TURNING_GAIN = 80;

    private float prevSendTime;
    private float btSendInterval = 0.1f;

    private Queue<byte[]> cmdQueue = new Queue<byte[]>();

    // Initialization
    void Awake()
    {
        if (btEnabled)
        {
            this.btDevice = GlobalControl.Instance.btDevice;

            if (this.btDevice != null)
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
        if (this.btDevice != null)
        {
            this.btDevice.close();
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

                bool going_straight = false;

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
                    going_straight = true;
                }

                if (joystickInput.y > 0.2 || joystickInput.y < -0.2)
                {
                    int speed = (int)(joystickInput.y * MAX_FORWARD_SPEED);
                    if (!going_straight)
                    {
                        speed += TURNING_GAIN;
                    }
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
                this.btDevice.send(cmd);
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
