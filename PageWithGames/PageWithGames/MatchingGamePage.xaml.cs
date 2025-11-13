using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching; // Для IDispatcherTimer

namespace PageWithGames;

public partial class MatchingGamePage : ContentPage
{
    // Конфигурация игры
    private const int GridSize = 4;
    private const int TotalCards = GridSize * GridSize;
    private const int TotalPairs = TotalCards / 2;

    // Символы (8 уникальных пар)
    private readonly List<string> AvailableSymbols = new() { "🍎", "🍌", "🥝", "🍇", "🍍", "🍓", "🍒", "🥭" };

    // Игровые переменные
    private List<string> gameSymbols;
    private Button firstCard = null;
    private Button secondCard = null;
    private bool isBusy = false; // Флаг для блокировки кликов во время проверки
    private int matchesFound = 0;

    // Переменные для счета и времени
    private int totalMoves = 0; // Общее количество ходов (две открытые карты)
    private IDispatcherTimer gameTimer;
    private int secondsElapsed = 0; // Общее время в секундах

    public MatchingGamePage()
    {
        InitializeComponent();
        // При старте страницы сразу запускаем новую игру, чтобы UI не был пустым.
        OnRestartClicked(null, null);
    }

    // Обработчик нажатия кнопки "Перезапуск"
    private void OnRestartClicked(object sender, EventArgs e)
    {
        SetupGame();
    }

    // Обработчик для события Tick таймера
    private void GameTimer_Tick(object sender, EventArgs e)
    {
        secondsElapsed++;
        StatusLabel.Text = $"Ходов: {totalMoves} | Время игры: {secondsElapsed} сек.";
    }

    private void SetupGame()
    {
        // 1. Очистка и сброс состояния
        GameGrid.Children.Clear();
        firstCard = null;
        secondCard = null;
        isBusy = false;
        matchesFound = 0;
        totalMoves = 0;

        // 2. Установка и запуск Таймера (ИСПРАВЛЕННЫЙ БЛОК)
        secondsElapsed = 0;

        if (gameTimer != null)
        {
            gameTimer.Stop();
            // Отписываемся от старого события, чтобы избежать дублирования
            gameTimer.Tick -= GameTimer_Tick;
        }

        // Создаем таймер с 1 аргументом (интервал)
        gameTimer = Dispatcher.CreateTimer(TimeSpan.FromSeconds(1));

        // Подписываемся на событие Tick
        gameTimer.Tick += GameTimer_Tick;

        gameTimer.Start();

        StatusLabel.Text = $"Ходов: {totalMoves} | Время игры: {secondsElapsed} сек.";

        // 3. Генерация и перемешивание символов
        gameSymbols = AvailableSymbols
            .Concat(AvailableSymbols)
            .OrderBy(x => Guid.NewGuid())
            .ToList();

        // 4. Динамическое создание кнопок
        for (int row = 0; row < GridSize; row++)
        {
            for (int col = 0; col < GridSize; col++)
            {
                int index = row * GridSize + col;

                var button = new Button
                {
                    Text = "?",
                    FontSize = 40,
                    TextColor = Colors.White,
                    BackgroundColor = Color.FromHex("#512BD4"),
                    AutomationId = gameSymbols[index], // Сохраняем истинный символ
                };

                button.Clicked += CardClicked;

                Grid.SetRow(button, row);
                Grid.SetColumn(button, col);
                GameGrid.Children.Add(button);
            }
        }
    }

    private async void CardClicked(object sender, EventArgs e)
    {
        if (isBusy)
            return;

        var currentCard = sender as Button;

        // Игнорируем, если карта уже найдена или уже открыта
        if (currentCard.Text != "?" || currentCard == firstCard)
            return;

        // Открываем карту
        currentCard.Text = currentCard.AutomationId;

        if (firstCard == null)
        {
            // Это первая карта
            firstCard = currentCard;
            return;
        }

        // Это вторая карта
        secondCard = currentCard;
        isBusy = true; // Блокируем клики

        // УВЕЛИЧИВАЕМ СЧЕТЧИК ХОДОВ
        totalMoves++;
        // Статус обновится автоматически через Tick, но можно обновить явно:
        StatusLabel.Text = $"Ходов: {totalMoves} | Время игры: {secondsElapsed} сек.";

        // Задержка для просмотра второй карты
        await Task.Delay(1200);

        await CheckForMatch();
    }

    private async Task CheckForMatch()
    {
        if (firstCard.Text == secondCard.Text)
        {
            // Совпадение найдено
            firstCard.BackgroundColor = secondCard.BackgroundColor = Colors.Green;
            firstCard.Clicked -= CardClicked;
            secondCard.Clicked -= CardClicked;

            matchesFound++;

            CheckWin();
        }
        else
        {
            // Совпадение НЕ найдено, закрываем карты
            firstCard.Text = secondCard.Text = "?";
        }

        // Сбрасываем переменные для следующего хода
        firstCard = null;
        secondCard = null;
        isBusy = false; // Разблокируем клики
    }

    private void CheckWin()
    {
        if (matchesFound == TotalPairs)
        {
            gameTimer.Stop(); // ОСТАНОВКА ТАЙМЕРА
            DisplayAlert("Победа!", $"Вы нашли все пары за {totalMoves} ходов и {secondsElapsed} секунд!", "Отлично");
        }
    }
}