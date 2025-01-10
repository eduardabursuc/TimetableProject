import sys
import json
from linear_regression import RidgeConstraintClassifier
from input_preprocessing import preprocess_text

def main():
    # Load model
    classifier = RidgeConstraintClassifier()
    try:
        classifier.load_model("ridge_model.pkl")
    except FileNotFoundError:
        print("Error: Trained model not found. Please train and save the model first.")
        sys.exit(1)
    
    # Check if input is provided
    if len(sys.argv) < 2:
        print("Error: No input provided. Please pass the natural language input as an argument.")
        sys.exit(1)
    
    # Get the input text
    input_text = " ".join(sys.argv[1:])
    print(f"Input Text: {input_text}")
    
    # Preprocess and predict
    processed_text = preprocess_text(input_text)
    predicted_constraints = classifier.predict(processed_text)

    # Return results as JSON
    result = {
        "input": input_text,
        "processed": processed_text,
        "predicted_constraints": predicted_constraints
    }
    print(json.dumps(result, indent=4))

if __name__ == "__main__":
    main()