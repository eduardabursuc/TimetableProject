from flask import Flask, request, jsonify
from linear_regression import RidgeConstraintClassifier
from input_preprocessing import preprocess_text

app = Flask(__name__)

# Load the model once when the API starts
classifier = RidgeConstraintClassifier()
classifier.load_model("ridge_model.pkl")

@app.route('/predict', methods=['POST'])
def predict():
    try:
        # Parse input
        input_data = request.json
        input_text = input_data.get("text", "")

        # Preprocess and predict
        processed_text = preprocess_text(input_text)
        predicted_constraints = classifier.predict(processed_text)

        if len(predicted_constraints) > 0:
            return jsonify({"constraint_type": predicted_constraints[0]})
        else:
            return jsonify({"constraint_type": "UNKNOWN"}), 404
    except Exception as e:
        return jsonify({"error": str(e)}), 500

if __name__ == '__main__':
    app.run(host="0.0.0.0", port=5001)