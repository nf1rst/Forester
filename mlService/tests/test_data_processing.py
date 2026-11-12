# focus on data loading, validation, missing value check, stratification and scaling
from pathlib import Path

import pandas as pd
import pytest
from sklearn.model_selection import train_test_split


@pytest.fixture
def dataset_path():
    base_dir = Path(__file__).resolve().parent.parent
    return base_dir / "data" / "covtype.data"


def test_covtype_data_exists(dataset_path):
    assert dataset_path.exists(), (
        f"The file {dataset_path} was not found. Try running dvc pull to get the files"
    )


def test_54_values_per_row(dataset_path):
    df = pd.read_csv(dataset_path, header=None)
    assert df.shape[1] == 55


def test_result_between_1_and_7(dataset_path):
    df = pd.read_csv(dataset_path, header=None)

    target_column = df.iloc[:, -1]
    assert target_column.min() >= 1, (
        f"Found target has a label less than 1: {target_column.min()}"
    )
    assert target_column.max() <= 7, (
        f"Found targer has a label greater than 7: {target_column.max()}"
    )


def test_wilderness_area_binary(dataset_path):
    df = pd.read_csv(dataset_path, header=None)
    wilderness_df = df.iloc[:, 10:14]
    assert wilderness_df.isin([0, 1]).all().all(), (
        "Wilderness area columns contain values other than 0 and 1"
    )

    row_sums = wilderness_df.sum(axis=1)
    assert (row_sums == 1).all(), (
        "Some rows have more than one 1 per row for wilderness areas"
    )


def test_soiltype_bineary(dataset_path):
    df = pd.read_csv(dataset_path)

    soiltype_df = df.iloc[:, 14:54]
    assert soiltype_df.isin([0, 1]).all().all(), (
        "Soil type columns cotntainer values other than 0 and 1"
    )

    row_sums = soiltype_df.sum(axis=1)
    assert (row_sums == 1).all(), (
        "Some rows have more than one 1 per row for soil types"
    )


def test_train_test_split_80_20(dataset_path):
    df = pd.read_csv(dataset_path, header=None)

    X = df.iloc[:, :-1]
    y = df.iloc[:, -1]

    X_train, X_test, y_train, y_test = train_test_split(
        X, y, test_size=0.2, random_state=42, stratify=y
    )

    total_rows = X.shape[0]

    # rounding tolerance +-1
    assert abs(len(X_train) - (total_rows * 0.8)) <= 1
    assert abs(len(X_test) - (total_rows * 0.2)) <= 1
    assert abs(len(y_train) - (total_rows * 0.8)) <= 1
    assert abs(len(y_test) - (total_rows * 0.2)) <= 1
