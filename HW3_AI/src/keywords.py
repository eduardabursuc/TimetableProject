from keybert import KeyBERT
from transformers import pipeline, AutoTokenizer, AutoModel

# Hardcoded Romanian stopwords
romanian_stopwords = [
    "acea", "aceasta", "această", "aceea", "acei", "aceia", "acel", "acela", "acele", "acelea", "acest", "acesta",
    "aceste", "acestea", "aceşti", "aceştia", "aceștia", "acolo", "acord", "acum", "ai", "aia", "aibă", "aici", "al", "ăla", "ale",
    "alea", "ălea", "altceva", "altcineva", "am", "ar", "are", "aș", "aş", "așadar", "aşadar", "asemenea", "asta", "ăsta", "astăzi",
    "astea", "ăstea", "ăştia", "ăștia", "asupra", "ați", "aţi", "au", "avea", "avem", "aveți", "aveţi", "azi", "bine", "bucur", "bună", "ca",
    "că", "căci", "când", "cînd", "care", "cărei", "căror", "cărui", "cât", "cît", "câte", "cîte", "câți", "cîți", "către", "câtva", "cîtva", "caut", "ce",
    "cel", "ceva", "chiar", "cinci", "cine", "cineva", "contra", "cu", "cum",
    "cumva", "curând", "curînd", "da", "dă", "dacă", "dar", "dată", "datorită", "dau", "de", "deci", "deja", "deoarece",
    "departe", "deși", "deşi", "din", "dinaintea", "dintr-", "dintre", "doi", "doilea", "două", "drept", "după", "ea", "ei", "el",
    "ele", "eram", "este", "ești", "eşti", "eu", "face", "fără", "fata", "fi", "fie", "fiecare", "fii", "fim", "fiți", "fiţi", "fiu",
    "frumos", "grație", "graţie", "halbă", "iar", "ieri", "îi", "îl", "îmi", "împotriva", "în", "înainte", "înaintea", "încât",
    "încît", "încotro", "între", "întrucât", "întrucît", "îți", "îţi", "la", "lângă", "lîngă", "le", "li", "lor", "lui", "mă",
    "mai", "mâine", "mîine", "mea", "mei", "mele", "mereu", "meu", "mi", "mie", "mine", "mult", "multă", "mulți", "mulţi",
    "mulțumesc", "mulţumesc", "ne", "nevoie", "nicăieri", "nici", "nimeni", "nimeri", "nimic", "niște", "nişte", "noastră", "noastre", "noi",
    "noroc", "noștri", "noştri", "nostru", "nouă", "nu", "opt", "ori", "oricând", "oricînd", "oricare", "oricât", "oricît", "orice", "oricine",
    "oricum", "oriunde", "până", "pînă", "patra", "patru", "patrulea", "pe", "pentru", "peste", "pic", "poate",
    "pot", "prea", "prima", "primul", "prin", "printr-", "puțin", "puţin", "puțina", "puţina", "puțină", "puţină", "rog", "sa", "să", "săi", "sale",
    "șapte", "şapte", "șase", "şase", "sau", "său", "se", "și", "şi", "sînt", "sunt", "sîntem", "suntem", "sînteți", "sunteți", "sînteţi", "sunteţi", "spate", "spre", "știu", "ştiu", "sub", "sută", "ta", "tăi", "tale", "tău", "te", "ți", "ţi", "ție", "ţie", "timp", "tine", "toată", "toate", "tot",
    "toți", "toţi", "totuși", "totuşi", "trei", "treia", "treilea", "tu", "un", "una", "unde", "undeva", "unei", "uneia", "unele", "uneori",
    "unii", "unor", "unora", "unu", "unui", "unuia", "unul", "vă", "vi", "voastră", "voastre", "voi", "voștri", "voştri", "vostru",
    "vouă", "vreme", "vreo", "vreun", "zece", "zero", "zi", "zice"
]

from keybert import KeyBERT
from transformers import AutoTokenizer, AutoModel

tokenizer = AutoTokenizer.from_pretrained("dumitrescustefan/bert-base-romanian-cased-v1")
model = AutoModel.from_pretrained("dumitrescustefan/bert-base-romanian-cased-v1")

def extract_keywords(text, num_keywords=7):
    max_length = 512
    tokens = tokenizer(text, truncation=True, max_length=max_length, return_tensors="pt")
    truncated_text = tokenizer.decode(tokens["input_ids"][0], skip_special_tokens=True)

    kw_model = KeyBERT(model=model)
    keywords = kw_model.extract_keywords(truncated_text, keyphrase_ngram_range=(1, 1), stop_words=romanian_stopwords, top_n=num_keywords)
    return [kw[0] for kw in keywords]


from openai import OpenAI
import os

# Initialize the OpenAI client
client = OpenAI(api_key=os.getenv("OPENAI_API_KEY"))

def generate_sentences_for_keywords(keywords, original_text, max_tokens_per_sentence=50):
    """
    Generate one creative sentence for each keyword in the given context.

    Args:
        keywords (list of str): List of keywords to use in sentences.
        original_text (str): Context text for generating sentences.
        max_tokens_per_sentence (int): Max tokens for each sentence.

    Returns:
        list of str: List of generated sentences for each keyword.
    """
    # Construct the messages for chat completion
    keywords_list = ", ".join(f"'{kw}'" for kw in keywords)
    messages = [
        {"role": "system", "content": "You are a creative assistant skilled in generating sentences in romanian language for keywords."},
        {
            "role": "user",
            "content": (
                f"Generate one creative sentence for each of the following keywords: {keywords_list}. "
                f"The sentences generated should keep the original meaning of the keywords used in this context: '{original_text}' but be used in another context. You can play around."
                "Each sentence should be on a new line and in romanian language. The sencence doesn't necesarrily have to start with the keyword."
            )
        }
    ]

    try:
        # Call OpenAI Chat API
        response = client.chat.completions.create(
            model="gpt-4o-mini",
            messages=messages,
            n=1
        )
        # Access the message content correctly
        content = response.choices[0].message.content.strip()
        sentences = content.split("\n")
        return [sentence.strip() for sentence in sentences if sentence.strip()]
    except Exception as e:
        # Handle errors gracefully
        print(f"[ERROR] Failed to generate sentences: {e}")
        return [f"Am regăsit cuvântul-cheie '{kw}' în textul original." for kw in keywords]