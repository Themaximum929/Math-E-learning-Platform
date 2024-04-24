from flask import Flask, request, jsonify
import torch
from transformers import GPT2LMHeadModel, GPT2Tokenizer

app = Flask(__name__)

tokenizer = GPT2Tokenizer.from_pretrained("gpt2")
model = GPT2LMHeadModel.from_pretrained("gpt2")

@app.route('/predict', methods=['POST'])
def predict():
    data = request.get_json(force=True)
    prompt = data['prompt']
    
    inputs = tokenizer.encode(prompt, return_tensors="pt")
    outputs = model.generate(inputs, max_length=100, num_return_sequences=1, no_repeat_ngram_size=2)
    text = tokenizer.decode(outputs[0])

    print(jsonify({'result': text }))
    return jsonify({'result': text })

if __name__ == '__main__':
    app.run(port=5000, debug=True)
    
'''
from flask import Flask, request, jsonify
from transformers import AutoModelForCausalLM, AutoTokenizer

app = Flask(__name__)

tokenizer = AutoTokenizer.from_pretrained("HuggingFaceH4/zephyr-7b-alpha")
model = AutoModelForCausalLM.from_pretrained("HuggingFaceH4/zephyr-7b-alpha")

@app.route('/predict', methods=['POST'])
def predict():
    data = request.get_json(force=True)
    prompt = data['prompt']
    
    inputs = tokenizer.encode(prompt, return_tensors="pt")
    outputs = model.generate(inputs, max_length=100, num_return_sequences=1, no_repeat_ngram_size=2)
    text = tokenizer.decode(outputs[0])

    return jsonify({'result': text })

if __name__ == '__main__':
    app.run(port=5000, debug=True)
'''