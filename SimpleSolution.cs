using System;
using System.Collections.Generic;

// Дано: поле размером y * x, каждая точка которого равна 0(проходимая) или 1(непроходимая);
// 		 стартовая точка [y1, x1], конечная точка [y2, x2].
// Алгоритм находит минимальный путь между этими точками, при условии, что он существует.
// При визуализации используются цвета - синий для уже проверенных точек; зеленый для точек, доступных для перемещения на текущем ходу; красный для конечной точки.


// информация для отрисовки поля(только для отладки)
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
		int Width = Convert.ToInt32(Console.ReadLine());
		Console.Write("Enter field height: ");
		int Height = Convert.ToInt32(Console.ReadLine());
		while(Width <= 0 || Height <= 0)
		{
			Console.WriteLine("ERROR: Field's dimensions can't be less or equal 0. Enter the new ones: ");
			Width = Convert.ToInt32(Console.ReadLine());
			Height = Convert.ToInt32(Console.ReadLine());
		}
		Console.WriteLine();

		int[] Field = new int[Height * Width];
		FieldDims FieldInfo = new FieldDims(Field, Width, Height);
		FillField(FieldInfo);

		DrawParams DrawInfo = new DrawParams(-1, -1, null, null);
		DrawField(FieldInfo, DrawInfo);

		Console.WriteLine("Enter start coords: ");
		int sx = Convert.ToInt32(Console.ReadLine()); 
		int sy = Convert.ToInt32(Console.ReadLine());
		int start = Width * sy + sx;
		while(start < 0 || start >= Width * Height || Field[start] == 1)
		{
			Console.WriteLine("ERROR: Start point is unreachable or out of field bounds. Enter new coords: ");
			sx = Convert.ToInt32(Console.ReadLine()); 
			sy = Convert.ToInt32(Console.ReadLine());
			start = Width * sy + sx;
		}

		Console.WriteLine("Enter destination coords: ");
		int dx = Convert.ToInt32(Console.ReadLine());
		int dy = Convert.ToInt32(Console.ReadLine());
		int dest = Width * dy + dx;
		while(dest < 0 || dest >= Width * Height || Field[dest] == 1)
		{
			Console.WriteLine("ERROR: Destination point is unreachable or out of field bounds. Enter new coords: ");
			dx = Convert.ToInt32(Console.ReadLine()); 
			dy = Convert.ToInt32(Console.ReadLine());
			dest = Width * dy + dx;
		}

		SortedList<int, int> Path = FindPath(FieldInfo, start, dest);
		if(Path != null)
		{
			int prev = Path[-1];
			do
			{
				(int x, int y) Coords = PointToCoords(prev, FieldInfo);
				Console.WriteLine("[" + Coords.x + ", " + Coords.y + "]");
				prev = Path[prev];
			} while(prev != -1);
		}
		else 
		{
			Console.WriteLine("Path not exists");
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
		//Console.WriteLine("Point: " + point);
		(int x, int y) Coords = (-1, -1);
		Coords.x = point % field_dims.Width;
		Coords.y = (point - Coords.x) / field_dims.Width;
		//Console.WriteLine("x : " + Coords.x + ", y: " + Coords.y);
		return Coords;
	}

	static void FillField(FieldDims field_dims)
	{
		Random rnd = new Random();
		//
		for(int i = 0; i < field_dims.Width; i++)
		{
			for(int j = 0; j < field_dims.Height; j++)
			{
				field_dims.Field[field_dims.Width * j + i] = rnd.Next(2);
			}
		}
	}

	static void DrawField(FieldDims field_dims, DrawParams draw_params)
	{
		for(int i = 0; i < field_dims.Width; i++)
		{
			for(int j = 0; j < field_dims.Height; j++)
			{
				int point = field_dims.Width * j + i;
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
		Console.WriteLine();
	}

	// получает точки, достижимые из заданной
	static List<int> GetReachablePointsFromThis(FieldDims field_dims, int point, List<int> checked_points)
	{
		//Console.WriteLine("Point: " + point);

		List<int> Points = new List<int>();
		// восстановить координаты точки, чтобы найти координаты, по которым будут находиться соседние точки
		(int x, int y) PointCoords = PointToCoords(point, field_dims);

		//Console.WriteLine("Testing point: " + PointCoords.x + ", " + PointCoords.y);

		for(int i = (PointCoords.x - 1); i <= (PointCoords.x + 1); i++)
		{
			for(int j = (PointCoords.y - 1); j <= (PointCoords.y + 1); j++)
			{
				// Console.WriteLine("Checking coords: " + i + ", " + j);
				// координаты не должны выходить за пределы размеров поля
				if((i >= 0 && j >= 0) && i < field_dims.Width && j < field_dims.Height)
				{
					int PossiblePoint = field_dims.Width * j + i;
					// также отбрасываются непроходимые и уже пройденные точки.
					if(PossiblePoint >= 0 && PossiblePoint < (field_dims.Width * field_dims.Height) && 
						field_dims.Field[PossiblePoint] != 1 && 
						checked_points.Contains(PossiblePoint) == false)
					{
						Points.Add(PossiblePoint);
					}
					// Console.WriteLine("Point founded: " + PossiblePoint);
					// if(PossiblePoint < 0 || PossiblePoint >= (field_dims.Width * field_dims.Height))
					// {
					// 	Console.WriteLine(" and it's not reachable cause it's out of field");
					// }
					// else if(field_dims.Field[PossiblePoint] == 1) 
					// {
					// 	Console.WriteLine(" and it's not reachable cause it's blocked");
					// }
					// else if(checked_points.Contains(PossiblePoint))
					// {
					// 	Console.WriteLine(" and it's not reachable cause it's already checked");
					// }
					// else 
					// {
					// 	Console.WriteLine(" and it's reachable");
					// 	Points.Add(PossiblePoint);
					// }
				}
			}
		}

		// Console.WriteLine("Reachable points founded: ");
		// foreach(int p in Points)
		// {
		// 	Console.WriteLine(p);
		// }

		return Points;
	}

	// из списка достижимых точек, находит максимально близкую к point
	static int GetClosestPointToThis(int point, List<int> reachable_points, FieldDims field_dims)
	{
		double ClosestDist = Double.MaxValue;
		int CurrentPoint = -1;
		foreach(int p in reachable_points)
		{
			(int x, int y) Coords = PointToCoords(point, field_dims);
			(int x, int y) pCoords = PointToCoords(p, field_dims);
			double CurrDist = Math.Sqrt(Math.Pow((Coords.x - pCoords.x), 2)  + Math.Pow((Coords.y - pCoords.y), 2));
			if(CurrDist <= ClosestDist)
			{
				ClosestDist = CurrDist;
				CurrentPoint = p;
			}
		}
		return CurrentPoint;
	}

	static SortedList<int, int> FindPath(FieldDims field_dims, int start_point, int dest_point)
	{
		List<int> ReachablePoints = new List<int>();
		List<int> CheckedPoints = new List<int>();
		SortedList<int, int> Connections = new SortedList<int, int>();
		// стартовая точка, очевидно, является изначально достижимой
		ReachablePoints.Add(start_point);
		Connections.Add(start_point, -1);
		Connections.Add(-1, dest_point);
		DrawParams DrawInfo = new DrawParams(start_point, dest_point, CheckedPoints, ReachablePoints);
		// непосредственно поиск
		while(ReachablePoints.Count != 0)
		{
			DrawField(field_dims, DrawInfo);
			// выбрать ближайшую к конечной точку из достижимых
			int Point = GetClosestPointToThis(dest_point, ReachablePoints, field_dims);
			// добавить ее в список проверенных и убрать из списка достижимых
			CheckedPoints.Add(Point);
			ReachablePoints.Remove(Point);
			// найти точки, достижимые из выбранной
			List<int> PossiblePoints = GetReachablePointsFromThis(field_dims, Point, CheckedPoints);
			// если таких точек нет, проверяем следующую
			if(PossiblePoints.Count == 0)
			{
				continue;
			}
			// если среди них есть конечная - завершить алгоритм
			else if(PossiblePoints.Contains(dest_point))
			{
				//DrawParams DrawInfo = new DrawParams(start_point, dest_point, CheckedPoints, ReachablePoints);
				DrawField(field_dims, DrawInfo);
				Console.WriteLine("Done!");
				Connections.Add(dest_point, Point);
				return Connections;
			}
			// иначе добавить все найденные точки в список достижимых для дальнейшей проверки
			else
			{
				//DrawParams DrawInfo = new DrawParams(start_point, dest_point, CheckedPoints, ReachablePoints);
				DrawField(field_dims, DrawInfo);
				foreach (int pp in PossiblePoints) 
				{
					if(ReachablePoints.Contains(pp) == false)
					{
						ReachablePoints.Add(pp);
						Connections.Add(pp, Point);
					}
				}
			}
		}
		return null;
	}
}