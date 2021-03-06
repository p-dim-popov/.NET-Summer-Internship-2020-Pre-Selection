﻿using System;
using System.Linq;

namespace Codenavirus
{
    public class Program
    {
        static void Main(string[] args)
        {
        }

        int[] Codenavirus(char[][] world, int[] firstInfected)
        {
            // Not using the new "Tuple type" because not sure of required C# version
            
            int notInfectedPeople = 0;
            // Projection of the world with people and their health
            // Item1 - person's health state, Item2 - infection day of the person
            var people = new Tuple<char, int>[world.Length][];
            for (int row = 0; row < world.Length; row++)
            {
                people[row] = new Tuple<char, int>[world[row].Length];
                for (int col = 0; col < world[row].Length; col++)
                {
                    if (world[row][col] == '#')
                    {
                        people[row][col] = Tuple.Create('H', 0);
                        notInfectedPeople++;
                        continue;
                    }
                    people[row][col] = Tuple.Create('.', 0);
                }
            }

            // infect the first person
            people[firstInfected[0]][firstInfected[1]] = Tuple.Create('I', 0);

            int daysPassed = 1;
            int infectedPeople = 1;
            int recoveredPeople = 0;
            //int notInfectedPeople = world.Length * world[0].Length - infectedPeople;
            bool isSomeoneInfectedToday = true;

            while (isSomeoneInfectedToday)
            {
                isSomeoneInfectedToday = false;

                for (int row = 0; row < people.Length; row++)
                {
                    for (int col = 0; col < people[row].Length; col++)
                    {
                        Tuple<char, int> person = people[row][col];

                        if (person.Item1 != 'I') continue;

                        if (daysPassed - person.Item2 >= 3)
                        {
                            people[row][col] = Tuple.Create('R', person.Item2);
                            recoveredPeople++;
                            infectedPeople--;
                            continue;
                        }

                        // if person is infected the same day 
                        if (daysPassed - person.Item2 == 0) continue;

                        var possibleTargets = new[]
                        {
                            new { IsFeasible = col + 1 < people.Length && people[row][col + 1].Item1 == 'H', Row = row, Col = col + 1 }, // on the right
                            new { IsFeasible = row - 1 >= 0 && people[row - 1][col].Item1 == 'H', Row = row - 1,Col = col }, // on top
                            new { IsFeasible = col - 1 >= 0 && people[row][col - 1].Item1 == 'H', Row = row, Col = col - 1 }, // on the left
                            new { IsFeasible = row + 1 < people[row].Length && people[row + 1][col].Item1 == 'H', Row = row + 1, Col = col } // underneath
                        };

                        foreach (var possibleTarget in possibleTargets)
                        {
                            if (possibleTarget.IsFeasible) // is feasible target => infect
                            {
                                people[possibleTarget.Row][possibleTarget.Col] = Tuple.Create('I', daysPassed);
                                infectedPeople++;
                                notInfectedPeople--;
                                isSomeoneInfectedToday = true;
                                break; // only one person infected per day
                            }
                        }
                    }
                }
                daysPassed++;
            }

            return new int[] { daysPassed, infectedPeople, recoveredPeople, notInfectedPeople };
        }
    }
}
