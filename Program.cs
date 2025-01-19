using ChalkDotNET;

namespace GamedayTracker
{
    internal class Program
    {
        static async Task Main(string[] args)
        { 
            Console.WriteLine("\r\n");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(@"


         ▄████  ▄▄▄       ███▄ ▄███▓▓█████ ▓█████▄  ▄▄▄     ▓██   ██▓   ▄▄▄█████▓ ██▀███   ▄▄▄       ▄████▄   ██ ▄█▀▓█████  ██▀███  
        ██▒ ▀█▒▒████▄    ▓██▒▀█▀ ██▒▓█   ▀ ▒██▀ ██▌▒████▄    ▒██  ██▒   ▓  ██▒ ▓▒▓██ ▒ ██▒▒████▄    ▒██▀ ▀█   ██▄█▒ ▓█   ▀ ▓██ ▒ ██▒
       ▒██░▄▄▄░▒██  ▀█▄  ▓██    ▓██░▒███   ░██   █▌▒██  ▀█▄   ▒██ ██░   ▒ ▓██░ ▒░▓██ ░▄█ ▒▒██  ▀█▄  ▒▓█    ▄ ▓███▄░ ▒███   ▓██ ░▄█ ▒
       ░▓█  ██▓░██▄▄▄▄██ ▒██    ▒██ ▒▓█  ▄ ░▓█▄   ▌░██▄▄▄▄██  ░ ▐██▓░   ░ ▓██▓ ░ ▒██▀▀█▄  ░██▄▄▄▄██ ▒▓▓▄ ▄██▒▓██ █▄ ▒▓█  ▄ ▒██▀▀█▄  
       ░▒▓███▀▒ ▓█   ▓██▒▒██▒   ░██▒░▒████▒░▒████▓  ▓█   ▓██▒ ░ ██▒▓░     ▒██▒ ░ ░██▓ ▒██▒ ▓█   ▓██▒▒ ▓███▀ ░▒██▒ █▄░▒████▒░██▓ ▒██▒
        ░▒   ▒  ▒▒   ▓▒█░░ ▒░   ░  ░░░ ▒░ ░ ▒▒▓  ▒  ▒▒   ▓▒█░  ██▒▒▒      ▒ ░░   ░ ▒▓ ░▒▓░ ▒▒   ▓▒█░░ ░▒ ▒  ░▒ ▒▒ ▓▒░░ ▒░ ░░ ▒▓ ░▒▓░
         ░   ░   ▒   ▒▒ ░░  ░      ░ ░ ░  ░ ░ ▒  ▒   ▒   ▒▒ ░▓██ ░▒░        ░      ░▒ ░ ▒░  ▒   ▒▒ ░  ░  ▒   ░ ░▒ ▒░ ░ ░  ░  ░▒ ░ ▒░
       ░ ░   ░   ░   ▒   ░      ░      ░    ░ ░  ░   ░   ▒   ▒ ▒ ░░       ░        ░░   ░   ░   ▒   ░        ░ ░░ ░    ░     ░░   ░ 
             ░       ░  ░       ░      ░  ░   ░          ░  ░░ ░                    ░           ░  ░░ ░      ░  ░      ░  ░   ░     
                                                 ░                ░ ░                                    ░                               
            

");
            Console.ResetColor();
            Console.WriteLine($"{Chalk.Yellow($"[{DateTimeOffset.UtcNow}]")} {Chalk.DarkGray("[200 /Program]")} {Chalk.Blue("\t [INFO ]")} shaking hands with discord...");
            await Task.Delay(1000);
            var bot = new Bot();
            await bot.RunAsync();

        }
    }
}
