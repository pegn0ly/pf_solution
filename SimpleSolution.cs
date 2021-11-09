using System;
using System.Collections;

class SimpleSolution
{
	static void Main(string[] args)
	{
		// построение поля
		Console.Write("Enter field width: ");
		int width = Convert.ToInt32(Console.ReadLine());
		Console.Write("Enter field height: ");
		int height = Convert.ToInt32(Console.ReadLine());
		Console.WriteLine();
		//
		int[] arr = new int[height * width];
		FillField(arr, width, height);
		//
		DrawField(arr, width, height, null);
		//
		ArrayList ReachablePoints = new ArrayList();
		//
		Console.WriteLine("Enter start coords: ");
		int sx = Convert.ToInt32(Console.ReadLine()); 
		int sy = Convert.ToInt32(Console.ReadLine());
		ReachablePoints.Add(width * sx + sy);
		Console.WriteLine("Enter destination coords: ");
		int dx = Convert.ToInt32(Console.ReadLine());
		int dy = Convert.ToInt32(Console.ReadLine());
		//
		ArrayList PossiblePoints = GetReachablePointsFromThis(arr, width, height, width * sx + sy);
		DrawField(arr, width, height, PossiblePoints);
	}

	static void Swap(int x, int y)
	{
		int temp = x;
		x = y;
		y = temp;
	}

	static void FillField(int[] field, int width, int height)
	{
		int dim = width;
		if(width > height)
		{
			Swap(width, height);
			dim = height;
		}
		//
		Random rnd = new Random();
		//
		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				field[dim * i + j] = rnd.Next(2);
			}
		}
	}

	static void DrawField(int[] field, int width, int height, ArrayList points_to_check)
	{
		for(int i = 0; i < height; i++)
		{
			for(int j = 0; j < width; j++)
			{
				int point = width * i + j;
				if(points_to_check != null && points_to_check.Contains(point))
				{
					Console.Write("#" + " ");
				}
				else 
				{
					Console.Write(field[width * i + j] + " ");
				}
			}
			Console.WriteLine();
		}
	}

	// получает точки, достижимые из заданной
	static ArrayList GetReachablePointsFromThis(int[] field, int width, int height, int point)
	{
		ArrayList Answer = new ArrayList();
		// восстановить координаты точки, чтобы найти координаты, по которым будут находиться соседние точки
		int px = point % width;
		int py = point - (px * width);
		//
		for(int i = px - 1; i <= px + 1; i++)
		{
			for(int j = py - 1; j <= py + 1; j++)
			{
				int possible_point = width * i + j;
				// проверяемые точки не должны выходить за пределы поля. Также отбрасываются непроходимые точки.
				if(possible_point >= 0 && possible_point < (width * height) && field[possible_point] != 1)
				{
					Answer.Add(possible_point);
				}
			}
		}
		return Answer;
	}
}