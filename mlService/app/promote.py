import os

import mlflow
from mlflow import MlflowClient

mlflow.set_tracking_uri(os.getenv("MLFLOW_TRACKING_URI", "http://127.0.0.1:5001"))

NAME = "cover-classifier"
client = MlflowClient()

versions = client.search_model_versions(f"name='{NAME}'")
sortedVersions = sorted(versions, key=lambda v: int(v.version), reverse=True)

N = 2  # number of models trained with the training.py script:KNN & RFC
recent_versions = sortedVersions[:N]

for latest in reversed(recent_versions):
    latest_run = client.get_run(latest.run_id)
    latest_model_macro = latest_run.data.metrics["macro_f1"]

    try:
        current_champion = client.get_model_version_by_alias(NAME, "champion")
        champion_run = client.get_run(current_champion.run_id)
        champion_macro = champion_run.data.metrics["macro_f1"]

        if latest_model_macro >= champion_macro + 0.02:
            client.set_registered_model_alias(NAME, "champion", latest.version)
            print(f"Promoted version: {latest.version} to champion")
        else:
            raise SystemExit(
                f"Quality gate failed: New macro-F1 ({latest_model_macro:.3f}) "
                f"did not beat baseline ({champion_macro:.3f}) + 0.02 "
                f"(Required: {champion_macro + 0.02:.3f})"
            )
        print("in the try block")
    except Exception:
        client.set_registered_model_alias(NAME, "champion", latest.version)
        print(f"Set the initial champion to the {latest.version}")

model = mlflow.pyfunc.load_model(f"models:/{NAME}@champion")
