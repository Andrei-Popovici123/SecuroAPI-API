FROM python:3.12-slim
WORKDIR /app
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt
COPY contract.py checks_headers.py checks_requests.py checks_active.py run.py .
ENTRYPOINT ["python", "run.py"]