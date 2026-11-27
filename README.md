# Guide to Forester

## Forester MLService 
The connections for starting the application locally or with docker compose up --build do not need to be changed inside the mlService directory

The GUI for the model registry can be accessed on localhost:5001
 
The champion model can be accessed on on localhost:8000
For more detailed overview access Swagger UI on localhost:8000/docs

## Forester Web Service
Connection string for local and container start will be automatically recognized

Application will try to load an env variable from the dockerfile and use localhost it it does not find it 

Developer does not need to change the connection string manually anymore

### Local start 
Before running the web application, make sure the registry and then the fastAPI is activated inside the virtual environment - in that order

Run local mlRegistry: 
`mlflow server --host 127.0.0.1 --port 5001`

Run fastAPI (from mlService/app)
`python -m uvicorn main:app --host 0.0.0.0 --port 8000 --reload`

Run the Web application from an IDE 

### Container start 
Run
`docker compose up--build`
for stsarting the build process. The first tim might take a few minutes for the images to download. 

Check the container status with
`docker ps -a`

Permanently remove the containers at once by
`docker compose down -v`

#### Registry fill
If there are no models available in the registry, run the train.py script inside the mlService/app in a venv:
`python train.py`
The script currently trains 2 models with tailored parameters: KNN and RFC


#### No champion
If there are models in the registry, but no champion, you can crown the first champion from inside mlService/app in a venv
`python promote.py`
This script will crown the latest trained model if no champion exists, or compare the newly trained models's macro-f1 score again the current champion's. If the new macro-f1 + by at least 0.02 greater, a new champion is crowned

#### Training data
This project includes DVC. Set up the dvc by running
`az login` 
and logging in with your account. Only the developer's account is currently available to pull data. 
Set up the local information for dvc
`dvc remote modify --local azstorage connection_string "CONNECTION STRING COPIED FROM AZURE BLOB STORAGE ACCESS KEYS FOR KEY 1")`
Then run dvc pull in the mlService/data

