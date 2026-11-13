import os
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


def configure_mlFlow():
    mlflow.set_tracking_uri(os.getenv("MLFLOW_TRACKING_URI", "http://127.0.0.1:5001"))
    mlflow.set_experiment("coverType-classifier")


def process_data(data: DataFrame):
    X = data.iloc[:, :-1].astype("float32")
    y = data.iloc[:, -1]

    X_train, X_test, y_train, y_test = train_test_split(
        X, y, test_size=0.2, random_state=42, stratify=y
    )

    scaler = StandardScaler()
    X_train_scaled = scaler.fit_transform(X_train)
    X_test_scaled = scaler.transform(X_test)

    X_train_knn, _, y_train_knn, _ = train_test_split(
        X_train_scaled, y_train, train_size=30000, random_state=42, stratify=y_train
    )
    return dict(
        X_train=X_train,
        y_train=y_train,
        X_test=X_test,
        y_test=y_test,
        X_train_scaled=X_train_scaled,
        X_test_scaled=X_test_scaled,
        X_train_knn=X_train_knn,
        y_train_knn=y_train_knn,
    )


def train_model(
    name: str, model: BaseEstimator, use_scaled_set: bool, use_knn_subset: bool, d
):
    trusted_types = None
    if use_knn_subset:
        X_tr, y_tr = d["X_train_knn"], d["y_train_knn"]
    elif use_scaled_set:  # not a tree based model
        X_tr, y_tr = d["X_train_scaled"], d["y_train"]
    else:
        X_tr, y_tr = d["X_train"], d["y_train"]
        trusted_types = "sklearn.tree._tree.Tree"

    X_te = d["X_test_scaled"] if use_scaled_set else d["X_test"]

    with mlflow.start_run(run_name=name):
        model.fit(X_tr, y_tr)
        predictions = model.predict(X_te)

        macro_f1 = f1_score(d["y_test"], predictions, average="macro")
        acc = accuracy_score(d["y_test"], predictions)

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


def main():
    configure_mlFlow()
    base_dir = Path(__file__).resolve().parent.parent
    input_file = base_dir / "data" / "covtype.data"

    if not input_file.exists():
        raise FileNotFoundError(
            f"The file {input_file} has not been found, try running the dvc pull to get files locally"
        )

    df = pd.read_csv(input_file, header=None)

    d = process_data(df)

    models = [
        (
            "KNN",
            KNeighborsClassifier(n_neighbors=5, weights="uniform", n_jobs=2),
            True,
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
            False,
        ),
    ]

    for name, model, use_scaled, use_knn in models:
        print("Training model")
        train_model(name, model, use_scaled, use_knn, d)
        print("Training finished")


if __name__ == "__main__":
    main()
