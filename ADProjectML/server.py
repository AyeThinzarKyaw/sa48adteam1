import pandas as pd
from flask import Flask, request, jsonify, render_template
import pickle
import json
import jsonschema
from LogicDataOne import LRTraining
from pandas.io.json import json_normalize

app = Flask(__name__)

f = open('LogicDataOne.pkl', 'rb')
model = pickle.load(f)

# handles the POST request to this home URL
@app.route('/', methods=['POST'])
def FilterByName():
    req = request.get_json(force = True) # get entire incoming request
    filteredName = model.EstimateAllRequisitionQuantity(req['InputYear'],req['InputMonth'],req['InputDay'])
    return filteredName

@app.route('/', methods=['GET'])
def FilterByName2():
    req = request.get_json(force = True) # get entire incoming request
    filteredName = model.EstimateAllRequisitionQuantity(req['InputYear'],req['InputMonth'],req['InputDay'])
    return filteredName

# run the server
if __name__ == '__main__':

    app.run(port=5000, debug=True)
