using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace InstallerHost
{
    public static class Texts
    {
        private static string licence = @"-- RETROBAT LICENSE --

RetroBat is a Windows softwares distribution dedicated to retrogaming and emulation.

Copyright (c) 2017-2019 Adrien Chalard ""Kayl""
Copyright (c) 2020-2025 RetroBat Team

RetroBat is free and open source project. It should not be used for commercial purposes. 
It is done by a team of enthusiasts in their free time mainly for fun.
All the code written by RetroBat Team, unless covered by a licence from an upstream project, is given under the LGPL v3 licence.
See https://www.gnu.org/licenses.

It is not allowed to sell RetroBat on a pre-installed machine or on any storage devices. 
RetroBat includes softwares which cannot be associated with any commercial activities.
Shipping RetroBat with additional proprietary and copyrighted content is illegal, strictly forbidden and strongly discouraged by the RetroBat Team.
Otherwise, you can start a new project off RetroBat sources if you follow the same conditions.

Finally, the license which concerns the entire RetroBat Project as a work, in particular the written or graphic content broadcast on its various media, is conditioned by the terms of the CC BY-NC-SA 4.0 license.
See https://creativecommons.org/licenses/by-nc-sa/4.0.";

    private static Dictionary<string, string> frenchStrings = new Dictionary<string, string>
        {
            { "Cancel", "Annuler" },
            { "Next", "Suivant" },
            { "Back", "Retour" },
            { "Browse", "Parcourir" },
            { "Install", "Installer" },
            { "Welcome", "Bienvenue dans l'assistant d'installation de RetroBat" },
            { "WelcomeText", "Cet assistant va vous guider dans l'installation de RetroBat {0} {1} sur votre ordinateur.\n\nIl est recommandé de fermer toutes les applications actives avant de continuer.\n\nCliquez sur Suivant pour continuer ou sur Annuler pour abandonner l'installation." },
            { "CancelSure", "Êtes-vous sûr de vouloir quitter l'installation ?" },
            { "CancelButtonTitle", "Annulation" },
            { "AgreeText", "J'accepte les conditions d'utilisation" },
            { "LicenseText", licence },
            { "LicenseIntro", "Accord de licence" },
            { "SelectFolder", "Répertoire d'installation:" },
            { "InstallTitle", "Dossier de destination" },
            { "InstallInfo", "L'assistant va installer RetroBat dans le dossier suivant.\nPour continuer, cliquez sur Suivant. Si vous souhaitez choisir un dossier différent, cliquez sur Parcourir." },
            { "InstallFolderHint", "Le programme requiert au moins 3,38 Go d'espace disque disponible." },
            { "FolderNotEmpty", "Le dossier sélectionné n'est pas vide. Veuillez choisir un dossier vide ou créez un nouveau dossier." },
            { "ValidFolder", "Veuillez sélectionner un dossier valide." },
            { "FailedFolder", "Erreur lors de la création du dossier: " },
            { "ExtractFail", "Erreur lors de l'extraction des fichiers: " },
            { "InstallFail", "Erreur lors de l'installation: " },
            { "LaunchFail", "Erreur lors du lancement de RetroBat: " },
            { "ExeNotFound", "Exécutable non trouvé: " },
            { "InstallComplete", "Installation terminée!" },
            { "RunRetroBat", "Lancer RetroBat.exe" },
            { "Finish", "Terminer" },
            { "Error", "Erreur" },
            { "StartupError", "Erreur au lancement de l'application, consultez le log." }
        };

        public static string GetString(string key, params object[] args)
        {
            var culture = CultureInfo.CurrentUICulture;

            string text;

            if (culture.Name.StartsWith("fr") && frenchStrings.ContainsKey(key))
                text = frenchStrings[key];
            else
                switch (key)
                {
                    case "Welcome":
                        text = "Welcome to the RetroBat installation program";
                        break;
                    case "StartupError":
                        text = "Startup error occurred. See log file for details.";
                        break;
                    case "RunRetroBat":
                        text = "Run RetroBat.exe";
                        break;
                    case "InstallComplete":
                        text = "Installation completed successfully!";
                        break;
                    case "LaunchFail":
                        text = "Failed to launch application: ";
                        break;
                    case "ExeNotFound":
                        text = "Executable not found: ";
                        break;
                    case "InstallFail":
                        text = "Installation failed: ";
                        break;
                    case "ExtractFail":
                        text = "Failed to extract installer data: ";
                        break;
                    case "FailedFolder":
                        text = "Failed to create installation folder: ";
                        break;
                    case "ValidFolder":
                        text = "Please select a valid installation folder.";
                        break;
                    case "LicenseIntro":
                        text = "Licence Agreement";
                        break;
                    case "AgreeText":
                        text = "I accept the terms of the license agreement";
                        break;
                    case "SelectFolder":
                        text = "Select the installation folder:";
                        break;
                    case "FolderNotEmpty":
                        text = "The selected folder is not empty. Please choose an empty folder or a new folder.";
                        break;
                    case "CancelSure":
                        text = "Are you sure that you want to cancel the installation ?";
                        break;
                    case "CancelButtonTitle":
                        text = "Cancel";
                        break;
                    case "WelcomeText":
                        text = "This wizard will guide you through the installation of RetroBat {0} {1} on your computer.\n\nIt is recommended to close all active applications before the next step.\n\nClick Next to continue or Cancel to exit the installer.";
                        break;
                    case "InstallTitle":
                        text = "Destination folder";
                        break;
                    case "InstallInfo":
                        text = "The installer program will install RetroBat in the folder below.\n\nTo continue, click Next. If you want to specify another folder, Click Browse.";
                        break;
                    case "InstallFolderHint":
                        text = "The prgram requires at least 3.38 GB of free disk space.";
                        break;
                    default:
                        text = key;
                        break;
                }
            return string.Format(text, args);
        }
    }
}
