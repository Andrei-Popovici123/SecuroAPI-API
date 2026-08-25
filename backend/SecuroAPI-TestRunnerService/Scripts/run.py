import os,json,sys
print( "Ia ni ca merge" )
print()
print(json.dumps({"target" : os.environ.get("TARGET_URL" , ""), "findings": []}))
sys.exit(0)
