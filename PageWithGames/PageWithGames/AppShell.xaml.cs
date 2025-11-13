namespace PageWithGames;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // --- РЕГИСТРАЦИЯ ИГР ---
        // 1. Крестики-нолики
        Routing.RegisterRoute("TicTacToe", typeof(TicTacToeModePage));
        Routing.RegisterRoute("TicTacToeGame", typeof(TicTacToeGamePage));
        // 2. Парные символы (Мемо-игра)
        Routing.RegisterRoute("MatchingGame", typeof(MatchingGamePage));

        // --- ЗАГЛУШКИ ДЛЯ БУДУЩИХ ИГР ---
        // Важно: Эти маршруты должны указывать на ContentPage,
        // так как они еще не созданы.
        Routing.RegisterRoute("Minesweeper", typeof(ContentPage));
        Routing.RegisterRoute("Puzzle", typeof(ContentPage));
    }
}