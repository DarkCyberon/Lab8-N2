using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Lab8_N2 {
    class OutputOfCardAccount  {
        static void Main()  {
            string[] lines = File.ReadAllLines("text.txt");
            List<Operation> operations = DoTheOperations(lines);

            Console.Write("Enter the date and time of the last operation you want to consider (Input format: yyyy-MM-dd hh:mm): ");
            string input = Console.ReadLine();
            int theCurrentAccount = (DateTime.TryParseExact(input, "yyyy-MM-dd hh:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime time)) ? 
                ReturnTheCardAccount(operations, time) : Operation.LastSum;
            Console.WriteLine("On this point of time the card account was: {0}", theCurrentAccount);
            Console.ReadKey();
        }

        static int ReturnTheCardAccount(List<Operation> operations, DateTime time) {
            int theCurrentAccount = Operation.StartSum;

            foreach (var operation in operations)  {
                if (time < operation.Time) break;
                switch (operation.Action) {
                    case "in":
                        theCurrentAccount += operation.Sum;
                        break;
                    case "out":
                        theCurrentAccount -= operation.Sum;
                        if (theCurrentAccount < 0) throw new Exception("The operation was impossible!");
                        break;
                    case "revert":
                        theCurrentAccount = (operations[operation.Id - 1].Action == "in") ?
                            theCurrentAccount - operations[operation.Id - 1].Sum :
                            theCurrentAccount + operations[operation.Id - 1].Sum;
                        break;
                }
            }

            return theCurrentAccount;
        }

        static List<Operation> DoTheOperations(string[] lines)  {
            Operation.StartSum = int.Parse(lines[0]);
            var operations = new List<Operation>();

            for (int i = 1; i < lines.Length; i++)  {
                string[] elements = lines[i].Split("|");
                var operation = new Operation();
                if (elements.Length == 3)  {
                    operation.Time = DateTime.Parse(elements[0].Trim());
                    operation.Sum = int.Parse(elements[1].Trim());
                    operation.Action = elements[2].Trim();
                }
                else {
                    operation.Time = DateTime.Parse(elements[0].Trim());
                    operation.Action = elements[1].Trim();
                }

                if (operation.Time > Operation.MaxTime) Operation.MaxTime = operation.Time;
                operations.Add(operation);
            }

            return SortTheOperations(operations);
        }

        static List<Operation> SortTheOperations(List<Operation> operations) {
            operations.Sort(delegate (Operation operation1, Operation operation2)
            { return operation1.Time.CompareTo(operation2.Time); });
            operations = DeleteExtraRevertions(operations);

            for (int i = 0; i < operations.Count; i++) operations[i].Id = i;
            Operation.LastSum = ReturnTheCardAccount(operations, Operation.MaxTime);

            return operations;
        }

        static List<Operation> DeleteExtraRevertions(List<Operation> operations) {
            var listWithoutExtra = new List<Operation>();

            for (int i = 0; i < operations.Count; i++)  {
                if (operations[i].Action == "revert") {
                    int revertionsCount = 0;
                    revertionsCount++;

                    while (operations.Count > i + 1 && operations[i + 1].Action == "revert") {
                        i++;
                        revertionsCount++;
                    }

                    if (revertionsCount % 2 == 0) continue;
                }

                listWithoutExtra.Add(operations[i]);
            }

            return listWithoutExtra;
        }
    }
}
