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
		int dest_point = width * dx + dy;
		// массив уже проверенных точек
		ArrayList CheckedPoints = new ArrayList();
		// непосредственно поиск
		while(ReachablePoints.Count != 0)
		{
			// выбрать первую точку из достижимых
			int point = Convert.ToInt32(ReachablePoints[0]);
			// добавить ее в массив проверенных и убрать из массива достижимых
			CheckedPoints.Add(point);
			ReachablePoints.Remove(point);
			// найти точки, достижимые из выбранной
			ArrayList PossiblePoints = GetReachablePointsFromThis(arr, width, height, point, CheckedPoints);
			// если таких точек нет, проверяем следующую
			if(PossiblePoints.Count == 0)
			{
				continue;
			}
			// если среди них есть конечная - завершить алгоритм
			else if(PossiblePoints.Contains(dest_point))
			{
				Console.WriteLine("Done!");
				break;
			}
			// иначе добавить все найденные точки в массив достижимых для дальнейшей проверки
			else
			{
				DrawField(arr, width, height, PossiblePoints);
				Console.WriteLine();
				foreach (int pp in PossiblePoints) 
				{
					if(ReachablePoints.Contains(pp) == false)
					{
						ReachablePoints.Add(pp);
					}
				}
			}
		}
		//Console.WriteLine("Path not found");
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
	static ArrayList GetReachablePointsFromThis(int[] field, int width, int height, int point, ArrayList checked_points)
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
				// проверяемые точки не должны выходить за пределы поля. Также отбрасываются непроходимые и уже пройденные точки.
				if(possible_point >= 0 && possible_point < (width * height) && field[possible_point] != 1 && checked_points.Contains(possible_point) == false)
				{
					Answer.Add(possible_point);
				}
			}
		}
		return Answer;
	}
}