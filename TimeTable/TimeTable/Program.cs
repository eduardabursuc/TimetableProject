using System.Text.Json;
using TimeTable.Models;
using TimeTable.Validators;

class Program
{
    static void Main()
    {
        string configPath = Path.Combine(Directory.GetCurrentDirectory(), "../../../config.json");

        if (!File.Exists(configPath))
        {
            Console.WriteLine("Configuration file 'config.json' not found.");
            return;
        }

        // Load the instance from the configuration file
        Instance instance = new Instance(configPath);
        Console.WriteLine("Instance loaded successfully!");

        // Initialize the validator
        SoftConstraintsValidator validator = new SoftConstraintsValidator(instance);

        while (true)
        {
            // Read JSON input for the constraint from the console
            Console.WriteLine("Enter the constraint in JSON format:");
            string jsonInput = Console.ReadLine();

            // Check for empty input
            if (string.IsNullOrWhiteSpace(jsonInput))
            {
                Console.WriteLine("Invalid input. Please provide a valid JSON string.");
                return;
            }

            try
            {
                // Deserialize the JSON string into a Constraint object
                Constraint constraint = JsonSerializer.Deserialize<Constraint>(jsonInput);

                // Validate the constraint and print the result
                if (constraint == null)
                {
                    Console.WriteLine("Deserialization failed. The input JSON may be incorrect.");
                }
                else
                {
                    bool validationResult = validator.Validate(constraint);
                    Console.WriteLine($"Validation Result: {validationResult}");
                }
            }
            catch (JsonException ex)
            {
                // Handle JSON deserialization exceptions
                Console.WriteLine($"Error deserializing JSON input: {ex.Message}");
            }
        }
    }

}
