import sys
import os
from text_reader import read_text
from language_detection import detect_language
from stylometry import get_stylometric_info
from transformations import generate_advanced_alternative_text
from keywords import extract_keywords, generate_sentences_for_keywords

# Disable the tokenizers parallelism warning
os.environ["TOKENIZERS_PARALLELISM"] = "false"
# We only allow Romanian
SUPPORTED_LANGUAGES = ['ro']

def main():
    # 1. Read text
    input_path = sys.argv[1] if len(sys.argv) > 1 else None
    try:
        text = read_text(input_path)
    except FileNotFoundError as e:
        print(f"[ERROR] Fișierul specificat nu a fost găsit: {e}")
        return
    except IOError as e:
        print(f"[ERROR] Eroare la citirea fișierului: {e}")
        return

    print("[INFO] Text citit cu succes.")

    # 2. Detect language
    lang_code = detect_language(text)
    print(f"[INFO] Limba detectată: {lang_code}")

    # 3. Verify language (only 'ro' supported)
    if lang_code not in SUPPORTED_LANGUAGES:
        print(f"[ERROR] Limba detectată '{lang_code}' nu este suportată. "
              f"Vă rugăm să introduceți text în limba română.")
        return

    # 4. Stylometric analysis
    print("[INFO] Analiza stilometrică în curs...")
    info = get_stylometric_info(text)
    print("\n\n=== Analiză stilometrică ===")
    print(f"Număr de cuvinte: {info['num_words']}")
    print(f"Număr de caractere (fără spații/punctuație): {info['num_chars']}")
    print("Top 10 cuvinte frecvente:")
    for word, count in info["word_frequency_top10"]:
        print(f"  {word} -> {count}")

    # 5. Generate advanced alternative text (20% replaced)
    print("[INFO] Generare text alternativ în curs...")
    try:
        alt_text = generate_advanced_alternative_text(
            text,
            replacement_ratio=0.9
        )
        print("\n\n=== Text alternativ (cuvinte înlocuite cu sinonime/hiperonime) ===")
        print(alt_text)
    except RuntimeError as e:
        print(f"[ERROR] Generarea textului alternativ a eșuat: {e}")
        return

    # 6. Keyword extraction (on original text)
    print("[INFO] Extracție de cuvinte-cheie în curs...")
    kw = extract_keywords(text, num_keywords=7)
    print("\n\n=== Cuvinte-cheie ===")
    print(kw)

    # 7. Generate sentences for each keyword
    print("[INFO] Generare propoziții pentru cuvinte-cheie în curs...")
    sentences = generate_sentences_for_keywords(kw, text)
    print("\n\n=== Propoziții pentru cuvinte-cheie ===")
    for s in sentences:
        print(s)

if __name__ == "__main__":
    import multiprocessing
    multiprocessing.freeze_support()
    main()