from linear_regression import RidgeConstraintClassifier
from data_matching import match_and_validate_data
from input_preprocessing import preprocess_text
from psycopg2.extras import RealDictCursor
from flask import Flask, request, jsonify
from _project_setup import user_email
from db_helper import DatabaseHelper
from flask_cors import CORS

# Initialize Flask
app = Flask(__name__)
CORS(app)

# Database connection
CONNECTION_STRING = "host=ep-solitary-sun-a298zpeg.eu-central-1.aws.neon.tech dbname=TimeTable user=TimeTable_owner password=NA5uy3rKWIFQ"
db_helper = DatabaseHelper(CONNECTION_STRING)

classifier = RidgeConstraintClassifier()
classifier.load_model("ridge_model.pkl")

@app.route('/create_constraint', methods=['POST'])
def create_constraint():
    print("[DEBUG] Starting create_constraint endpoint...")
    try:
        # 1) Parse input JSON
        input_data = request.json
        print(f"[DEBUG] Raw input_data: {input_data}")
        if not input_data:
            print("[DEBUG] No JSON body provided.")
            return jsonify({"error": "No JSON body provided."}), 400

        input = input_data.get("input", "")
        professorEmail = input_data.get("professorEmail", "")
        timetableId = input_data.get("timetableId", "")

        print(f"[DEBUG] input: {input}")
        print(f"[DEBUG] professorEmail: {professorEmail}")
        print(f"[DEBUG] timetableId: {timetableId}")
        print(f"[DEBUG] global user_email: {user_email}")

        # Basic sanity checks
        if not input or not user_email or not professorEmail or not timetableId:
            print("[DEBUG] Missing required fields.")
            return jsonify({
                "error": "Missing required fields: 'input', 'user_email', 'professorEmail', 'timetableId'."
            }), 400

        # 2) Fetch user data from DB
        print("[DEBUG] Fetching user_data from DB...")
        user_data = db_helper.fetch_user_data(user_email)
        if not user_data:
            print(f"[DEBUG] Failed to fetch user data for email: {user_email}")
            return jsonify({"error": f"Failed to fetch user data for email: {user_email}"}), 400

        # 3) Preprocess and predict
        print("[DEBUG] Preprocessing input for model prediction...")
        processed_text = preprocess_text(input)
        predicted_constraints = classifier.predict(processed_text)
        print(f"[DEBUG] Predicted constraints: {predicted_constraints}")
        if not predicted_constraints:
            print("[DEBUG] Unable to predict constraint type")
            return jsonify({"error": "Unable to predict constraint type"}), 400

        predicted_constraint = predicted_constraints[0]
        print(f"[DEBUG] Final predicted constraint: {predicted_constraint}")

        # 4) Match & Validate data
        print("[DEBUG] Matching and validating data...")
        matched_data = match_and_validate_data(input, predicted_constraint, user_data, professorEmail)
        print(f"[DEBUG] matched_data: {matched_data}")

        # If an error dictionary is returned, send that back
        if isinstance(matched_data, dict) and "error" in matched_data:
            print("[DEBUG] Error in matched_data.")
            return jsonify({
                "error": matched_data["error"], 
                "constraint_type": predicted_constraint
            }), 400

        # 5) Insert the constraint into DB
        print("[DEBUG] Inserting new constraint into DB...")
        insert_result = db_helper.create_new_constraint(
            timetableId=timetableId,
            constraint_type=predicted_constraint,
            matched_data=matched_data,
            user_data=user_data
        )
        print(f"[DEBUG] insert_result: {insert_result}")

        # Check if insertion succeeded
        if insert_result["IsSuccess"]:
            print(f"[DEBUG] Insert successful. New constraint ID: {insert_result['Data']['Id']}")
            return jsonify({
                "id": insert_result["Data"]["Id"]
            }), 200
        else:
            print("[DEBUG] Insert failed.")
            return jsonify({
                "error": insert_result["ErrorMessage"],
                "constraint_type": predicted_constraint
            }), 500

    except Exception as e:
        print(f"[DEBUG] Exception: {e}")
        return jsonify({"error": str(e)}), 500

if __name__ == '__main__':
    print("[DEBUG] Starting Flask app on port 5001...")
    app.run(host="0.0.0.0", port=5001)