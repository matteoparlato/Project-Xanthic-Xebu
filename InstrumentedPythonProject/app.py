import os
import time
import requests
 
API_KEY = "5ce88e3505ca41e6a1f133506251609"  # la API key
CITY = "noale"       # default
TARGET_API="http://localhost:5400/postForecast"
def get_weather(city, api_key):
    # url = f"http://api.weatherapi.com/v1/current.json?key={api_key}&q={city}&aqi=no"
    # resp = requests.get(url)
    # if resp.status_code == 200:
    # data = resp.json()
    payload = {
        "location": "location", # data["location"]["name"],
        "temp_c": "temp_c", # data["current"]["temp_c"],
        "humidity": "humidity", # data["current"]["humidity"],
        "condition": "condition" # data["current"]["condition"]["text"]
    }
    print("Dati raccolti:", payload)
    try:
            r = requests.post(TARGET_API, json=payload, timeout=10)
            print(f"Inviato a {TARGET_API} - Status: {r.status_code}")
    except Exception as e:
            print("Errore invio API:", e)
    # else:
    #     print("Errore API:", resp.text)
 
if __name__ == "__main__":
    while True:
        print("dioboia")
        get_weather(CITY, API_KEY)
        time.sleep(1)  # ogni 30sec