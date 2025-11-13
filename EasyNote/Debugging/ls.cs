using System.IO;
namespace EasyNote.Debugging;

public class ls
{
    public static void ShowFoldersInCurrentPath()
    {
        // Get the current directory (where the program is running)
        string currentPath = Directory.GetCurrentDirectory();
        
        Console.WriteLine("Current Path: " + currentPath);
        Console.WriteLine("-----------------------------------");
        
        try
        {
            // Get all directories (folders) in the current path
            string[] folders = Directory.GetDirectories(currentPath);
            
            // Check if there are any folders
            if (folders.Length == 0)
            {
                Console.WriteLine("No folders found in this directory.");
            }
            else
            {
                Console.WriteLine($"Found {folders.Length} folder(s):\n");
                
                // Loop through each folder and display it
                foreach (string folder in folders)
                {
                    // Get just the folder name (not the full path)
                    string folderName = Path.GetFileName(folder);
                    
                    // Display the folder name
                    Console.WriteLine($"📁 {folderName}");
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("Error: No permission to access this directory.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}