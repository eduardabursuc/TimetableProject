### RUN THIS SCRIPT TO SETUP THE PROJECT ENVIRONMENT ###

import subprocess
import sys

required_packages = [
    "langdetect",
    "nltk",
    "spacy",
    "openai",
    "keybert",
    "transformers"
]

def install_packages():
    """
    Install the required Python packages using pip.
    """
    print("Installing required packages...")
    for package in required_packages:
        try:
            subprocess.check_call([sys.executable, "-m", "pip", "install", package])
        except subprocess.CalledProcessError as e:
            print(f"[ERROR] Failed to install {package}: {e}")
            sys.exit(1)

def download_nltk_resources():
    """
    Download necessary NLTK resources after installation.
    """
    try:
        import nltk
        print("Downloading NLTK resources...")
        nltk_resources = ['punkt', 'wordnet', 'stopwords']
        for resource in nltk_resources:
            nltk.download(resource)
    except ImportError:
        print("[ERROR] NLTK is not installed. Ensure packages are installed first.")
        sys.exit(1)

def download_spacy_model():
    """
    Download the spaCy model for Romanian.
    """
    print("Downloading spaCy model 'ro_core_news_sm'...")
    try:
        subprocess.check_call([sys.executable, "-m", "spacy", "download", "ro_core_news_sm"])
    except subprocess.CalledProcessError as e:
        print(f"[ERROR] Failed to download spaCy model: {e}")
        sys.exit(1)

def main():
    install_packages()
    download_spacy_model()
    download_nltk_resources()
    print("Setup completed successfully!")

if __name__ == "__main__":
    main()