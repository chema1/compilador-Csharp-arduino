//sketch desde consola
int led=13;
int t=250;

void setup()
	{
	pinMode(led, OUTPUT);
	}

void loop()
	{
	digitalWrite(led, HIGH);
	delay(t);
	digitalWrite(led, LOW);
	delay(t);
	}
