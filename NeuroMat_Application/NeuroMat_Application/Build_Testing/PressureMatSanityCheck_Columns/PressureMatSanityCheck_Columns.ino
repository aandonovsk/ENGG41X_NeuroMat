double numberOfSamples = 20;

int Apin =10;
int Bpin = 9;
int Cpin = 8;
int analogPins[] = {A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13};

void setup() 
{
  // put your setup code here, to run once:
    Serial.begin(9600);
    pinMode(Apin,OUTPUT);
    pinMode(Bpin,OUTPUT);
    pinMode(Cpin,OUTPUT);
    setColumnC();
}

void loop() 
{
    double avgSensorVoltages[] = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
    //double sensorVoltage = analogRead(A2);
    
    for (int i=0; i<numberOfSamples; i++)
    {  
        for (int j=0; j<sizeof(analogPins)/sizeof(int); j++)
        {
            avgSensorVoltages[j] = analogRead(analogPins[j])+ avgSensorVoltages[j];
        }    
        delay(100);
    }
  
    for (int i=0; i<sizeof(analogPins)/sizeof(int); i++)
    {
        //Serial.print("Sensor #");
        //Serial.print(i+1);
        //Serial.print(": ");
        Serial.print(((avgSensorVoltages[i]/numberOfSamples)*0.0049),2);
        Serial.print(";");
    }
    Serial.println();
    Serial.println();
}


void setColumnA(){
    digitalWrite(Apin,LOW);
    digitalWrite(Bpin,LOW);
    digitalWrite(Cpin,LOW);
}
void setColumnB(){
    digitalWrite(Apin,HIGH);
    digitalWrite(Bpin,LOW);
    digitalWrite(Cpin,LOW);
}
void setColumnC(){
    digitalWrite(Apin,LOW);
    digitalWrite(Bpin,HIGH);
    digitalWrite(Cpin,LOW);
}
void setColumnD(){
    digitalWrite(Apin,HIGH);
    digitalWrite(Bpin,HIGH);
    digitalWrite(Cpin,LOW);
}
void setColumnE(){
    digitalWrite(Apin,LOW);
    digitalWrite(Bpin,LOW);
    digitalWrite(Cpin,HIGH);
}
void setColumnF(){
    digitalWrite(Apin,HIGH);
    digitalWrite(Bpin,LOW);
    digitalWrite(Cpin,HIGH);
}
void setColumnG(){
    digitalWrite(Apin,LOW);
    digitalWrite(Bpin,HIGH);
    digitalWrite(Cpin,HIGH);
}
void setColumnH(){
    digitalWrite(Apin,HIGH);
    digitalWrite(Bpin,HIGH);
    digitalWrite(Cpin,HIGH);
}

