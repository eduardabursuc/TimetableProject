using System.Text.Json;
using TimeTable.Models;
using TimeTable.Validators;

class Program
{
    static void Main()
    {
        string configPath = Path.Combine(Directory.GetCurrentDirectory(), "../../../config.json");
        string constraintsPath = Path.Combine(Directory.GetCurrentDirectory(), "../../../constraints.json"); 
        
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
        
        // Uncomment this section if you want to read constraints from the console
        /*
        while (true)
        {
            // Read JSON input for the constraint from the console
            Console.WriteLine("Enter the constraint in JSON format:");
            string jsonInput = Console.ReadLine();
            
            if (jsonInput == "exit")
            {
                instance.UploadToJson(configPath);
                break;
            }

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
                    if (constraint != null)
                    {
                        Tuple<bool, string> validationResult = validator.Validate(constraint);
                        Console.WriteLine($"Validation Result: {validationResult.Item2}");
                        if (validationResult.Item1)
                        {
                            instance.Constraints.Add(constraint);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Validation skipped: constraint is not valid for validation.");
                    }
                    Console.WriteLine();
                }
            }
            catch (JsonException ex)
            {
                // Handle JSON deserialization exceptions
                Console.WriteLine($"Error deserializing JSON input: {ex.Message}");
            }
        }
        */
        // Uncomment this section if you want to read constraints from a file
        
        // Check if the constraints file exists
        if (!File.Exists(constraintsPath))
        {
            Console.WriteLine("Constraints file 'constraints.json' not found.");
            return;
        }

        // Read and deserialize constraints from the JSON file
        try
        {
            string jsonInput = File.ReadAllText(constraintsPath);
            List<Constraint> constraints = JsonSerializer.Deserialize<List<Constraint>>(jsonInput);

            // Validate each constraint and print the results
            if (constraints != null)
            {
                foreach (var constraint in constraints)
                {
                    if (constraint != null)
                    {
                        Tuple<bool,string> validationResult = validator.Validate(constraint);
                        Console.WriteLine($"Constraint: {JsonSerializer.Serialize(constraint)}");
                        Console.WriteLine($"Validation Result: {validationResult.Item2}");
                        if (validationResult.Item1)
                        {
                            instance.Constraints.Add(constraint);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Constraint: {JsonSerializer.Serialize(constraint)}");
                        Console.WriteLine("Validation skipped: constraint is not valid for validation.");
                    }
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("Deserialization failed. The input JSON may be incorrect.");
            }
        }
        catch (JsonException ex)
        {
            // Handle JSON deserialization exceptions
            Console.WriteLine($"Error deserializing constraints JSON: {ex.Message}");
        }
        instance.UploadToJson(configPath);
        
    }
}
