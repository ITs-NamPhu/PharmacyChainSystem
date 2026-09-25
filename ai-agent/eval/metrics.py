"""Các chỉ số định lượng cho evaluation."""
import re
from typing import Iterable, List

_NUM_RE = re.compile(r"\d+(?:[.,]\d+)*")


def tool_f1(expected: List[str], called: List[str]) -> float:
    """F1 trên tập tool dự kiến sử dụng so với tool thực tế agent gọi."""
    exp, cal = set(expected), set(called)
    if not exp and not cal:
        return 1.0
    if not exp or not cal:
        return 0.0
    tp = len(exp & cal)
    precision = tp / len(cal)
    recall = tp / len(exp)
    if precision + recall == 0:
        return 0.0
    return 2 * precision * recall / (precision + recall)


def _norm_number(token: str) -> float:
    return float(token.replace(",", ""))


def extract_numbers(text: str) -> List[float]:
    """Trích mọi số trong đoạn text (dấu phẩy trở thành phần thập phân nếu chỉ 1 lần cuối)."""
    numbers = []
    for m in _NUM_RE.finditer(text):
        token = m.group(0)
        try:
            numbers.append(_norm_number(token))
        except ValueError:
            continue
    return numbers


def hallucinated_numbers(answer: str, contexts: Iterable[str]) -> List[float]:
    """Các số xuất hiện trong câu trả lời nhưng KHÔNG có trong bất kỳ context nào.

    Là heuristic cảnh báo "bịa số" — không phải kết luận tuyệt đối (ngày tháng,
    số thứ tự, phép toán đơn giản có thể là chính đáng).
    """
    context_numbers = set()
    for ctx in contexts:
        if not ctx:
            continue
        context_numbers.update(extract_numbers(ctx))
    return [n for n in extract_numbers(answer) if n not in context_numbers]


def precision_over_grounding(answer: str, contexts: Iterable[str]) -> float:
    """Tỷ lệ các số trong câu trả lời có nguồn gốc từ tool outputs (grounding)."""
    ans_nums = extract_numbers(answer)
    if not ans_nums:
        return 1.0
    ctx_nums = set()
    for ctx in contexts:
        if not ctx:
            continue
        ctx_nums.update(extract_numbers(ctx))
    grounded = sum(1 for n in ans_nums if n in ctx_nums)
    return grounded / len(ans_nums)