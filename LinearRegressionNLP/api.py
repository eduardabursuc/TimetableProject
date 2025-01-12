from flask import Flask, request, jsonify
from linear_regression import RidgeConstraintClassifier
from input_preprocessing import preprocess_text
from db_helper import DatabaseHelper
from data_matching import match_and_validate_data
from psycopg2.extras import RealDictCursor

# Initialize Flask
app = Flask(__name__)

# Database connection
CONNECTION_STRING = "host=ep-solitary-sun-a298zpeg.eu-central-1.aws.neon.tech dbname=TimeTable user=TimeTable_owner password=NA5uy3rKWIFQ"
db_helper = DatabaseHelper(CONNECTION_STRING)

classifier = RidgeConstraintClassifier()
classifier.load_model("ridge_model.pkl")

@app.route('/create_constraint', methods=['POST'])
def create_constraint():
    """
    Endpoint that:
      1) Receives JSON input containing:
         {
            "text": "...",
            "user_email": "...",
            "professor_email": "...",
            "timetable_id": "..."
         }
      2) Predicts constraint type from 'text'.
      3) Matches & validates data from 'text' + user DB context.
      4) Inserts new constraint into "constraints" table if everything is valid.
      5) Returns success/failure JSON.
    """
    try:
        # 1) Parse input JSON
        input_data = request.json
        if not input_data:
            return jsonify({"error": "No JSON body provided."}), 400

        text = input_data.get("text", "")
        user_email = input_data.get("user_email", "")
        professor_email = input_data.get("professor_email", "")
        timetable_id = input_data.get("timetable_id", "")

        # Basic sanity checks
        if not text or not user_email or not professor_email or not timetable_id:
            return jsonify({
                "error": "Missing required fields: 'text', 'user_email', 'professor_email', 'timetable_id'."
            }), 400

        # 2) Fetch user data from DB
        user_data = db_helper.fetch_user_data(user_email)
        if not user_data:
            return jsonify({"error": f"Failed to fetch user data for email: {user_email}"}), 400

        # 3) Preprocess and predict
        processed_text = preprocess_text(text)
        predicted_constraints = classifier.predict(processed_text)
        if not predicted_constraints:
            # No constraint predicted
            return jsonify({"error": "Unable to predict constraint type"}), 400

        predicted_constraint = predicted_constraints[0]

        # 4) Match & Validate data
        matched_data = match_and_validate_data(text, predicted_constraint, user_data, professor_email)

        # If an error dictionary is returned, send that back
        if isinstance(matched_data, dict) and "error" in matched_data:
            return jsonify({
                "error": matched_data["error"], 
                "constraint_type": predicted_constraint
            }), 400

        # 5) Insert the constraint into DB
        insert_result = db_helper.create_new_constraint(
            timetable_id=timetable_id,
            constraint_type=predicted_constraint,
            matched_data=matched_data,
            user_data=user_data
        )

        # Check if insertion succeeded
        if insert_result["IsSuccess"]:
            return jsonify({
                "constraint_type": predicted_constraint,
                "matched_data": matched_data,
                "insertion_result": insert_result
            }), 200
        else:
            return jsonify({
                "error": insert_result["ErrorMessage"],
                "constraint_type": predicted_constraint
            }), 500

    except Exception as e:
        return jsonify({"error": str(e)}), 500


if __name__ == '__main__':
    app.run(host="0.0.0.0", port=5001)