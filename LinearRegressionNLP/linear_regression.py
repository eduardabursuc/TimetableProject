import pandas as pd
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.linear_model import RidgeClassifier
from sklearn.model_selection import train_test_split
from sklearn.multioutput import ClassifierChain
from sklearn.preprocessing import MultiLabelBinarizer
from sklearn.metrics import jaccard_score, hamming_loss
import joblib

from input_preprocessing import preprocess_text

CONSTRAINT_TYPES = [
    "SOFT_ROOM_CHANGE",
    "SOFT_ROOM_PREFERENCE",
    "SOFT_TIME_CHANGE",
    "SOFT_DAY_CHANGE",
    "SOFT_ADD_WINDOW",
    "SOFT_REMOVE_WINDOW",
    "SOFT_DAY_OFF",
    "SOFT_WEEK_EVENNESS",
    "SOFT_CONSECUTIVE_HOURS",
    "SOFT_INTERVAL_AVAILABILITY",
    "SOFT_INTERVAL_UNAVAILABILITY",
    "SOFT_LECTURE_BEFORE_LABS"
]

class RidgeConstraintClassifier:
    """Multi-label Linear Regression model using ClassifierChain."""

    def __init__(self):
        # Use ngram_range=(1, 2) for bigrams and unigrams
        self.vectorizer = TfidfVectorizer(preprocessor=None, tokenizer=None, lowercase=False, ngram_range=(1, 2), max_features=5000)
        base_model = RidgeClassifier(random_state=42)
        self.chain = ClassifierChain(base_model, order='random', random_state=42)
        self.mlb = MultiLabelBinarizer(classes=CONSTRAINT_TYPES)
        self.is_trained = False
    
    def train(self, csv_path="training_data.csv", test_size=0.2):
        df = pd.read_csv(csv_path)
        df["processed_text"] = df["text"].apply(preprocess_text)
        df["label_list"] = df["label"].apply(lambda x: x.split(","))
        Y = self.mlb.fit_transform(df["label_list"])

        X_train, X_test, y_train, y_test = train_test_split(
            df["processed_text"], Y, test_size=test_size, random_state=42
        )

        X_train_vec = self.vectorizer.fit_transform(X_train)
        X_test_vec = self.vectorizer.transform(X_test)

        self.chain.fit(X_train_vec, y_train)
        self.is_trained = True
        
        y_pred = self.chain.predict(X_test_vec)

        # --- Print Additional Metrics ---
        # 1. Jaccard Score (samples)
        jaccard = jaccard_score(y_test, y_pred, average="samples")
        print(f"Jaccard Score (samples): {jaccard:.3f}")

        # 2. Hamming Loss
        hloss = hamming_loss(y_test, y_pred)
        print(f"Hamming Loss: {hloss:.3f}")

        # 3. Exact Match / Subset Accuracy
        exact_matches = 0
        for true, pred in zip(y_test, y_pred):
            if (true == pred).all():
                exact_matches += 1
        subset_accuracy = exact_matches / len(y_test)
        print(f"Exact Match Ratio (Subset Accuracy): {subset_accuracy:.3f}")

        # Print label counts
        y_pred_labels = self.mlb.inverse_transform(y_pred)
        y_test_labels = self.mlb.inverse_transform(y_test)
        print("\nDetailed Label Counts:")
        self._print_multilabel_classification_report(y_test_labels, y_pred_labels)
    
    def _print_multilabel_classification_report(self, y_true_lists, y_pred_lists):
        all_labels_true, all_labels_pred = [], []
        for t_labels, p_labels in zip(y_true_lists, y_pred_lists):
            all_labels_true.extend(t_labels)
            all_labels_pred.extend(p_labels)

        # Convert to series for proper value_counts usage
        label_counts_true = pd.Series(all_labels_true).value_counts()
        label_counts_pred = pd.Series(all_labels_pred).value_counts()

        print("True label counts:\n", label_counts_true.to_string())
        print("\nPredicted label counts:\n", label_counts_pred.to_string())

    def predict(self, text: str) -> list:
        if not self.is_trained:
            raise ValueError("Model is not trained yet.")
        processed = preprocess_text(text)
        vec = self.vectorizer.transform([processed])
        pred_binary = self.chain.predict(vec)[0]
        return [self.mlb.classes_[i] for i, val in enumerate(pred_binary) if val == 1]
    
    def save_model(self, model_path="ridge_model.pkl"):
        if not self.is_trained:
            raise ValueError("Model is not trained.")
        joblib.dump({
            "chain": self.chain,
            "vectorizer": self.vectorizer,
            "mlb": self.mlb
        }, model_path)
        print(f"Model saved to {model_path}")
    
    def load_model(self, model_path="ridge_model.pkl"):
        data = joblib.load(model_path)
        self.chain = data["chain"]
        self.vectorizer = data["vectorizer"]
        self.mlb = data["mlb"]
        self.is_trained = True
        print(f"Model loaded from {model_path}")

if __name__ == "__main__":
    classifier = RidgeConstraintClassifier()
    classifier.train(csv_path="training_data.csv", test_size=0.2)
    classifier.save_model("ridge_model.pkl")