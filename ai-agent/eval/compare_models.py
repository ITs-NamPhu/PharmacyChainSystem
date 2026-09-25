"""So sánh metrics giữa các provider/mô hình trên cùng một golden set.

Cách dùng:
    python -m eval.compare_models --providers groq,gemini,auto --mode mock --judge
"""
import argparse
import asyncio
import json
import logging
import pathlib
from datetime import datetime
from types import SimpleNamespace

from eval.run_eval import run_all

pretty_names = {
    "auto": "auto (router heuristic)",
    "groq": "groq",
    "gemini": "gemini",
}
metrics_order = ["case_count", "error_count", "avg_tool_f1", "avg_judge_score",
                 "hallucination_cases", "avg_grounding_precision",
                 "avg_latency_ms", "total_tokens"]


def _cell(summary, key):
    v = summary["overall"].get(key)
    return f"{v:g}" if isinstance(v, (int, float)) else "-"


def main():
    logging.basicConfig(level=logging.INFO, format="%(asctime)s %(levelname)s %(message)s")
    p = argparse.ArgumentParser(description="So sánh provider/model trên cùng golden set")
    p.add_argument("--providers", default="groq,gemini",
                   help="danh sách provider cách nhau bằng dấu phẩy (auto|groq|gemini)")
    p.add_argument("--mode", choices=["mock", "live"], default="mock")
    p.add_argument("--judge", action="store_true")
    p.add_argument("--limit", type=int, default=0)
    p.add_argument("-o", "--out", default="reports")
    args = p.parse_args()

    providers = [x.strip() for x in args.providers.split(",") if x.strip()]

    summaries = {}
    bounds_dir = None
    for provider in providers:
        run_args = SimpleNamespace(mode=args.mode, provider=provider, judge=args.judge,
                                   limit=args.limit, out=args.out)
        md_path = asyncio.run(run_all(run_args))
        d = pathlib.Path(md_path).parent
        bounds_dir = d.parent
        with (d / "summary.json").open(encoding="utf-8") as f:
            summaries[provider] = json.load(f)

    out = bounds_dir / "compare_markdown"
    out.mkdir(parents=True, exist_ok=True)
    lines = [f"# So sánh provider — mode={args.mode}\n",
             "| Metric | " + " | ".join(pretty_names.get(x, x) for x in providers) + " |",
             "|---|" + "---|" * len(providers)]
    for m in metrics_order:
        lines.append(f"| {m} | " + " | ".join(_cell(summaries[x], m) for x in providers) + " |")
    lines.append("")

    for provider in providers:
        lines.append(f"## Chi tiết {pretty_names.get(provider, provider)}\n")
        for cat, s in summaries[provider]["by_category"].items():
            lines.append(f"- {cat}: case={s['case_count']}, F1={s['avg_tool_f1']}, "
                         f"judge={s['avg_judge_score']}, hallu={s['hallucination_cases']}, "
                         f"latency={s['avg_latency_ms']}ms")

    file = out / f"compare_{datetime.now().strftime('%Y%m%d-%H%M%S')}.md"
    file.write_text("\n".join(lines), encoding="utf-8")
    print(f"\n[OK] So sánh: {file}")


if __name__ == "__main__":
    main()