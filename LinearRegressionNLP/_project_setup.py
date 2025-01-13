import subprocess
import sys

def install_package(package):
    """Install a package using pip."""
    try:
        subprocess.check_call([sys.executable, "-m", "pip", "install", package])
        print(f"Successfully installed {package}")
    except subprocess.CalledProcessError:
        print(f"Failed to install {package}. Please install it manually.")
        sys.exit(1)

def download_nltk_data():
    """Download required NLTK data."""
    import nltk
    try:
        nltk.download('punkt')
        nltk.download('stopwords')
        nltk.download('wordnet')
        print("Successfully downloaded NLTK data")
    except Exception as e:
        print(f"Failed to download NLTK data: {e}")
        sys.exit(1)

def main():
    print("Setting up the project...")

    # List of required packages
    required_packages = [
        "nltk",
        "autocorrect",
        "symspellpy",
        "gensim",
        "scikit-learn",
        "pandas",
        "flask",
        "psycopg2",
        "flask_cors"
    ]

    # Install required packages
    for package in required_packages:
        install_package(package)

    # Download NLTK data
    download_nltk_data()

    print("\nSetup completed successfully!")

user_email = "admin@gmail.com"

if __name__ == "__main__":
    main()