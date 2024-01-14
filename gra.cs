using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;

class Program
{
    static void Main()
    {
        int totalTimeInSeconds = 5 * 60;
        int playerScore = 0;

        Console.WriteLine("Witaj w Egipskiej Piramidzie!");
        DisplayAsciiArt();
        DisplayScoreAndTime(playerScore, totalTimeInSeconds);

        // Ładowanie danych gry z pliku JSON
        string jsonFilePath = "game.json";
        GameData gameData = LoadGameData(jsonFilePath);

        if (gameData == null)
        {
            Console.WriteLine("Błąd podczas ładowania danych gry. Gra nie może być kontynuowana.");
            return;
        }

        while (totalTimeInSeconds > 0)
        {
            Console.Clear();
            DisplayAsciiArt();
            DisplayScoreAndTime(playerScore, totalTimeInSeconds);

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("Wybierz opcję:");
            Console.WriteLine("E. Przeszukaj pokój");
            Console.WriteLine("P. Rozwiąż łamigłówkę");
            Console.WriteLine("I. Sprawdź dostępne przedmioty");
            Console.WriteLine("U. Użyj przedmiotu");

            if (playerScore >= 50)
            {
                Console.WriteLine("X. Wyjdź z piramidy");
            }
            else
            {
                Console.WriteLine($"X. Wyjdź z piramidy (Wymagane minimum 50 punktów, obecny wynik: {playerScore})");
            }
            Console.ForegroundColor = ConsoleColor.Gray;

            ConsoleKeyInfo keyInfo = Console.ReadKey();
            char userInput = char.ToUpper(keyInfo.KeyChar);

            switch (userInput)
            {
                case 'E':
                    ExploreRoom(gameData, ref playerScore);
                    break;

                case 'P':
                    Console.WriteLine("\nRozwiązujesz skomplikowaną łamigłówkę!");
                    playerScore += 20;
                    break;

                case 'I':
                    Console.WriteLine("\nPrzedmioty:");
                    Console.WriteLine(gameData.HasTorch ? "   Pochodnia" : "   (Brak pochodni)");
                    Console.WriteLine(gameData.HasMap ? "   Mapa" : "   (Brak mapy)");
                    break;

                case 'U':
                    UseItem(gameData, ref playerScore, ref totalTimeInSeconds);
                    break;

                case 'X':
                    if (playerScore >= 50)
                    {
                        Console.WriteLine("\nOpuszczanie piramidy...");
                        Thread.Sleep(2000);
                        Console.WriteLine("Gratulacje! Udało Ci się uciec z piramidy!");
                        Console.WriteLine($"Twój końcowy wynik: {playerScore}");
                        Environment.Exit(0);
                    }
                    else
                    {
                        Console.WriteLine($"\nNie możesz jeszcze opuścić piramidy. Potrzebujesz minimum 50 punktów, a twój obecny wynik to: {playerScore}");
                    }
                    break;

                default:
                    Console.WriteLine("\nNieprawidłowa opcja. Spróbuj ponownie.");
                    break;
            }
            totalTimeInSeconds -= 20;
            Thread.Sleep(1000);
        }

        Console.Clear();
        Console.WriteLine("Skończył się czas! Nie udało Ci się uciec z piramidy na czas.");
        Console.WriteLine($"Twój końcowy wynik: {playerScore}");
    }

    static void ExploreRoom(GameData gameData, ref int playerScore)
    {
        Console.WriteLine("\nOdkrywasz pokój...");

        // Symulacja znalezienia pokoju na podstawie danych gry
        Room currentRoom = GetRandomRoom(gameData);
        Console.WriteLine(currentRoom.Description);

        // Symulacja znalezienia przedmiotów na podstawie danych gry
        FindItems(currentRoom.Items, gameData, ref playerScore);
    }

    static void UseItem(GameData gameData, ref int playerScore, ref int totalTimeInSeconds)
    {
        Console.WriteLine("\nWybierz przedmiot do użycia:");

        if (gameData.HasTorch)
        {
            Console.WriteLine("P. Użyj pochodni");
        }

        if (gameData.HasMap)
        {
            Console.WriteLine("M. Użyj mapy");
        }

        ConsoleKeyInfo itemKeyInfo = Console.ReadKey();
        char itemChoice = char.ToUpper(itemKeyInfo.KeyChar);

        switch (itemChoice)
        {
            case 'P':
                if (gameData.HasTorch)
                {
                    Console.WriteLine("\nZapalasz pochodnię, oświetlając otoczenie.");
                    playerScore += 5;
                    totalTimeInSeconds += 30;
                    Console.WriteLine("Masz więcej czasu na znalezienie wyjścia, bo nie błądzisz już w ciemności.");
                }
                else
                {
                    Console.WriteLine("\nNie masz pochodni.");
                }
                break;

            case 'M':
                if (gameData.HasMap)
                {
                    Console.WriteLine("\nZdobywasz wiedzę o układzie piramidy.");
                    playerScore += 10;
                    Console.WriteLine("Łatwiej ci znaleźć wyjście.");
                }
                else
                {
                    Console.WriteLine("\nNie masz mapy.");
                }
                break;

            default:
                Console.WriteLine("\nNieprawidłowy wybór przedmiotu.");
                break;
        }
    }

    static Room GetRandomRoom(GameData gameData)
    {
        Random random = new Random();
        int roomIndex = random.Next(0, gameData.Rooms.Count);
        return gameData.Rooms[roomIndex];
    }

    static void FindItems(string[] itemIds, GameData gameData, ref int playerScore)
    {
        foreach (var itemId in itemIds)
        {
            if (gameData.Items.TryGetValue(itemId, out var item))
            {
                Console.WriteLine($"Znalazłeś {item.Name}: {item.Description}");
                playerScore += item.Score;

                if (item.Effect != null && item.Effect.Type == "time_bonus")
                {
                    totalTimeInSeconds += item.Effect.Value;
                    Console.WriteLine($"Zdobywasz dodatkowe {item.Effect.Value} sekundy!");
                }
            }
        }
    }

    static GameData LoadGameData(string jsonFilePath)
    {
        try
        {
            string jsonString = File.ReadAllText(jsonFilePath);
            return JsonSerializer.Deserialize<GameData>(jsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd ładowania danych gry: {ex.Message}");
            return null;
        }
    }

    static void DisplayScoreAndTime(int score, int time)
    {
        Console.WriteLine($"Twój wynik: {score}  |  Pozostały czas: {time / 60:D2}:{time % 60:D2}");
    }

    static void DisplayAsciiArt()
    {
        Console.WriteLine(@"
               .
              /=\\
             /===\ \
            /=====\' \
           /=======\'' \
          /=========\ ' '\
         /===========\''   \
        /=============\ ' '  \
       /===============\   ''  \
      /=================\' ' ' ' \
     /===================\' ' '  ' \
    /=====================\' ' '   ' \
   /=======================\  '   ' /
  /=========================\   ' /
 /===========================\'  /
/=============================\/
                    
");
    }
}

public class GameData
{
    public bool HasTorch { get; set; }
    public bool HasMap { get; set; }
    public List<Room> Rooms { get; set; }
    public Dictionary<string, Item> Items { get; set; }
}

public class Room
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string[] Items { get; set; }
}

public class Item
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Score { get; set; }
    public ItemEffect Effect { get; set; }
}

public class ItemEffect
{
    public string Type { get; set; }
    public int Value { get; set; }
}
