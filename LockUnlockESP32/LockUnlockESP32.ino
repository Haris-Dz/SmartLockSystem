#include <WiFi.h>
#include <PubSubClient.h>
#include <ESP32Servo.h>
#include <WiFiClientSecure.h>
#include <HTTPClient.h>

// WiFi credentials
const char* ssid = "SSID"; // Replace with your WiFi SSID
const char* password = "Password"; // Replace with your WiFi password

// MQTT Broker details
const char* mqtt_server = "your broker adress"; // MQTT broker address
const int mqtt_port = 8883; // Secure MQTT port
const char* mqtt_user = "your username"; // MQTT username
const char* mqtt_pass = "your password"; // MQTT password

// Topics for MQTT 
const char* topic = "topic";

// Servo setup
Servo myServo;
const int servoPin = 18;
const int lockPosition = 0;
const int unlockPosition = 90;

// RGB LED pins
const int redPin = 25;
const int greenPin = 26;
const int bluePin = 27;

// Button pin
const int buttonPin = 33;  // Change to your desired GPIO pin

// MQTT client
WiFiClientSecure espClient;
PubSubClient client(espClient);

// Root CA certificate
const char* root_ca = \
"Replace with certificate";

// Define the API URL
const char* apiUrl = "url"; // Replace with your API URL

bool isLocked = true; // Track the lock state (true = locked, false = unlocked)

void setup_wifi() {
  delay(10);
  Serial.begin(9600);
  Serial.print("Connecting to ");
  Serial.println(ssid);
  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.print(".");
  }
  Serial.println("\nWiFi connected");
  Serial.print("IP address: ");
  Serial.println(WiFi.localIP());
  client.setServer(mqtt_server, mqtt_port);
}

void callback(char* topic, byte* payload, unsigned int length) {
  String message;
  for (int i = 0; i < length; i++) {
    message += (char)payload[i];
  }
  Serial.print("Message received: ");
  Serial.println(message);

  if (message.equalsIgnoreCase("Lock")) {
    Serial.println("Locking...");
    myServo.write(lockPosition);
    isLocked = true;  // Set the state to locked
    digitalWrite(redPin, LOW);   // Red ON
    digitalWrite(greenPin, HIGH); // Green OFF
    digitalWrite(bluePin, HIGH);  // Blue OFF
  } else if (message.equalsIgnoreCase("Unlock")) {
    Serial.println("Unlocking...");
    myServo.write(unlockPosition);
    isLocked = false;  // Set the state to unlocked
    digitalWrite(redPin, HIGH);   // Red OFF
    digitalWrite(greenPin, LOW);  // Green ON
    digitalWrite(bluePin, HIGH);  // Blue OFF
  } else {
    Serial.println("Unknown command");
  }
}

void reconnect() {
  while (!client.connected()) {
    Serial.print("Attempting MQTT connection...");
    if (client.connect("ESP32Client", mqtt_user, mqtt_pass)) {
      Serial.println("connected");
      client.subscribe(topic);
    } else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" trying again in 5 seconds");
      delay(5000);
    }
  }
}

void sendApiRequest(String lockState) {
  HTTPClient http;
  http.begin(apiUrl);  // Specify the URL
  http.addHeader("Content-Type", "application/json");  // Set content type to JSON

  String jsonPayload = "{\"status\":\"" + lockState + "\"}";  // Create JSON payload

  int httpResponseCode = http.POST(jsonPayload);  // Send the POST request

  if (httpResponseCode > 0) {
    Serial.print("HTTP Response code: ");
    Serial.println(httpResponseCode);
    String response = http.getString();
    Serial.println("Response: " + response);
  } else {
    Serial.print("Error on sending POST request: ");
    Serial.println(httpResponseCode);
    String response = http.getString();
    Serial.println("Error Response: " + response);
  }

  http.end();  // Close connection
}


void setup() {
  Serial.begin(115200);
  setup_wifi();
  //espClient.setCACert(root_ca);
  espClient.setInsecure();
  client.setServer(mqtt_server, mqtt_port);
  client.setCallback(callback);

  myServo.attach(servoPin);
  myServo.write(lockPosition);  // Start in locked position

  pinMode(redPin, OUTPUT);
  pinMode(greenPin, OUTPUT);
  pinMode(bluePin, OUTPUT);

  digitalWrite(redPin, LOW);    // Red ON (locked)
  digitalWrite(greenPin, HIGH); // Green OFF
  digitalWrite(bluePin, HIGH);  // Blue OFF

  pinMode(buttonPin, INPUT_PULLUP); // Button input with pull-up resistor
}

void loop() {
  if (!client.connected()) {
    reconnect();
  }
  client.loop();

  // Check if button is pressed
  if (digitalRead(buttonPin) == LOW) {
    // Toggle the lock/unlock state
    if (isLocked) {
      myServo.write(unlockPosition);
      isLocked = false;
      digitalWrite(redPin, HIGH);   // Red OFF
      digitalWrite(greenPin, LOW);  // Green ON
      digitalWrite(bluePin, HIGH);  // Blue OFF
      sendApiRequest("Unlock");
    } else {
      myServo.write(lockPosition);
      isLocked = true;
      digitalWrite(redPin, LOW);    // Red ON
      digitalWrite(greenPin, HIGH); // Green OFF
      digitalWrite(bluePin, HIGH);  // Blue OFF
      sendApiRequest("Lock");
    }
    
    // Debounce the button press
    delay(300);
  }
}
