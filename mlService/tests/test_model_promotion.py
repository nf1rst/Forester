import pytest
import mlflow
import os
import pandas as pd
from unittest.mock import MagicMock
from mlflow import MlflowClient
from unittest.mock import patch
from mlflow.exceptions import MlflowException
from app.promote import promote_latest_version


def make_version(version, run_id):
    return MagicMock(version=version, run_id=run_id)


def make_run(macro_f1):
    run = MagicMock()
    run.data.metrics = {"macro_f1": macro_f1}
    return run


def test_crown_first_champion_if_none_exist():
    client = MagicMock()
    client.search_model_versions.return_value = [make_version("1", "run_1")]
    client.get_run.return_value = make_run(0.80)

    client.get_model_version_by_alias.side_effect = MlflowException("no alias")

    promote_latest_version(client)

    client.set_registered_model_alias.assert_called_once_with(
        "cover-classifier", "champion", "1"
    )


def test_champion_is_replaced_when_new_model_beats_threshold():
    client = MagicMock()
    old = make_version("1", "run_1")
    new = make_version("2", "run_2")
    client.search_model_versions.return_value = [old, new]
    client.get_model_version_by_alias.return_value = old

    runs = {"run_1": make_run(0.80), "run_2": make_run(0.90)}
    client.get_run.side_effect = lambda run_id: runs[run_id]

    promote_latest_version(client)

    client.set_registered_model_alias.assert_called_once_with(
        "cover-classifier", "champion", "2"
    )


def test_champion_stays_champion_if_new_model_under_threshold():
    client = MagicMock()
    old = make_version("1", "run_1")
    new = make_version("2", "run_2")
    client.search_model_versions.return_value = [old, new]
    client.get_model_version_by_alias.return_value = old

    runs = {"run_1": make_run(0.80), "run_2": make_run(0.60)}
    client.get_run.side_effect = lambda run_id: runs[run_id]

    promote_latest_version(client)

    client.set_registered_model_alias.assert_not_called()
