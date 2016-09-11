import serial
from time import sleep

PORT_PATH = "COM5"

device_found = False

try:
    with serial.Serial(PORT_PATH, 9600, timeout=1) as ser:

        # Get Echo message
        msg = bytes([ord('e'), 123, 255])
        ser.write(msg)
        sleep(0.1)
        print(ser.readline())

        # Go Forward
        msg = bytes([ord('m'), 1, 100])
        ser.write(msg)
        sleep(1.0)

        # Go Backward
        msg = bytes([ord('m'), 2, 100])
        ser.write(msg)
        sleep(1.0)

        # Turn Left
        msg = bytes([ord('t'), 1])
        ser.write(msg)
        sleep(1.0)

        # Turn Right
        msg = bytes([ord('t'), 2])
        ser.write(msg)
        sleep(1.0)

        # Go Faster
        msg = bytes([ord('m'), 2, 255])
        ser.write(msg)
        sleep(1.0)

        # Stop
        msg = bytes([ord('m'), 0])
        ser.write(msg)
        sleep(1.0)

        # Straighten Steering
        msg = bytes([ord('t'), 0])
        ser.write(msg)
        sleep(1.0)

        print("Basic Test Completed")

except serial.SerialException:
    print("ERROR: Cannot find the robot!")