import json
import random
import spacy
import os

try:
    ro_nlp = spacy.load("ro_core_news_sm")
except OSError:
    ro_nlp = None

# Global dictionaries populated from the JSON
lemma_to_synsets = {}
synset_to_lemmas = {}
relations = {"hypernym": {}, "hyponym": {}, "instance_hypernym": {}}

#############################################
# 1) PARSE THE ROMANIAN WORDNET JSON
#############################################
def parse_romanian_wordnet(json_path):
    """
    Reads the Romanian WordNet JSON file and builds dictionaries for:
      - lemma_to_synsets
      - synset_to_lemmas
      - relations (hypernym, hyponym, instance_hypernym)
    """
    with open(json_path, "r", encoding="utf-8-sig") as f:
        data = json.load(f)

    graph_list = data.get("@graph", [])
    if not graph_list:
        raise ValueError("The JSON does not contain '@graph' or it's empty.")

    lexicon_obj = None
    for obj in graph_list:
        if "entry" in obj or "synset" in obj:
            lexicon_obj = obj
            break

    if not lexicon_obj:
        raise ValueError("No object with 'entry' or 'synset' found in '@graph'.")

    entries = lexicon_obj.get("entry", [])
    synsets = lexicon_obj.get("synset", [])

    _lemma_to_synsets = {}
    _synset_to_lemmas = {}
    _relations = {"hypernym": {}, "hyponym": {}, "instance_hypernym": {}}

    # Build lemma <-> synset references
    for entry in entries:
        lemma_info = entry.get("lemma", {})
        written_form = lemma_info.get("writtenForm")
        if not written_form:
            continue

        senses = entry.get("sense", [])
        for sense_obj in senses:
            sid = sense_obj.get("synsetRef")
            if sid:
                lemma_lower = written_form.lower()
                _lemma_to_synsets.setdefault(lemma_lower, []).append(sid)
                _synset_to_lemmas.setdefault(sid, []).append(written_form)

    # Build relations
    for syn_obj in synsets:
        syn_id = syn_obj.get("@id")
        if not syn_id:
            continue

        relations = syn_obj.get("relations", [])
        for rel in relations:
            rel_type = rel.get("relType")
            target_syn = rel.get("target")
            if rel_type in _relations and target_syn:
                _relations[rel_type].setdefault(syn_id, []).append(target_syn)

    return _lemma_to_synsets, _synset_to_lemmas, _relations


#############################################
# 2) INITIALIZE (PARSE ONCE AT MODULE LOAD)
#############################################
# We assume the JSON is in the same folder, named "romanian_dictionary.json"
json_file_path = os.path.join(os.path.dirname(__file__), "romanian_dictionary.json")
try:
    (lemma_to_synsets,
     synset_to_lemmas,
     relations) = parse_romanian_wordnet(json_file_path)
except FileNotFoundError:
    print("[WARNING] 'romanian_dictionary.json' not found. Synonym/hypernym lookups will fail.")
except Exception as e:
    print(f"[WARNING] Problem parsing 'romanian_dictionary.json': {e}")


#############################################
# 3) HELPER FUNCTIONS: SYNONYMS, HYPERNYMS, ETC.
#############################################
def get_replacements_from_relations(word, relation_type):
    """
    Returns replacements for `word` using a specific relationship type.
    """
    w_lower = word.lower()
    if w_lower not in lemma_to_synsets:
        return []

    replacements = set()
    for sid in lemma_to_synsets[w_lower]:
        related_synsets = relations.get(relation_type, {}).get(sid, [])
        for related_sid in related_synsets:
            replacements.update(synset_to_lemmas.get(related_sid, []))
    return list(replacements)


def get_synonym_or_hypernym_candidates(word):
    """
    Combine replacements from multiple relationship types:
    - synonyms
    - hypernyms
    - hyponyms
    - instance hypernyms
    """
    syns = get_replacements_from_relations(word, "similar")  # Synonyms
    hyps = get_replacements_from_relations(word, "hypernym")  # Hypernyms
    hypos = get_replacements_from_relations(word, "hyponym")  # Hyponyms
    inst_hyps = get_replacements_from_relations(word, "instance_hypernym")  # Instance Hypernyms

    combined = syns + hyps + hypos + inst_hyps
    return list(set(combined))  # Remove duplicates


#############################################
# 4) CONTEXTUAL REPLACEMENT + TEXT REWRITE
#############################################
def get_contextual_replacements_ro(word, pos_tag):
    """
    For Romanian, we consider synonyms/hypernyms/hyponyms only if POS is NOUN, ADJ, VERB, ADV.
    """
    if pos_tag not in ["NOUN", "ADJ", "VERB", "ADV"]:
        return []

    return get_synonym_or_hypernym_candidates(word)


def generate_advanced_alternative_text(text, replacement_ratio=0.2):
    """
    Generate alternative Romanian text by replacing ~20% of tokens
    with synonyms/hypernyms/hyponyms/instance hypernyms.
    """
    if not ro_nlp:
        raise RuntimeError("Romanian spaCy model not loaded.")

    doc = ro_nlp(text)

    # Identify tokens to consider for replacement
    eligible_tokens = [
        (i, token.text, token.pos_)
        for i, token in enumerate(doc)
        if token.is_alpha and not token.is_stop
    ]

    num_to_replace = max(1, int(len(eligible_tokens) * replacement_ratio))
    to_replace = random.sample(eligible_tokens, num_to_replace)

    new_tokens = list(doc)  # spaCy Doc is not directly mutable, so we build a list

    for (i, original_word, pos_tag) in to_replace:
        replacements = get_contextual_replacements_ro(original_word.lower(), pos_tag)
        if replacements:
            replacement = random.choice(replacements)
            new_tokens[i] = replacement

    return " ".join(str(token) for token in new_tokens)