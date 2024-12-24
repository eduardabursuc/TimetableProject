def read_text(input_path=None):
    """
    Reads a text from a file or command-line input.

    Args:
        input_path (str): Path to the input file. If None, read from stdin.

    Returns:
        str: The text read from the file or user input.
    """
    if input_path:
        try:
            with open(input_path, 'r', encoding='utf-8') as file:
                return file.read()
        except FileNotFoundError:
            raise FileNotFoundError(f"File not found: {input_path}")
        except Exception as e:
            raise IOError(f"Error reading file: {e}")
    else:
        print("Please input your text (Press Enter to submit, Ctrl+D to finish):")
        return "".join(iter(input, ""))