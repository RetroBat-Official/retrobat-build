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

        public static string GetString(string key, params object[] args)
        {
            var culture = CultureInfo.CurrentUICulture;

            string text;

            if (culture.Name.ToLowerInvariant().StartsWith("fr") && frenchStrings.ContainsKey(key))
                text = frenchStrings[key];
            else if (culture.Name.ToLowerInvariant().StartsWith("de") && germanStrings.ContainsKey(key))
                text = germanStrings[key];
            else if (culture.Name.ToLowerInvariant().StartsWith("es") && spanishStrings.ContainsKey(key))
                text = spanishStrings[key];
            else if (culture.Name.ToLowerInvariant().StartsWith("it") && italianStrings.ContainsKey(key))
                text = italianStrings[key];
            else if (culture.Name.ToLowerInvariant().StartsWith("bg") && bulgarianStrings.ContainsKey(key))
                text = bulgarianStrings[key];
            else if (culture.Name.ToLowerInvariant().StartsWith("pl") && polishStrings.ContainsKey(key))
                text = polishStrings[key];
            else
                switch (key)
                {
                    case "WaitingSelect":
                        text = "Waiting for selection...";
                        break;
                    case "WindowsTitle":
                        text = "RetroBat Installer ";
                        break;
                    case "Welcome":
                        text = "Welcome to the RetroBat installation program";
                        break;
                    case "InstallDX":
                        text = "Installing DirectX...";
                        break;
                    case "InstallComplete...":
                        text = "Installation complete...";
                        break;
                    case "DownloadAndInstall":
                        text = "Downloading and installing prerequisites...\r\nPlease wait...";
                        break;
                    case "dx9text":
                        text = "DirectX 9 (Legacy)";
                        break;
                    case "dokanyText":
                        text = "Dokany (used to mount XBOX images with CXBX)";
                        break;
                    case "vcText":
                        text = "Microsoft Visual C++ Redistributables (2005–2022, x86 + x64)";
                        break;
                    case "PrerequisiteIntro":
                        text = "Select components to install before continuing.";
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
                    case "LicenseText":
                        text = licence;
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
                        text = "The program requires at least 3.38 GB of free disk space.\n\nDo not use folders with spaces or special characters.";
                        break;
                    default:
                        text = key;
                        break;
                }
            return string.Format(text, args);
        }

        #region todo
        
        #endregion

        #region done
        private static Dictionary<string, string> bulgarianStrings = new Dictionary<string, string> //OK
        {
            { "Cancel", "Отказ" },
            { "WindowsTitle", "Инсталатор RetroBat " },
            { "Next", "Напред" },
            { "Back", "Назад" },
            { "Browse", "Преглед" },
            { "Install", "Инсталирай" },
            { "Welcome", "Добре дошли в програмата за инсталиране на RetroBat" },
            { "WelcomeText", "Този помощник ще ви преведе през инсталацията на RetroBat {0} {1} на вашия компютър.\n\nПрепоръчва се да затворите всички активни приложения преди следващата стъпка.\n\nЩракнете върху „Напред“, за да продължите, или върху „Отказ“, за да излезете от инсталатора." },
            { "CancelSure", "Сигурни ли сте, че искате да отмените инсталацията?" },
            { "CancelButtonTitle", "Отказ" },
            { "AgreeText", "Приемам условията на лицензионното споразумение" },
            { "LicenseText", licence },
            { "LicenseIntro", "Лицензионно споразумение" },
            { "SelectFolder", "Изберете папката за инсталиране:" },
            { "InstallTitle", "Целева папка" },
            { "InstallInfo", "Програмата за инсталиране ще инсталира RetroBat в папката по-долу.\n\nЗа да продължите, щракнете върху „Напред“. Ако искате да изберете друга папка, щракнете върху „Преглед“." },
            { "InstallFolderHint", "Програмата изисква поне 3.38 GB свободно дисково пространство.\n\nНе използвай папки с специални символи." },
            { "FolderNotEmpty", "Избраната папка не е празна. Моля, изберете празна папка или създайте нова." },
            { "ValidFolder", "Моля, изберете валидна папка за инсталиране." },
            { "FailedFolder", "Неуспешно създаване на инсталационна папка: " },
            { "ExtractFail", "Неуспешно извличане на данните на инсталатора: " },
            { "InstallFail", "Инсталацията е неуспешна: " },
            { "LaunchFail", "Неуспешно стартиране на приложението: " },
            { "ExeNotFound", "Изпълнимият файл не е намерен: " },
            { "InstallComplete", "Инсталацията е завършена..." },
            { "RunRetroBat", "Стартирай RetroBat.exe" },
            { "Finish", "Готово" },
            { "Error", "Грешка" },
            { "StartupError", "Възникна грешка при стартиране. Вижте файла с логове за повече информация." },
            { "PrerequisiteIntro", "Изберете компонентите за инсталиране, преди да продължите." },
            { "vcText", "Microsoft Visual C++ Redistributables (2005–2022, x86 + x64)" },
            { "dx9text", "DirectX 9 (Наследена версия)" },
            { "dokanyText", "Dokany (използва се за монтиране на XBOX образи с CXBX)" },
            { "DownloadAndInstall", "Изтегляне и инсталиране на предварителни компоненти...\r\nМоля, изчакайте..." },
            { "Downloading", "Изтегляне" },
            { "Extracting", "Извличане" },
            { "InstallDX", "Инсталиране на DirectX..." },
            { "Installing", "Инсталиране" },
            { "WaitingSelect", "Изчакване на избор..." }
        };

        private static Dictionary<string, string> frenchStrings = new Dictionary<string, string> // OK
        {
            { "Cancel", "Annuler" },
            { "WindowsTitle", "Installation de RetroBat " },
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
            { "InstallFolderHint", "Le programme requiert au moins 3,38 Go d'espace disque disponible.\n\nN'utilisez pas de dossier avec des espaces ou des caractères spéciaux !" },
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
            { "StartupError", "Erreur au lancement de l'application, consultez le log." },
            { "PrerequisiteIntro", "Sélectionnez les prérequis à installer." },
            { "vcText", "Microsoft Visual C++ Redistributables (2005–2022, x86 + x64)" },
            { "dx9text", "DirectX 9 (Legacy)" },
            { "dokanyText", "Dokany (permet de monter les images Xbox avec CXBX)" },
            { "DownloadAndInstall", "Téléchargement et installation des prérequis...\r\nVeuillez patienter..." },
            { "Downloading", "Téléchargement de" },
            { "Extracting", "Extraction de" },
            { "InstallDX", "Installation de DirectX..." },
            { "Installing", "Installation de" },
            { "WaitingSelect", "En attente des choix..." }
        };

        private static Dictionary<string, string> germanStrings = new Dictionary<string, string>
        {
            { "Cancel", "Abbrechen" },
            { "WindowsTitle", "RetroBat-Installer " },
            { "Next", "Weiter" },
            { "Back", "Zurück" },
            { "Browse", "Durchsuchen" },
            { "Install", "Installieren" },
            { "Welcome", "Willkommen beim RetroBat-Installationsprogramm" },
            { "WelcomeText", "Dieser Assistent führt dich durch die Installation von RetroBat {0} {1}.\n\nSchließe alle laufenden Programme, bevor du fortfährst.\n\nKlicke auf Weiter, um fortzufahren, oder auf Abbrechen, um den Installer zu beenden." },
            { "CancelSure", "Möchtest du die Installation wirklich abbrechen?" },
            { "CancelButtonTitle", "Abbrechen" },
            { "AgreeText", "Ich akzeptiere die Lizenzbedingungen" },
            { "LicenseText", licence },
            { "LicenseIntro", "Lizenzvereinbarung" },
            { "SelectFolder", "Installationsordner auswählen:" },
            { "InstallTitle", "Zielordner" },
            { "InstallInfo", "RetroBat wird im unten angegebenen Ordner installiert.\n\nZum Fortfahren auf Weiter klicken, für einen anderen Ordner auf Durchsuchen." },
            { "InstallFolderHint", "Das Programm benötigt mindestens 3,38 GB freien Speicherplatz.\n\nVermeiden Sie Ordner mit Leerzeichen oder Sonderzeichen." },
            { "FolderNotEmpty", "Der gewählte Ordner ist nicht leer. Bitte wähle einen leeren oder neuen Ordner." },
            { "ValidFolder", "Bitte wähle einen gültigen Installationsordner." },
            { "FailedFolder", "Erstellen des Installationsordners fehlgeschlagen: " },
            { "ExtractFail", "Entpacken der Installationsdaten fehlgeschlagen: " },
            { "InstallFail", "Installation fehlgeschlagen: " },
            { "LaunchFail", "Start der Anwendung fehlgeschlagen: " },
            { "ExeNotFound", "Ausführbare Datei nicht gefunden: " },
            { "InstallComplete", "Installation abgeschlossen." },
            { "RunRetroBat", "RetroBat.exe starten" },
            { "Finish", "Fertigstellen" },
            { "Error", "Fehler" },
            { "StartupError", "Startfehler aufgetreten. Siehe Logdatei für Details." },
            { "PrerequisiteIntro", "Wähle die zu installierenden Komponenten aus, bevor du fortfährst." },
            { "vcText", "Microsoft Visual C++ Redistributables (2005–2022, x86 + x64)" },
            { "dx9text", "DirectX 9 (Altversion)" },
            { "dokanyText", "Dokany (zum Einbinden von XBOX-Images mit CXBX)" },
            { "DownloadAndInstall", "Komponenten werden heruntergeladen und installiert…\r\nBitte warten…" },
            { "Downloading", "Wird heruntergeladen" },
            { "Extracting", "Wird entpackt" },
            { "InstallDX", "DirectX wird installiert…" },
            { "Installing", "Wird installiert" },
            { "WaitingSelect", "Warte auf Auswahl…" }
        };

        private static Dictionary<string, string> italianStrings = new Dictionary<string, string>
        {
            { "Cancel", "Annulla" },
            { "WindowsTitle", "Installatore RetroBat " },
            { "Next", "Avanti" },
            { "Back", "Indietro" },
            { "Browse", "Sfoglia" },
            { "Install", "Installa" },
            { "Welcome", "Benvenuto nel programma di installazione di RetroBat" },
            { "WelcomeText", "Questa procedura guidata ti accompagnerà nell'installazione di RetroBat {0} {1} sul tuo computer.\n\nSi consiglia di chiudere tutte le applicazioni attive prima del passaggio successivo.\n\nFai clic su Avanti per continuare o su Annulla per uscire dal programma di installazione." },
            { "CancelSure", "Sei sicuro di voler annullare l'installazione ?" },
            { "CancelButtonTitle", "Annulla" },
            { "AgreeText", "Accetto i termini del contratto di licenza" },
            { "LicenseText", licence },
            { "LicenseIntro", "Contratto di licenza" },
            { "SelectFolder", "Seleziona la cartella di installazione:" },
            { "InstallTitle", "Cartella di destinazione" },
            { "InstallInfo", "Il programma di installazione installerà RetroBat nella cartella indicata di seguito.\n\nPer continuare, fai clic su Avanti. Se vuoi specificare un'altra cartella, fai clic su Sfoglia." },
            { "InstallFolderHint", "Il programma richiede almeno 3,38 GB di spazio libero su disco.\n\nNon utilizzare cartelle con spazi o caratteri speciali." },
            { "FolderNotEmpty", "La cartella selezionata non è vuota. Scegli una cartella vuota o una nuova cartella." },
            { "ValidFolder", "Seleziona una cartella di installazione valida." },
            { "FailedFolder", "Impossibile creare la cartella di installazione: " },
            { "ExtractFail", "Impossibile estrarre i dati dell’installatore: " },
            { "InstallFail", "Installazione non riuscita: " },
            { "LaunchFail", "Impossibile avviare l'applicazione: " },
            { "ExeNotFound", "Eseguibile non trovato: " },
            { "InstallComplete", "Installazione completata!" },
            { "RunRetroBat", "Esegui RetroBat.exe" },
            { "Finish", "Fine" },
            { "Error", "Errore" },
            { "StartupError", "Si è verificato un errore di avvio. Consultare il file di registro per i dettagli." },
            { "PrerequisiteIntro", "Seleziona i componenti da installare prima di continuare." },
            { "vcText", "Microsoft Visual C++ Redistributable (2005–2022, x86 + x64)" },
            { "dx9text", "DirectX 9 (Eredità)" },
            { "dokanyText", "Dokany (usato per montare immagini XBOX con CXBX)" },
            { "DownloadAndInstall", "Download e installazione dei prerequisiti...\r\nAttendere prego..." },
            { "Downloading", "Download in corso" },
            { "Extracting", "Estrazione in corso" },
            { "InstallDX", "Installazione di DirectX..." },
            { "Installing", "Installazione in corso" },
            { "WaitingSelect", "In attesa della selezione..." }
        };

        private static Dictionary<string, string> polishStrings = new Dictionary<string, string> //OK
        {
            { "Cancel", "Anuluj" },
            { "WindowsTitle", "Instalator RetroBat " },
            { "Next", "Dalej" },
            { "Back", "Cofnij" },
            { "Browse", "Przeglądaj" },
            { "Install", "Zainstaluj" },
            { "Welcome", "Witamy w programie instalacyjnym RetroBat" },
            { "WelcomeText", "Ten kreator poprowadzi Cię przez proces instalacji programu RetroBat {0} {1} na Twoim komputerze.\n\nPrzed wykonaniem kolejnego kroku zaleca się zamknięcie wszystkich aktywnych aplikacji.\n\nKliknij Dalej, aby kontynuować, lub Anuluj, aby zamknąć instalator." },
            { "CancelSure", "Czy na pewno chcesz anulować instalację?" },
            { "CancelButtonTitle", "Anuluj" },
            { "AgreeText", "Akceptuję warunki umowy licencyjnej" },
            { "LicenseText", licence },
            { "LicenseIntro", "Umowa licencyjna" },
            { "SelectFolder", "Wybierz folder instalacyjny:" },
            { "InstallTitle", "Folder docelowy" },
            { "InstallInfo", "Program instalacyjny zainstaluje RetroBat w poniższym folderze.\n\nAby kontynuować, kliknij Dalej. Jeśli chcesz wybrać inny folder, kliknij Przeglądaj." },
            { "InstallFolderHint", "Program wymaga co najmniej 3,38 GB wolnego miejsca na dysku.\n\nNie używaj folderów zawierających spacje lub znaki specjalne." },
            { "FolderNotEmpty", "Wybrany folder nie jest pusty. Wybierz pusty folder lub nowy folder." },
            { "ValidFolder", "Wybierz prawidłowy folder instalacyjny." },
            { "FailedFolder", "Nie udało się utworzyć folderu instalacyjnego: " },
            { "ExtractFail", "Nie udało się rozpakować danych instalatora: " },
            { "InstallFail", "Instalacja nie powiodła się: " },
            { "LaunchFail", "Nie udało się uruchomić aplikacji: " },
            { "ExeNotFound", "Nie znaleziono pliku wykonywalnego: " },
            { "InstallComplete", "Instalowanie zakończone..." },
            { "RunRetroBat", "Uruchom RetroBat.exe" },
            { "Finish", "Zakończ" },
            { "Error", "Błąd" },
            { "StartupError", "Wystąpił błąd uruchamiania. Szczegółowe informacje znajdują się w pliku dziennika." },
            { "PrerequisiteIntro", "Przed kontynuowaniem wybierz komponenty do zainstalowania." },
            { "vcText", "Microsoft Visual C++ Redistributables (2005–2022, x86 + x64)" },
            { "dx9text", "DirectX 9 (Legacy)" },
            { "dokanyText", "Dokany (używany do montowania obrazów XBOX za pomocą CXBX)" },
            { "DownloadAndInstall", "Pobieranie i instalowanie wymaganych komponentów...\r\nProszę czekać..." },
            { "Downloading", "Pobieranie" },
            { "Extracting", "Rozpakowywanie" },
            { "InstallDX", "Instalowanie DirectX..." },
            { "Installing", "Instalowanie" },
            { "WaitingSelect", "Oczekiwanie na wybór..." }
        };

        private static Dictionary<string, string> spanishStrings = new Dictionary<string, string> //OK
        {
            { "Cancel", "Cancelar" },
            { "WindowsTitle", "Instalador de RetroBat " },
            { "Next", "Siguiente" },
            { "Back", "Atrás" },
            { "Browse", "Examinar" },
            { "Install", "Instalar" },
            { "Welcome", "Bienvenido al programa de instalación de Retrobat" },
            { "WelcomeText", "Este asistente lo guiará a través de la instalación de RetroBat {0} {1} en su computadora.\n\nSe recomienda cerrar todas las aplicaciones activas antes del siguiente paso.\n\nHaga click en Siguiente para continuar o en Cancelar para salir del instalador." },
            { "CancelSure", "¿Está seguro de que desea cancelar la instalación?" },
            { "CancelButtonTitle", "Cancelar" },
            { "AgreeText", "Acepto los términos del acuerdo de licencia" },
            { "LicenseText", licence },
            { "LicenseIntro", "Acuerdo de Licencia" },
            { "SelectFolder", "Seleccione la carpeta de instalación:" },
            { "InstallTitle", "Carpeta de destino" },
            { "InstallInfo", "El programa de instalación instalará RetroBat en la carpeta indicada a continuación.\n\nPara continuar, haga click en Siguiente. Si desea especificar otra carpeta, haga click en Examinar" },
            { "InstallFolderHint", "El programa requiere al menos 3.38 GB de espacio libre en el disco.\n\nNo uses carpetas con espacios ni caracteres especiales." },
            { "FolderNotEmpty", "La carpeta seleccionada no está vacía. Por favor elija una carpeta vacía o cree una nueva." },
            { "ValidFolder", "Por favor seleccione una carpeta de instalación válida." },
            { "FailedFolder", "Fallo al crear la carpeta de instalación: " },
            { "ExtractFail", "Fallo al extraer los datos del instalador: " },
            { "InstallFail", "Fallo en la instalación: " },
            { "LaunchFail", "Fallo al iniciar la aplicacion: " },
            { "ExeNotFound", "Ejecutable no encontrado: " },
            { "InstallComplete", "Instalación completa" },
            { "RunRetroBat", "Ejecutar RetroBat.exe" },
            { "Finish", "Finalizar" },
            { "Error", "Error" },
            { "StartupError", "Ocurrió un error de inicio. Consulte el archivo de registro para más detalles." },
            { "PrerequisiteIntro", "Seleccione los componentes que desea instalar antes de continuar." },
            { "vcText", "Microsoft Visual C++ Redistributables (2005–2022, x86 + x64)" },
            { "dx9text", "DirectX 9 (Legacy)" },
            { "dokanyText", "Dokany (usado para montar imágenes de XBOX con CXBX)" },
            { "DownloadAndInstall", "Descargando e instalando los requisitos previos...\r\nPor favor espere..." },
            { "Downloading", "Descargando" },
            { "Extracting", "Extrayendo" },
            { "InstallDX", "Instalando DirectX..." },
            { "Installing", "Instalando" },
            { "WaitingSelect", "Esperando la selección..." }
        };
        #endregion
    }
}
