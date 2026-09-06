import uuid
import requests
from dataclasses import dataclass, field
from typing import Callable


@dataclass
class ScanContext:
    target: str
    session: requests.Session
    base_response: requests.Response
    _soft404: tuple | None = field(default=None, init=False)

    @property
    def is_https(self) -> bool:
        return self.target.lower().startswith("https://")

    @classmethod
    def create(cls, target: str, timeout: int = 10) -> "ScanContext":
        session = requests.Session()
        base = session.get(target, timeout=timeout, allow_redirects=True)
        return cls(target=target, session=session, base_response=base)

    def soft404(self, timeout: int = 10) -> tuple:
        if self._soft404 is None:
            r = self.session.get(f"{self.target.rstrip('/')}/{uuid.uuid4().hex}", timeout=timeout)
            self._soft404 = (r.status_code, r.headers.get("Content-Type", ""), len(r.content))
        return self._soft404


def finding(check, severity, summary, recommendation, url, indicator) -> dict:
    return {
        "check": check, "severity": severity,
        "summary": summary, "recommendation": recommendation,
        "evidence": {"url": url, "indicator": indicator},
    }

Finding = dict[str, object]

@dataclass
class Check:
    id: str
    can_run: object
    run: object 