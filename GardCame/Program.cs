using GardCame.Environment;
using GardCame.GameLoop;
using System.Diagnostics;
using System.Runtime.InteropServices;
using GardCame.Environment.Network;
using Spectre.Console;
using W = GardCame.Utils.WindowUtility;
using System.Text;
using GardCame;

#region fixed Size
const int MF_BYCOMMAND = 0x00000000;
const int SC_MINIMIZE = 0xF020;
const int SC_MAXIMIZE = 0xF030;
const int SC_SIZE = 0xF000;

[DllImport("user32.dll")]
static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

[DllImport("user32.dll")]
static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

[DllImport("kernel32.dll", ExactSpelling = true)]
static extern IntPtr GetConsoleWindow();


Console.WindowHeight = 30;
Console.WindowWidth = 120;
Console.SetBufferSize(120, 30);

DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MINIMIZE, MF_BYCOMMAND);
DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MAXIMIZE, MF_BYCOMMAND);
DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_SIZE, MF_BYCOMMAND);

#endregion

#region editMode

[DllImport("kernel32.dll", SetLastError = true)]
static extern bool GetConsoleMode(
    IntPtr hConsoleHandle,
    out int lpMode);

[DllImport("kernel32.dll", SetLastError = true)]
static extern bool SetConsoleMode(
    IntPtr hConsoleHandle,
    int ioMode);

const int STD_INPUT_HANDLE = -10;

[DllImport("Kernel32.dll", SetLastError = true)]
static extern IntPtr GetStdHandle(int nStdHandle);

const int QuickEditMode = 64;

const int ExtendedFlags = 128;

IntPtr conHandle = GetStdHandle(STD_INPUT_HANDLE);
int mode;

if (!GetConsoleMode(conHandle, out mode))
{
    Debug.WriteLine("Error setting mode");
    return;
}

mode = mode & ~(QuickEditMode | ExtendedFlags);

if (!SetConsoleMode(conHandle, mode))
{
    Debug.WriteLine("Error setting mode");
}
#endregion

W.SetConsoleWindowPosition(W.AnchorWindow.Fill);
Console.OutputEncoding = Encoding.UTF8;

string gameMode;
if (args.Count() > 0)
{
    if (args[0] == "-c")
    {
        gameMode = "Client";
    }
    else if (args[0] == "-s")
    {
        gameMode = "Server";
    }
}
else
{

    AnsiConsole.Write(
    new FigletText($"Gard Came:")
        .Centered()
        .Color(Color.Thistle3));
     AnsiConsole.Write(
    new FigletText($"Ninety Nine")
        .Centered()
        .Color(Color.White));

    gameMode = AnsiConsole.Prompt(
   new SelectionPrompt<string>()
       .Title("Choose an option?")
       .PageSize(10)
       .MoreChoicesText("[grey](Move up and down)[/]")
       .AddChoices(new[] {
                    "Server", "Client",
       }));

    LocalName = AnsiConsole.Ask<string>("What's your [green]name[/]?");
}


if (gameMode == "Server")
{
    //LocalName = "Server";
    Network = new ServerNetwork();
}
else
{
    IP = AnsiConsole.Ask<string>("Connection IP");
   // LocalName = "Client";
    Network = new ClientNetwork();
}

Network.Start();

Main mainGame = new Main();
MainLayout mainLayout = new MainLayout();
Network.Main = mainGame;

Console.Clear();

mainLayout.OnStart();
mainGame.OnStart();

Thread thr = new Thread(new ThreadStart(Loop));
thr.Start();

mainGame.MainLoop();

void Loop()
{
    while (!mainGame.Quit)
    {
        Console.CursorVisible = false;

        mainGame.OnUpdate();
        mainLayout.OnUpdate();
    }
    Console.Clear();
    AnsiConsole.Write(
    new FigletText($"{WinnerName} Won")
        .Centered()
        .Color(Color.Red));
    Thread.Sleep(3000);
}

public partial class Program
{
    public static string IP = "127.0.0.1";
    public static int PORT = 8080;


    public static Network Network;
    public static string LocalName;
    public static string WinnerName;
    public static bool GameStarted = false;
}

