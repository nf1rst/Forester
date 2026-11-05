from pathlib import Path

import pandas as pd
from pandas import DataFrame
from sklearn.base import BaseEstimator
from sklearn.ensemble import RandomForestClassifier
from sklearn.linear_model import LogisticRegression
from sklearn.metrics import f1_score
from sklearn.model_selection import train_test_split
from sklearn.neighbors import KNeighborsClassifier
from sklearn.preprocessing import StandardScaler


def process_data(data: DataFrame):
    global X_train, y_train, X_test, y_test, X_train_scaled, X_test_scaled
    X = dataframe.iloc[:, :-1].astype("float32")
    y = dataframe.iloc[:, -1]

    X_train, X_test, y_train, y_test = train_test_split(
        X, y, test_size=0.2, random_state=42
    )

    scaler = StandardScaler()
    X_train_scaled = scaler.fit_transform(X_train)
    X_test_scaled = scaler.transform(X_test)


def train_model(model: BaseEstimator):
    if hasattr(model, "tree_"):
        model.fit(X_train, y_train)
        predictions = model.predict(X_test)
    else:
        model.fit(X_train_scaled, y_train)
        predictions = model.predict(X_test_scaled)

    print("Finished training")

    print(f1_score(y_test, predictions, average="macro"))


try:
    base_dir = Path(__file__).resolve().parent.parent
    input_file = base_dir / "data" / "covtype.data"

    dataframe = pd.read_csv(input_file, header=None)

    process_data(dataframe)

    train_model(LogisticRegression(max_iter=100))
    train_model(
        KNeighborsClassifier(
            n_neighbors=5,
            weights="uniform",
            n_jobs=-1,
        )
    )


except Exception as e:
    print(f"Cannot find the file specified: {e}")
