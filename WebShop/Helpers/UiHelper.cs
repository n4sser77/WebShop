using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop
{
    public static class UiHelper
    {
        public static void ChangeConsoleColors(ConsoleColor foreground, ConsoleColor background)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
        }

        public static async Task LoadingUI(CancellationToken cancellationToken, bool loginTrial)
        {
            var animationLength = 0;

            Console.Write("Loading");
            while (!cancellationToken.IsCancellationRequested && loginTrial)
            {
                Console.Write(".");
                animationLength++;
                await Task.Delay(200, cancellationToken); // Use cancellation token for Task.Delay

                if (animationLength % 3 == 0) // Cycle every three dots
                {
                    Console.Write("\b\b\b   \b\b\b"); // Erase the dots for a smoother animation
                    animationLength = 0; // Reset counter
                }
            }

            // Clear the animation line completely when stopping
            Console.Write("\r" + new string(' ', "Loading...".Length) + "\r");
        }
    }
}
