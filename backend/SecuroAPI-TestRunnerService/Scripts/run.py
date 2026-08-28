import os,json,sys,time
time.sleep(70)
print(json.dumps({"target" : os.environ.get("TARGET_URL" , ""), "findings": []}))
sys.exit(0)
