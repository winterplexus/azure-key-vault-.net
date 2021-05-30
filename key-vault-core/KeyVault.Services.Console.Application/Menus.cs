//
//  Menus.cs
//
//  Copyright (c) Wiregrass Code Technology 2020-2021
//
using System;
using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace KeyVault.Services.Console.Application
{
    internal class Menus
    {
        private readonly IConfigurationSection configuration;

        public Menus(IConfigurationSection configurationSection)
        {
            configuration = configurationSection;
        }

        internal void MainMenu()
        {
            try
            {
                while (true)
                {
                    WriteMainMenu();

                    var command = ReadMenuOption();
                    switch (command)
                    {
                        case "K": ProcessManageKeysMenu();
                                  break;
                        case "S": ProcessManageSecretsMenu();
                                  break;
                        case "X": return;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteUnexpectedException(ex);
                ReadContinue();
            }
        }

        private void ProcessManageKeysMenu()
        {
            var manageKeys = new ManageKeys(configuration);

            while (true)
            {
                WriteManageKeysSubmenu();

                var command = ReadMenuOption();
                switch (command)
                {
                    case "1": manageKeys.List();
                              break;
                    case "2": manageKeys.Create();
                              break;
                    case "3": manageKeys.Retrieval();
                              break;
                    case "4": manageKeys.Update();
                              break;
                    case "5": manageKeys.Delete();
                              break;
                    case "M": return;
                }
            }
        }

        private void ProcessManageSecretsMenu()
        {
            var manageSecrets = new ManageSecrets(configuration);

            while (true)
            {
                WriteManageSecretsSubmenu();

                var command = ReadMenuOption();
                switch (command)
                {
                    case "1": manageSecrets.List();
                              break;
                    case "2": manageSecrets.Create();
                              break;
                    case "3": manageSecrets.Retrieval();
                              break;
                    case "4": manageSecrets.Update();
                              break;
                    case "5": manageSecrets.Delete();
                              break;
                    case "M": return;
                }
            }
        }

        private static void WriteMainMenu()
        {
            System.Console.Clear();
            System.Console.WriteLine("KEY VAULT SERVICES: MAIN MENU");
            System.Console.WriteLine("");
            System.Console.WriteLine("COMMAND DESCRIPTION");
            System.Console.WriteLine("================================================================================");
            System.Console.WriteLine("[ K ]   MANAGE KEYS");
            System.Console.WriteLine("[ S ]   MANAGE SECRETS");
            System.Console.WriteLine("[ X ]   EXIT");
            System.Console.WriteLine("================================================================================");
        }

        private static void WriteManageKeysSubmenu()
        {
            System.Console.Clear();
            System.Console.WriteLine("KEY VAULT SERVICES: MANAGE KEYS MENU");
            System.Console.WriteLine("");
            System.Console.WriteLine("COMMAND DESCRIPTION");
            System.Console.WriteLine("================================================================================");
            System.Console.WriteLine("[ 1 ]   LIST KEYS");
            System.Console.WriteLine("[ 2 ]   CREATE KEY");
            System.Console.WriteLine("[ 3 ]   RETRIEVE KEY");
            System.Console.WriteLine("[ 4 ]   UPDATE KEY");
            System.Console.WriteLine("[ 5 ]   DELETE/PURGE KEY");
            System.Console.WriteLine("[ M ]   MAIN MENU");
            System.Console.WriteLine("================================================================================");
        }

        private static void WriteManageSecretsSubmenu()
        {
            System.Console.Clear();
            System.Console.WriteLine("KEY VAULT SERVICES: MANAGE SECRETS MENU");
            System.Console.WriteLine("");
            System.Console.WriteLine("COMMAND DESCRIPTION");
            System.Console.WriteLine("================================================================================");
            System.Console.WriteLine("[ 1 ]   LIST SECRETS");
            System.Console.WriteLine("[ 2 ]   CREATE SECERTS");
            System.Console.WriteLine("[ 3 ]   RETRIEVE SECRETS");
            System.Console.WriteLine("[ 4 ]   UPDATE SECRETS");
            System.Console.WriteLine("[ 5 ]   DELETE/PURGE SECRETS");
            System.Console.WriteLine("[ M ]   MAIN MENU");
            System.Console.WriteLine("================================================================================");
        }

        private static void WriteUnexpectedException(Exception ex)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("unexcepted exception-> {0}", ex.Message);
            System.Console.WriteLine("inner exception-> {0}", ex.InnerException?.Message);
            System.Console.WriteLine("stack trace-> {0}{1}", Environment.NewLine, ex.StackTrace);
            System.Console.ResetColor();
        }

        private static string ReadMenuOption()

        {
            System.Console.WriteLine("");
            System.Console.Write("ENTER COMMAND AND PRESS ENTER: ");

            var command = System.Console.ReadLine();
            return !string.IsNullOrEmpty(command) ? command.ToUpper(CultureInfo.CurrentCulture) : "X";
        }

        private static void ReadContinue()
        {
            System.Console.WriteLine("");
            System.Console.Write("PRESS ENTER TO CONTINUE ->");
            System.Console.ReadKey();
        }
    }
}