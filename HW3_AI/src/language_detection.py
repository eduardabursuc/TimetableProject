from langdetect import detect, DetectorFactory

DetectorFactory.seed = 0

def detect_language(text):
    """
    Detects the language of the given text string. If not Romanian, we won't proceed.

    Args:
        text (str): The text to analyze.

    Returns:
        str: 'ro' if Romanian is detected, otherwise the detected code.
             Returns 'unknown' if detection is not possible or text is empty.
    """
    text = text.strip()
    if not text:
        return 'unknown'
    try:
        return detect(text)
    except Exception:
        return 'unknown'