from unittest.mock import patch
from unittest.mock import call

import numpy as np
from sklearn.ensemble import RandomForestClassifier
from sklearn.neighbors import KNeighborsClassifier

from app import train

from sklearn.metrics import f1_score
from sklearn.metrics import accuracy_score


def test_mlflow_configuration(monkeypatch):
    monkeypatch.setenv("MLFLOW_TRACKING_URI", "http://test-host:5001")
    with patch("app.train.mlflow") as m:
        train.configure_mlFlow()
    m.set_tracking_uri.assert_called_once_with("http://test-host:5001")


def make_data():
    rng = np.random.default_rng(0)
    X = rng.random((40, 4))
    y = rng.integers(0, 2, 40)
    return dict(
        X_train=X,
        y_train=y,
        X_test=X,
        y_test=y,
        X_train_scaled=X,
        X_test_scaled=X,
        X_train_knn=X,
        y_train_knn=y,
    )


@patch("app.train.mlflow")
def test_tree_model_gets_trusted_types(mock_mlflow):
    train.train_model(
        "RFC", RandomForestClassifier(n_estimators=2), False, False, make_data()
    )
    kwargs = mock_mlflow.sklearn.log_model.call_args.kwargs
    assert kwargs["skops_trusted_types"] == "sklearn.tree._tree.Tree"


@patch("app.train.mlflow")
def test_mlflow_run(mock_mlflow):
    model_name = "RFC"
    train.train_model(
        model_name, RandomForestClassifier(n_estimators=2), False, False, make_data()
    )
    mock_mlflow.start_run.assert_called_with(run_name=model_name)


@patch("app.train.mlflow")
def test_mlflow_metric_log(mock_mlflow):
    d = make_data()
    model = RandomForestClassifier(n_estimators=2)
    train.train_model("model", model, False, False, d)

    predictions = model.predict(d["X_test"])

    mock_mlflow.log_metric.assert_has_calls(
        [
            call("macro_f1", f1_score(d["y_test"], predictions, average="macro")),
            call("accuracy", accuracy_score(d["y_test"], predictions)),
        ],
        any_order=True,
    )
