import re
import os
from nltk.corpus import stopwords
from nltk.tokenize import word_tokenize
from nltk.stem import WordNetLemmatizer
from symspellpy import SymSpell, Verbosity

# ================ Additional Setup ================

lemmatizer = WordNetLemmatizer()

CONTRACTION_MAPPING = {
    "don't": "do not",
    "can't": "can not",
    "won't": "will not",
    "isn't": "is not",
    "aren't": "are not",
    "doesn't": "does not",
    "didn't": "did not",
    "haven't": "have not",
    "hasn't": "has not",
    "hadn't": "had not",
    "shouldn't": "should not",
    "wouldn't": "would not",
    "couldn't": "could not",
    "n't": " not"
}

SLANG_MAPPING = {
    "gonna": "going to",
    "wanna": "want to",
    "u": "you",
    "cuz": "because",
    "aint": "is not",
    "dont": "do not",
    "cant": "can not",
    "couldnt": "could not",
    "didnt": "did not",
    "doesnt": "does not",
    "isnt": "is not",
    "wont": "will not"
}

# no longer using this on step 9 because it lowers prediction accuracy
# stop_words = set(stopwords.words('english'))

IRRELEVANT_WORDS = {
    "please", "kindly", "thank", "thanks",
    "would", "like", "maybe", "just", "really", "actually"
}

# Static Synonym Dictionary
SYNONYMS = {
    # General Terms
    "classroom": "room",
    "lecture": "class",
    "seminar": "class",
    "course": "class",
    "lab": "class",
    "busy": "unavailable",
    "free": "available",
    "teacher": "instructor",
    "professor": "instructor",
    "tutor": "instructor",
    "exam": "test",
    "assignment": "task",
    "task": "assignment",
    "start": "begin",
    "end": "finish",
    "morning": "early",
    "afternoon": "later",
    "evening": "later",
    "early": "morning",
    "later": "afternoon",
    
    # Time-Related
    "reschedule": "change",
    "shift": "change",
    "move": "change",
    "postpone": "change",
    "delay": "change",
    "earlier": "before",
    "later": "after",

    # Days of the Week
    "monday": "weekday",
    "tuesday": "weekday",
    "wednesday": "weekday",
    "thursday": "weekday",
    "friday": "weekday",
    "saturday": "weekend",
    "sunday": "weekend",

    # Availability
    "unavailable": "not available",
    "available": "free",
    "accessible": "free",
    "open": "available",
    "occupied": "unavailable",

    # Preferences
    "prefer": "want",
    "would like": "want",
    "desire": "want",
    "need": "require",
    "require": "want",

    # Locations
    "room": "location",
    "classroom": "location",
    "lab": "location",
    "venue": "location",
    "building": "location",

    # Weeks
    "even week": "biweekly",
    "odd week": "biweekly",
    "biweekly": "alternate",

    # Breaks and Windows
    "pause": "break",
    "gap": "break",
    "interval": "break",
    "window": "break",
    "rest": "break",

    # Scheduling
    "schedule": "plan",
    "assign": "plan",
    "arrange": "plan",
    "organize": "plan",
    "set": "plan",
    "block": "mark",

    # Other
    "make sure": "ensure",
    "guarantee": "ensure",
    "confirm": "ensure"
}

# SymSpell Setup (Spelling Correction)
sym_spell = SymSpell(max_dictionary_edit_distance=2, prefix_length=7)
dictionary_path = "./frequency_dict.txt"
sym_spell.load_dictionary(dictionary_path, term_index=0, count_index=1)

# ================ Helper Functions ================

def expand_contractions(text: str) -> str:
    """
    Expand common contractions in the text based on CONTRACTION_MAPPING.
    """
    # Simple whitespace split to handle tokens
    tokens = text.split()
    expanded_tokens = []
    for token in tokens:
        lower_token = token.lower()
        if lower_token in CONTRACTION_MAPPING:
            expanded_tokens.append(CONTRACTION_MAPPING[lower_token])
        else:
            expanded_tokens.append(token)
    return " ".join(expanded_tokens)

def symspell_correct_text(text: str) -> str:
    """
    Spelling correction using SymSpell for each token.
    """
    words = text.split()
    corrected_words = []
    for w in words:
        suggestions = sym_spell.lookup(w, Verbosity.CLOSEST, max_edit_distance=2)
        if suggestions:
            corrected_words.append(suggestions[0].term)
        else:
            corrected_words.append(w)
    return " ".join(corrected_words)

def remove_html_and_urls(text: str) -> str:
    """
    Remove any HTML tags and URLs from the text.
    """
    text = re.sub(r"<[^>]*>", "", text)  # remove HTML tags
    text = re.sub(r"http\S+|www\.\S+", "", text)  # remove URLs
    return text

def replace_slang(text: str) -> str:
    """
    Replace common slang words or phrases with their formal equivalents.
    """
    tokens = text.split()
    replaced_tokens = []
    for token in tokens:
        # Remove punctuation from token to match dictionary if needed
        cleaned_token = re.sub(r"[^\w\s]", "", token).lower()
        # First check CONTRACTION_MAPPING, then SLANG_MAPPING
        replaced_tokens.append(
            SLANG_MAPPING.get(cleaned_token, token)
        )
    return " ".join(replaced_tokens)

def static_synonym_replacement(tokens):
    """
    Replace tokens with their synonyms using a static dictionary.
    """
    return [SYNONYMS.get(t, t) for t in tokens]

def handle_negations(tokens):
    """
    Combine negation words (not, no, n't) with the following token.
    Example: "not available" -> "not_available"
    """
    updated_tokens = []
    skip_next = False

    for i in range(len(tokens)):
        if skip_next:
            skip_next = False
            continue

        # Check if current token is a negation
        if tokens[i] in {"not", "no", "n't"}:
            if i + 1 < len(tokens):
                updated_tokens.append(f"not_{tokens[i + 1]}")
                skip_next = True  # skip the next token
            else:
                updated_tokens.append(tokens[i])
        else:
            updated_tokens.append(tokens[i])

    return updated_tokens

# ================ Main Preprocessing Pipeline ================
def preprocess_text(text: str) -> str:
    """
    Preprocess the input text by applying multiple NLP techniques.
    Args:
        text (str): The raw input text.
    Returns:
        str: The preprocessed text.
    """

    # 1. Remove HTML & URLs
    text = remove_html_and_urls(text)

    # 2. Expand Contractions
    text = expand_contractions(text)

    # 3. Handle Slang
    text = replace_slang(text)

    # 4. Spelling Correction (kind of out of control, so disabled for now)
    # text = symspell_correct_text(text)

    # 5. Lowercase
    text = text.lower()

    # 6. Remove Punctuation
    text = re.sub(r"[^\w\s]", "", text)

    # 7. Tokenization
    tokens = word_tokenize(text)

    # 8. Negation Handling
    tokens = handle_negations(tokens)

    # 9. Remove Irrelevant Words (and Stopwords)
    tokens = [t for t in tokens if t not in IRRELEVANT_WORDS] # or t not in stop_words]

    # 10. Lemmatization
    tokens = [lemmatizer.lemmatize(t) for t in tokens]

    # 11. Handle Numbers
    tokens = [re.sub(r"\d+", "number", t) for t in tokens]

    # 12. Synonym Replacement (Static Dictionary)
    tokens = static_synonym_replacement(tokens)

    # 13. Remove Empty Tokens (in case we created any blanks)
    tokens = [t for t in tokens if t]

    return " ".join(tokens)

# ================ Example Usage ================
if __name__ == "__main__":
    raw_text = """
    The lecture scheduled for <b>Monday at 9:00 AM</b> in room #B104 needs to be rescheduled due to maintenance issues. 
    Please refer to the maintenance schedule at https://university.edu/maintenance for more details.

    I do not want the seminar to be after the lecture on Tuesday at 2:00 PM. 
    The current room assignment is not suitable as it is too small to accommodate all students. 
    Could you please move the class to a larger room?

    Thank you for your understanding and assistance in this matter.
    """
    preprocessed = preprocess_text(raw_text)
    print("Original:", raw_text)
    print("\nPreprocessed:", preprocessed)