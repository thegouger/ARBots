
const char* ROBOT_NAME = "Hummer1";
const uint16_t BAUDRATE = 9600;

const uint8_t PIN_ONBOARD_LED = 13;
const uint8_t PIN_PWMA = 3;
const uint8_t PIN_PWMB = 4;
const uint8_t PIN_AIN1 = 23;
const uint8_t PIN_AIN2 = 22;
const uint8_t PIN_BIN1 = 2;
const uint8_t PIN_BIN2 = 5;

const uint8_t REST_THRESHOLD = 5;
const uint8_t TURN_POWER = 255;
const int8_t TURN_LEFT = -1;
const int8_t TURN_RIGHT = 1;
const int8_t TURN_NONE = 0;

void setup()
{
  pinMode(PIN_ONBOARD_LED, OUTPUT);      // sets the digital pin as output

  pinMode(PIN_PWMA, OUTPUT);
  pinMode(PIN_AIN1, OUTPUT);
  pinMode(PIN_AIN2, OUTPUT);

  pinMode(PIN_PWMB, OUTPUT);
  pinMode(PIN_BIN1, OUTPUT);
  pinMode(PIN_BIN2, OUTPUT);

  // initialize serial communication
  Serial1.begin(BAUDRATE);
}

// Protocol
// ===============
// byte 0: types of commands
// byte 1: parameter 1
// byte 2: parameter 2

// Commands
// ---------------
enum cmd {
    ECHO = 'e', 
    // Return with the same message in parameter 1.

    MOVE = 'm', 
    // Move the back wheels. 
    // Parameter 1: 0 = stop; 1 = forward, 2 = backward
    // Parameter 2: 0 to 255 = speed of motor (ignored for "stop")
    
    TURN = 't'
    // Turn the front wheels.
    // Parameter 1: 0 = center, 1 = left, 2 = right
};

const uint8_t BUFF_SIZE = 3;
uint8_t msgBuff[BUFF_SIZE];

void loop()
{
    digitalWrite(PIN_ONBOARD_LED, HIGH);   // sets the LED on
    
    // clear buffer
    for (uint8_t i = 0; i < BUFF_SIZE; i++){
        msgBuff[i] = 0;
    }
    
    if (Serial1.available() > 0) {
        if (Serial1.readBytes(msgBuff, BUFF_SIZE) > 1){
            switch(msgBuff[0]){
                case 'e':
                    handleEchoCmd(msgBuff);
                    break;
                case 'm':
                    handleMoveCmd(msgBuff[1], msgBuff[2]);
                    break;
                case 't':
                    handleTurnCmd(msgBuff[1]);
                    break;
                default:
                    //do nothing
                    break;
            }
        }
    }
    delay(10);
}

void handleEchoCmd(uint8_t* msg){
    Serial1.write(msg, 3);
}

void handleMoveCmd(uint8_t dir, uint8_t pwr){
    int speed = (int) pwr;
    
    if (dir == 0){
        backWheelsMove(0);
    }
    else{
        if (dir == 2) {
            speed *= -1;
        }
        backWheelsMove(speed);
    }
    Serial1.write("done");
}
void handleTurnCmd(uint8_t dir){
    if (dir == 1){
        frontWheelsTurn(TURN_LEFT);
    }
    else if (dir == 2){
        frontWheelsTurn(TURN_RIGHT);
    }
    else{
        frontWheelsTurn(TURN_NONE);
    }
    Serial1.write("done");
}

// -255 for fully backward
//255 for fully forward
void backWheelsMove(int speed){
    if (speed <= REST_THRESHOLD && speed >= -REST_THRESHOLD){
      digitalWrite(PIN_BIN1, LOW);
      digitalWrite(PIN_BIN2, LOW);
      analogWrite(PIN_PWMB, 0);
      return;
    }

    if (speed > REST_THRESHOLD){
      digitalWrite(PIN_BIN1, LOW);
      digitalWrite(PIN_BIN2, HIGH);
    }
    else{
      digitalWrite(PIN_BIN1, HIGH);
      digitalWrite(PIN_BIN2, LOW);
      // invert the speed
      speed = -speed;
    }
    // cap the speed
    if (speed > 255){
      speed = 255;
    }
    analogWrite(PIN_PWMB, speed);
}

// 0 for left striaght
// -1 for left turn
// 1 for right turn
void frontWheelsTurn(int dir){

    if (dir == 0){
      digitalWrite(PIN_AIN1, LOW);
      digitalWrite(PIN_AIN2, LOW);
      analogWrite(PIN_PWMA, 0);
      return;
    }

    if (dir > 0){
      digitalWrite(PIN_AIN1, HIGH);
      digitalWrite(PIN_AIN2, LOW);
    }
    else{
      digitalWrite(PIN_AIN1, LOW);
      digitalWrite(PIN_AIN2, HIGH);
    }
    analogWrite(PIN_PWMA, TURN_POWER);
}
