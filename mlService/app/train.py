from pathlib import Path

import mlflow
import mlflow.sklearn
import pandas as pd
from pandas import DataFrame
from sklearn.base import BaseEstimator
from sklearn.ensemble import RandomForestClassifier
from sklearn.metrics import accuracy_score, f1_score
from sklearn.model_selection import train_test_split
from sklearn.neighbors import KNeighborsClassifier
from sklearn.preprocessing import StandardScaler


def process_data(data: DataFrame):
    global X_train, y_train, X_test, y_test, X_train_scaled, X_test_scaled
    X = dataframe.iloc[:, :-1].astype("float32")
    y = dataframe.iloc[:, -1]

    X_train, X_test, y_train, y_test = train_test_split(
        X, y, test_size=0.2, random_state=42, stratify=y
    )

    scaler = StandardScaler()
    X_train_scaled = scaler.fit_transform(X_train)
    X_test_scaled = scaler.transform(X_test)


def train_model(name: str, model: BaseEstimator, use_scaled_set: bool):
    trusted_types = None
    if use_scaled_set:  # not a tree based model
        X_tr, X_te = X_train_scaled, X_test_scaled
    else:
        X_tr, X_te = X_train, X_test
        trusted_types = "sklearn.tree._tree.Tree"

    with mlflow.start_run(run_name=name):
        model.fit(X_tr, y_train)
        predictions = model.predict(X_te)

        macro_f1 = f1_score(y_test, predictions, average="macro")
        acc = accuracy_score(y_test, predictions)

        params = model.get_params()
        for param_name, param_value in params.items():
            if param_value is not None:
                mlflow.log_param(param_name, param_value)

        mlflow.log_metric("accuracy", acc)
        mlflow.log_metric("macro_f1", macro_f1)

        mlflow.sklearn.log_model(
            sk_model=model,
            name=name,
            registered_model_name="cover-classifier",
            skops_trusted_types=trusted_types,
        )


try:
    base_dir = Path(__file__).resolve().parent.parent
    input_file = base_dir / "data" / "covtype.data"

    dataframe = pd.read_csv(input_file, header=None)

    process_data(dataframe)

    models = [
        (
            "KNN",
            KNeighborsClassifier(n_neighbors=5, weights="uniform", n_jobs=-1),
            True,
        ),
        (
            "RFC",
            RandomForestClassifier(
                n_estimators=50,
                max_depth=20,
                min_samples_leaf=5,
                n_jobs=1,
                random_state=42,
            ),
            False,
        ),
    ]

    for name, model, use_scaled in models:
        print("Training model")
        train_model(name, model, use_scaled)
        print("Training finished")

except Exception as e:
    print(f"Cannot find the file specified: {e}")
