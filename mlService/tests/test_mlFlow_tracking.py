from unittest.mock import patch

import mlflow
import pytest


@patch("app.train.train_model")
def test_mlFlow_run_execution():
    with mlflow.start_run() as run:
        assert run is not None
        assert run.info.run_id is not None

    assert mlflow.active_run() is None


def test_mlFlow_log_metrics():
    assert mlflow.log_metric("macro_f1", 0.8)
    assert mlflow.log_metric("accuracy", 0.6)
