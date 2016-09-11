import serial
from time import sleep
import threading
import queue

PORT_PATH = "COM5"
BAUD_RATE = 9600

in_msg_queue = queue.Queue()
out_msg_queue = queue.Queue()

class pollSerial(threading.Thread):

    def __init__(self, serial_port=PORT_PATH, baud_rate=BAUD_RATE, timeout=1):

        threading.Thread.__init__(self)

        self.serial_port = serial_port
        self.baud_rate = baud_rate
        self.timeout = 1
        self.ser = serial.Serial(self.serial_port, self.baud_rate, timeout=self.timeout)

    def run(self):
        self.serial_read()

    def serial_read(self):

        while(True):

            try:
                if not self.ser:
                    self.ser = serial.Serial(self.serial_port, self.baud_rate, timeout=self.timeout)
                    print("Reconnecting")

                in_msg = self.ser.readline()
                if in_msg and len(list(bytes(in_msg)))> 0:
                    in_msg_queue.put(in_msg)

                self.serial_write()

            except Exception as e:

                print(e)

                if self.ser:
                    self.ser.close()
                    self.ser = None
                    print("Disconnecting")

                print("No Connection")
            sleep(0.2)

    def serial_write(self):
        try:
            out_msg = out_msg_queue.get_nowait()
            self.ser.write(out_msg)
        except queue.Empty:
            pass


if __name__ == "__main__":

    bt_poll_thread = pollSerial()
    bt_poll_thread.start()

    test_messages = [(bytes([ord('m'), 1, 100]), "Go Forward"),
                     (bytes([ord('m'), 2, 100]), "Go Backward"),
                     (bytes([ord('t'), 1]), "Turn Left"),
                     (bytes([ord('t'), 2]), "Turn Right"),
                     (bytes([ord('m'), 2, 255]), "Go Faster"),
                     (bytes([ord('m'), 0]), "Stop"),
                     (bytes([ord('t'), 0]), "Straighten Steering"),
                    ]

    # Get Echo message
    msg = bytes([ord('e'), 123, 255])
    out_msg_queue.put(msg)
    sleep(0.1)
    echo_msg = in_msg_queue.get()
    print("Echo: ", list(bytes(echo_msg)) )
    if (echo_msg == msg):
        print("[PASSED] Echo Test")
    else:
        print("[FAILED] Echo Test")

    # Test cases
    for test_msg, test_name in test_messages:

        out_msg_queue.put(test_msg)
        in_msg = in_msg_queue.get()
        if in_msg == b'done':
            print("[PASSED] %s" % test_name)
        else:
            print("[FAILED] %s" % test_name)

    print("Basic Test Completed")


