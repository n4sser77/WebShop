using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebShop
{
    internal class InputHelper
    {

        public static string ReadKeysPassword()
        {
            StringBuilder password = new StringBuilder();

            while (true)
            {
                var keyInfo = Console.ReadKey(true); // Read key with no display

                switch (keyInfo.Key)
                {
                    case ConsoleKey.Enter:
                        // When Enter is pressed, return the password
                        Console.WriteLine();
                        return password.ToString();

                    case ConsoleKey.Backspace:
                        // Handle backspace for deleting characters
                        if (password.Length > 0)
                        {
                            password.Remove(password.Length - 1, 1);
                            Console.Write("\b \b"); // Move the cursor back, print space, and move back again
                        }
                        break;

                    default:
                        // Append the key character to the password if it's not a special key
                        if (!char.IsControl(keyInfo.KeyChar))
                        {
                            password.Append(keyInfo.KeyChar);
                            Console.Write('*'); // Display asterisks for the password input
                        }
                        break;
                }
            }
        }
        public static async IAsyncEnumerable<string> ReadKeysSearchAsync()
        {
            var input = new StringBuilder();

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var keyInfo = Console.ReadKey(true);

                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.Enter:
                            yield break;

                        case ConsoleKey.Backspace:
                            if (input.Length > 0)
                            {
                                input.Remove(input.Length - 1, 1);
                                Console.Write("\b \b");
                            }
                            break;

                        default:
                            if (!char.IsControl(keyInfo.KeyChar))
                            {
                                input.Append(keyInfo.KeyChar);
                                Console.Write(keyInfo.KeyChar);
                            }
                            break;
                    }

                    yield return input.ToString();
                }

                await Task.Delay(50);
            }
        }




        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
