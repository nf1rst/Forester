import os

import mlflow.sklearn
from fastapi import FastAPI, HTTPException
from mlflow import MlflowClient
from pydantic import BaseModel

app = FastAPI()
NAME = "cover-classifier"

model = None
model_name_alias = "No model loaded yet"


try:
    model = mlflow.sklearn.load_model(f"models:/{NAME}@champion")
except Exception as e:
    print("No champion model found: {e}")


class InputData(BaseModel):
    Elevation: int
    Aspect: int
    Slope: int
    Horizontal_Distance_To_Hydrology: int
    Vertical_Distance_To_Hydrology: int
    Horizontal_Distance_To_Roadways: int
    Hillshade_9am: int
    Hillshade_Noon: int
    Hillshade_3pm: int
    Horizontal_Distance_To_Fire_Points: int
    Wilderness_Area: int
    Soil_Type: int


@app.get("/")
def home():
    return {
        "message": "The model is ready for your predictions",
        "model": model_name_alias,
    }
