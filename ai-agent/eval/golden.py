import json
import pathlib

GOLDEN_SET_PATH = pathlib.Path(__file__).parent / "golden_set.jsonl"


def load_cases() -> list[dict]:
    """Nạp golden set (list các case dict)."""
    cases = []
    with GOLDEN_SET_PATH.open("r", encoding="utf-8") as f:
        for line in f:
            line = line.strip()
            if not line:
                continue
            cases.append(json.loads(line))
    return cases
