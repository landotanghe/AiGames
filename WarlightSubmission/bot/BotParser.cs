using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using main;
using move;

namespace bot
{

    public class BotParser
    {

        readonly Bot bot;
        BotState CurrentState;

        public BotParser(Bot bot)
        {
            this.bot = bot;
            CurrentState = new BotState();
        }

        public void Run()
        {
            while (true)
            {
                var line = Console.ReadLine();
                if (line == null)
                    break;
                line = line.Trim();

                if (line.Length == 0)
                    continue;

                String[] parts = line.Split(' ');
                if (parts[0] == "pick_starting_regions")
                {
                    // Pick which regions you want to start with
                    CurrentState.SetPickableStartingRegions(parts);
                    var preferredStartingRegions = bot.GetPreferredStartingRegions(CurrentState, long.Parse(parts[1]));
                    var output = new StringBuilder();
                    foreach (var region in preferredStartingRegions)
                        output.Append(region.Id + " ");

                    Console.WriteLine(output);
                }
                else if (parts.Length == 3 && parts[0] == "go")
                {
                    // We need to do a move
                    var output = new StringBuilder();
                    if (parts[1] == "place_armies")
                    {
                        // Place armies
                        List<PlaceArmiesMove> placeArmiesMoves = bot.GetPlaceArmiesMoves(CurrentState, long.Parse(parts[2]));
                        foreach (var move in placeArmiesMoves)
                            output.Append(move.String + ",");
                    }
                    else if (parts[1] == "attack/transfer")
                    {
                        // attack/transfer
                        var attackTransferMoves = bot.GetAttackTransferMoves(CurrentState, long.Parse(parts[2]));
                        foreach (var move in attackTransferMoves)
                            output.Append(move.String + ",");
                    }
                    if (output.Length > 0)
                        Console.WriteLine(output);
                    else
                        Console.WriteLine("No moves");
                }
                else if (parts.Length == 3 && parts[0] == "settings")
                {
                    // Update settings
                    CurrentState.UpdateSettings(parts[1], parts[2]);
                }
                else if (parts[0] == "setup_map")
                {
                    // Initial full map is given
                    CurrentState.SetupMap(parts);
                }
                else if (parts[0] == "update_map")
                {
                    // All visible regions are given
                    CurrentState.UpdateMap(parts);
                }
                else if (parts[0] == "opponent_moves")
                {
                    // All visible opponent moves are given
                    CurrentState.ReadOpponentMoves(parts);
                }
                else
                {
                    Console.Error.WriteLine("Unable to parse line \"" + line + "\"");
                }
            }
        }

    }

}