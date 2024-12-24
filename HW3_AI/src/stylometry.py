import spacy
import re
from collections import Counter

# Load Romanian spaCy model once
try:
    ro_nlp = spacy.load("ro_core_news_sm")
except OSError:
    ro_nlp = None

def get_stylometric_info(text):
    """
    Computes stylometric information for Romanian text:
      - number of words
      - number of characters (excluding punctuation/spaces)
      - a frequency dictionary of words (top 10)

    Args:
        text (str): The input text.

    Returns:
        dict: {
            "num_words": int,
            "num_chars": int,
            "word_frequency_top10": list of (word, count)
        }
    """
    if not ro_nlp:
        raise RuntimeError(
            "Romanian spaCy model not loaded. "
            "Run 'python -m spacy download ro_core_news_sm'."
        )

    # 1. Tokenize
    doc = ro_nlp(text)

    # 2. Extract alphabetic tokens only
    tokens = [token.text for token in doc if token.is_alpha]

    # 3. Normalize to lowercase
    words_lower = [w.lower() for w in tokens]

    # 4. Count words and characters
    num_words = len(words_lower)
    num_chars = sum(len(w) for w in words_lower)

    # 5. Word frequency (top 10)
    freq_counter = Counter(words_lower)
    word_frequency_top10 = freq_counter.most_common(10)

    return {
        "num_words": num_words,
        "num_chars": num_chars,
        "word_frequency_top10": word_frequency_top10
    }