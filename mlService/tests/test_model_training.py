import os
from pathlib import Path
from unittest.mock import patch

import mlflow
import pandas as pd
import pytest
from sklearn.model_selection import train_test_split

mlflow.set_tracking_uri(os.getenv("MLFLOW_TRACKING_URI", "http://localhost:5001"))


@pytest.fixture
def dataset_path():
    base_dir = Path(__file__).resolve().parent.parent
    return base_dir / "data" / "covtype.data"


@pytest.fixture
def data_sets(dataset_path):
    df = pd.read_csv(dataset_path, header=None)
    X = df.iloc[:, :-1]
    y = df.iloc[:, -1]
    X_train, X_test, y_train, y_test = train_test_split(
        X, y, test_size=0.2, random_state=42, stratify=y
    )
    return X_train, X_test, y_train, y_test


@pytest.fixture
def champion_model():
    try:
        return mlflow.sklearn.load_model("models:/cover-classifier@champion")
    except Exception:
        pytest.skip("MLFLow registry is not available, skipping tests")


def test_KNN_parameters(champion_model):
    params = champion_model.get_params()
    if hasattr(champion_model, "n_neighbors"):
        assert params.get("n_neighbors") == 5
    if hasattr(champion_model, "n_jobs"):
        assert params.get("n_jobs") >= 1


def test_RFC_parameters(champion_model):
    params = champion_model.get_params()
    if hasattr(champion_model, "n_estimators"):
        assert params.get("n_estimators") == 50
    if hasattr(champion_model, "max_depth"):
        assert params.get("max_depth") == 20
    if hasattr(champion_model, "min_samples_leaf"):
        assert params.get("min_samples_leaf") == 5
    if hasattr(champion_model, "n_jobs"):
        assert params.get("n_jobs") >= 1
    if hasattr(champion_model, "random_state"):
        assert params.get("random_state") == 42


def test_macro_f1_score_between_0_1(champion_model):
    smallest_macro_f1 = mlflow.search_runs(filter_string="metrics.macro_f1 <0")
    greaterst_macro_f1 = mlflow.search_runs(filter_string="metrics.macro_f1>1")

    assert len(smallest_macro_f1) == 0
    assert len(greaterst_macro_f1) == 0


# def test_model_fit_execution(champion_model, data_sets):
#     X_train, _, y_train, _ = data_sets
#     assert champion_model.fit(X_train, y_train)
