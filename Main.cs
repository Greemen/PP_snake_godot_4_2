using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Control
{
	private const int WindowHeight = 16;
	private const int WindowWidth = 32;
	private const int InitialScore = 5;
	private const float GameSpeed = 0.5f; // seconds
	private const int CellSize = 20; // Adjust this value to the desired cell size
	private Vector2 berryPosition;
	private List<Vector2> body;
	private string currentDirection;
	private Vector2 head;
	private bool isGameOver;
	private Random random;

	private int score;

	private Label gameOverLabel;
	private Timer gameTimer;

	public override void _Ready()
	{
		// Set window size
		DisplayServer.WindowSetSize(new Vector2I(WindowWidth * CellSize, WindowHeight * CellSize));

		gameOverLabel = GetNode<Label>("GameOverLabel");
		gameTimer = GetNode<Timer>("GameTimer");

		InitializeGame();
		gameTimer.Timeout += OnGameTimerTimeout;
		gameTimer.WaitTime = GameSpeed;
		gameTimer.Start();
	}

	private void InitializeGame()
	{
		score = InitialScore;
		isGameOver = false;
		head = new Vector2(WindowWidth / 2, WindowHeight / 2);
		currentDirection = "RIGHT";
		body = new List<Vector2>();
		random = new Random();
		berryPosition = new Vector2(random.Next(1, WindowWidth - 2), random.Next(1, WindowHeight - 2));

		gameOverLabel.Hide();
	}

	private void OnGameTimerTimeout()
	{
		if (!isGameOver)
		{
			UpdateGame();
			QueueRedraw();
		}
	}

	public override void _Process(double delta)
	{
		if (!isGameOver)
		{
			HandleInput();
		}
	}

	private void HandleInput()
	{
		if (Input.IsActionPressed("ui_up") && currentDirection != "DOWN")
			currentDirection = "UP";
		if (Input.IsActionPressed("ui_down") && currentDirection != "UP")
			currentDirection = "DOWN";
		if (Input.IsActionPressed("ui_left") && currentDirection != "RIGHT")
			currentDirection = "LEFT";
		if (Input.IsActionPressed("ui_right") && currentDirection != "LEFT")
			currentDirection = "RIGHT";
	}

	private void UpdateGame()
	{
		MoveSnake();
		CheckCollision();
		CheckBerry();
	}

	private void MoveSnake()
	{
		body.Add(head);

		switch (currentDirection)
		{
			case "UP":
				head.Y--;
				break;
			case "DOWN":
				head.Y++;
				break;
			case "LEFT":
				head.X--;
				break;
			case "RIGHT":
				head.X++;
				break;
		}

		if (body.Count > score) body.RemoveAt(0);
	}

	private void CheckCollision()
	{
		if (head.X < 0 || head.X >= (WindowWidth-1) ||
			head.Y < 0 || head.Y >= (WindowHeight-1) ||
			body.Contains(head))
		{
			isGameOver = true;
			gameOverLabel.Text = $"Game over, Score: {score}";
			gameOverLabel.Show();
		}
	}

	private void CheckBerry()
	{
		if (head == berryPosition)
		{
			score++;
			berryPosition = new Vector2(random.Next(1, WindowWidth - 2), random.Next(1, WindowHeight - 2));
		}
	}

	public override void _Draw()
	{
		DrawBorders();
		DrawBerry();
		DrawSnake();
	}

	private void DrawBorders()
	{
		for (int i = 0; i < WindowWidth; i++)
		{
			DrawRect(new Rect2(i * CellSize, 0, CellSize, CellSize), new Color(1, 1, 1));
			DrawRect(new Rect2(i * CellSize, (WindowHeight - 1) * CellSize, CellSize, CellSize), new Color(1, 1, 1));
		}

		for (int i = 0; i < WindowHeight; i++)
		{
			DrawRect(new Rect2(0, i * CellSize, CellSize, CellSize), new Color(1, 1, 1));
			DrawRect(new Rect2((WindowWidth - 1) * CellSize, i * CellSize, CellSize, CellSize), new Color(1, 1, 1));
		}
	}

	private void DrawBerry()
	{
		DrawRect(new Rect2(berryPosition * CellSize, new Vector2(CellSize, CellSize)), new Color(0, 1, 1));
	}

	private void DrawSnake()
	{
		foreach (var segment in body)
		{
			DrawRect(new Rect2(segment * CellSize, new Vector2(CellSize, CellSize)), new Color(0, 1, 0));
		}

		DrawRect(new Rect2(head * CellSize, new Vector2(CellSize, CellSize)), new Color(1, 0, 0));
	}
}
