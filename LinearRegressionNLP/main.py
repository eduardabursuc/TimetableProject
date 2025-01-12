# main.py

import sys
import json
from linear_regression import RidgeConstraintClassifier
from input_preprocessing import preprocess_text
from db_helper import DatabaseHelper
from data_matching import match_and_validate_data

# Database connection string
CONNECTION_STRING = "host=ep-solitary-sun-a298zpeg.eu-central-1.aws.neon.tech dbname=TimeTable user=TimeTable_owner password=NA5uy3rKWIFQ"

def main():
    if len(sys.argv) < 5:
        print("Error: Insufficient input. Provide natural language input, user email, professor email and timetable id.")
        sys.exit(1)

    input_text = sys.argv[1]
    user_email = sys.argv[2]
    professor_email = sys.argv[3]
    timetable_id = sys.argv[4]

    # Initialize database helper
    db_helper = DatabaseHelper(CONNECTION_STRING)
    user_data = db_helper.fetch_user_data(user_email)

    if not user_data:
        print("Error: Failed to fetch user data.")
        sys.exit(1)

    # Load trained model
    classifier = RidgeConstraintClassifier()
    try:
        classifier.load_model("ridge_model.pkl")
    except FileNotFoundError:
        print("Error: Trained model not found. Please train and save the model first.")
        sys.exit(1)

    # Preprocess input and predict constraint type
    processed_text = preprocess_text(input_text)
    predicted_constraint = classifier.predict(processed_text)

    # Match and validate data
    matched_data = match_and_validate_data(input_text, predicted_constraint[0], user_data, professor_email)

    result = {
        "input": input_text,
        "predicted_constraint": predicted_constraint,
        "matched_data": matched_data
    }
    print(json.dumps(result, indent=3))

if __name__ == "__main__":
    main()