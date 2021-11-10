using System;
using System.Collections.Generic;

public struct DrawParams 
{
	public const ConsoleColor StartPointColor = ConsoleColor.Yellow;
	public const ConsoleColor DestPointColor = ConsoleColor.Red;
	public const ConsoleColor ReachablePointColor = ConsoleColor.Green;
	public const ConsoleColor CheckedPointColor = ConsoleColor.Blue;
	public int StartPoint;
	public int DestPoint;
	public List<int> CheckedPoints;
	public List<int> ReachablePoints;

	public DrawParams(int sp, int dp, List<int> cp, List<int> rp)
	{
		StartPoint = sp;
		DestPoint = dp;
		CheckedPoints = cp;
		ReachablePoints = rp;
	}	
}

public struct FieldDims
{
	public int[] Field;
	public int Width;
	public int Height;

	public FieldDims(int[] f, int w, int h)
	{
		Field = f;
		Width = w;
		Height = h;
	}
}

class SimpleSolution
{
	static void Main(string[] args)
	{
		Console.ForegroundColor = ConsoleColor.White;

		Console.Write("Enter field width: ");
		int width = Convert.ToInt32(Console.ReadLine());
		Console.Write("Enter field height: ");
		int height = Convert.ToInt32(Console.ReadLine());
		Console.WriteLine();

		int[] arr = new int[height * width];
		FieldDims FieldInfo = new FieldDims(arr, width, height);
		FillField(FieldInfo);

		DrawParams DrawInfo = new DrawParams(-1, -1, null, null);
		DrawField(FieldInfo, DrawInfo);

		Console.WriteLine("Enter start coords: ");
		int sx = Convert.ToInt32(Console.ReadLine()); 
		int sy = Convert.ToInt32(Console.ReadLine());
		Console.WriteLine("Enter destination coords: ");
		int dx = Convert.ToInt32(Console.ReadLine());
		int dy = Convert.ToInt32(Console.ReadLine());
		int start = width * sx + sy;
		int dest = width * dx + dy;

		SortedList<int, int> Path = FindPath(FieldInfo, start, dest);
		if(Path != null)
		{
			int prev = Path[dest];
			while(prev != start)
			{
				(int x, int y) Coords = PointToCoords(prev, FieldInfo);
				Console.WriteLine("[" + Coords.x + ", " + Coords.y + "] <- ");
				prev = Path[prev];
			}
		}
	}

	static void Swap(int x, int y)
	{
		int temp = x;
		x = y;
		y = temp;
	}

	static (int, int) PointToCoords(int point, FieldDims field_dims)
	{
		Console.WriteLine("Point: " + point);
		(int x, int y) Coords = (-1, -1);
		Coords.y = point % field_dims.Height;
		Coords.x = (point - Coords.y) / field_dims.Width;
		Console.WriteLine("x : " + Coords.x + ", y: " + Coords.y);
		return Coords;
	}

	static void FillField(FieldDims field_dims)
	{
		int dim = field_dims.Width;
		if(field_dims.Width > field_dims.Height)
		{
			Swap(field_dims.Width, field_dims.Height);
			dim = field_dims.Height;
		}
		//
		Random rnd = new Random();
		//
		for(int i = 0; i < field_dims.Width; i++)
		{
			for(int j = 0; j < field_dims.Height; j++)
			{
				field_dims.Field[dim * i + j] = rnd.Next(2);
			}
		}
	}

	static void DrawField(FieldDims field_dims, DrawParams draw_params)
	{
		for(int i = 0; i < field_dims.Height; i++)
		{
			for(int j = 0; j < field_dims.Width; j++)
			{
				int point = field_dims.Width * i + j;
				if(point == draw_params.StartPoint)
				{
					Console.ForegroundColor = DrawParams.StartPointColor;
				}
				else if(point == draw_params.DestPoint) 
				{
					Console.ForegroundColor = DrawParams.DestPointColor;
				}
				if(draw_params.CheckedPoints != null && draw_params.CheckedPoints.Contains(point))
				{
					Console.ForegroundColor = DrawParams.CheckedPointColor;
				}
				else if(draw_params.ReachablePoints != null && draw_params.ReachablePoints.Contains(point))
				{
					Console.ForegroundColor = DrawParams.ReachablePointColor;
				}
				Console.Write(field_dims.Field[point] + " ");
				Console.ForegroundColor = ConsoleColor.White;
			}
			Console.WriteLine();
		}
	}

	// получает точки, достижимые из заданной
	static List<int> GetReachablePointsFromThis(FieldDims field_dims, int point, List<int> checked_points)
	{
		List<int> Points = new List<int>();
		// восстановить координаты точки, чтобы найти координаты, по которым будут находиться соседние точки
		(int x, int y) PointCoords = PointToCoords(point, field_dims);

		for(int i = PointCoords.x - 1; i <= PointCoords.x + 1; i++)
		{
			for(int j = PointCoords.y - 1; j <= PointCoords.y + 1; j++)
			{
				int PossiblePoint = field_dims.Width * i + j;
				// проверяемые точки не должны выходить за пределы поля. Также отбрасываются непроходимые и уже пройденные точки.
				if(PossiblePoint >= 0 && PossiblePoint < (field_dims.Width * field_dims.Height) && 
					field_dims.Field[PossiblePoint] != 1 && 
					checked_points.Contains(PossiblePoint) == false)
				{
					Points.Add(PossiblePoint);
				}
			}
		}
		return Points;
	}

	static SortedList<int, int> FindPath(FieldDims field_dims, int start_point, int dest_point)
	{
		List<int> ReachablePoints = new List<int>();
		List<int> CheckedPoints = new List<int>();
		SortedList<int, int> Connections = new SortedList<int, int>();
		// стартовая точка, очевидно, является изначально достижимой
		ReachablePoints.Add(start_point);
		Connections.Add(start_point, start_point);
		// непосредственно поиск
		while(ReachablePoints.Count != 0)
		{
			Random rnd = new Random();
			// выбрать случайную точку из достижимых
			int point = Convert.ToInt32(ReachablePoints[rnd.Next(ReachablePoints.Count)]);
			// добавить ее в список проверенных и убрать из списка достижимых
			CheckedPoints.Add(point);
			ReachablePoints.Remove(point);
			// найти точки, достижимые из выбранной
			List<int> PossiblePoints = GetReachablePointsFromThis(field_dims, point, CheckedPoints);
			// если таких точек нет, проверяем следующую
			if(PossiblePoints.Count == 0)
			{
				continue;
			}
			// если среди них есть конечная - завершить алгоритм
			else if(PossiblePoints.Contains(dest_point))
			{
				DrawParams DrawInfo = new DrawParams(start_point, dest_point, CheckedPoints, ReachablePoints);
				DrawField(field_dims, DrawInfo);
				Console.WriteLine("Done!");
				Connections.Add(dest_point, point);
				return Connections;
			}
			// иначе добавить все найденные точки в список достижимых для дальнейшей проверки
			else
			{
				DrawParams DrawInfo = new DrawParams(start_point, dest_point, CheckedPoints, ReachablePoints);
				DrawField(field_dims, DrawInfo);
				Console.WriteLine();
				foreach (int pp in PossiblePoints) 
				{
					if(ReachablePoints.Contains(pp) == false)
					{
						ReachablePoints.Add(pp);
						Connections.Add(pp, point);
					}
				}
			}
		}
		return null;
	}
}