using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Player
{
    static int MAX_X = 23;
    static int MAX_Y = 21;
    static int CANNON_RANGE = 10;
    
    class Board {
        public Board() {
            Entities = new Dictionary<int, Entity>();
        }
        public int MyShipCount { get; set; }
        public Dictionary<int, Entity> Entities { get; private set; }
        public IEnumerable<Entity> MyShips { 
            get { 
                return Entities.Values.Where(e => e.T == EType.SHIP && e.Owner == 1);
            }
        }
    }
    
    public struct Coord {
        private static double SQRT3_2 = Math.Sqrt(3) / 2;
        private static double THREE_OVER_PI = 3 / Math.PI;
        
        private static int[][] DIR_EVEN = new int[][] { new int[] { 1, 0 }, new int[] { 0, -1 }, new int[] { -1, -1 }, new int[] { -1, 0 }, new int[] { -1, 1 }, new int[] { 0, 1 } };
        private static int[][] DIR_ODD = new int[][] { new int[] { 1, 0 }, new int[] { 1, -1 }, new int[] { 0, -1 }, new int[] { -1, 0 }, new int[] { 0, 1 }, new int[] { 1, 1 } };
        public readonly int X;
        public readonly int Y;

        public Coord(int x, int y) {
            this.X = x;
            this.Y = y;
        }

        public static double Angle(Coord first, Coord second) {
            double dy = (second.Y - first.X) * SQRT3_2;
            double dx = second.X - first.X + ((first.Y - second.Y) & 1) * 0.5;
            double angle = -Math.Atan2(dy, dx) * THREE_OVER_PI;
            if (angle < 0) {
                angle += 6;
            } else if (angle >= 6) {
                angle -= 6;
            }
            return angle;
        }

        public Hex ToHex() {
            int xp = X - (Y - (Y & 1)) / 2;
            int zp = Y;
            int yp = -(xp + zp);
            return new Hex(xp, yp, zp);
        }

        public Coord Neighbor(int orientation) {
            int newY, newX;
            if (this.Y % 2 == 1) {
                newY = this.Y + DIR_ODD[orientation][1];
                newX = this.X + DIR_ODD[orientation][0];
            } else {
                newY = this.Y + DIR_EVEN[orientation][1];
                newX = this.X + DIR_EVEN[orientation][0];
            }

            return new Coord(newX, newY);
        }

        public bool IsValid() {
            return X >= 0 && X < MAX_X && Y >= 0 && Y < MAX_Y;
        }

        public int Dist(Coord other) {
            return Hex.Dist(this.ToHex(), other.ToHex());
        }
        
        public override bool Equals(object obj)
        {                       
            if (!(obj is Coord)) return false;            
            return ((Coord)obj).Y == this.Y && ((Coord)obj).X == this.X;
        }
        
        public override int GetHashCode()
        {
            return this.X + this.Y * MAX_X;
        }
        
        public override String ToString() {
            return X + "," + Y;
        }
    }
    
    public struct Hex {
        public static Hex Zero = new Hex(0,0,0);
        
        public static Hex[] DIR = new Hex[] { new Hex(1,-1,0), new Hex(1,0,-1), new Hex(0,1,-1), new Hex(-1,1,0), new Hex(-1,0,1), new Hex(0,-1,1)};
        public readonly int X;
        public readonly int Y;
        public readonly int Z;

        public Hex(int x, int y, int z) {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Coord ToOffset() {
            int col = X + (Z - (Z & 1)) / 2;
            int row = Z;
            return new Coord(col, row);
        }

        public Hex Neighbor(int orientation) {
            int nx = this.X + DIR[orientation].X;
            int ny = this.Y + DIR[orientation].Y;
            int nz = this.Z + DIR[orientation].Z;

            return new Hex(nx, ny, nz);
        }

        public static Hex operator +(Hex lhs, Hex rhs ) 
		{
			return new Hex(lhs.X+rhs.X,lhs.Y+rhs.Y,lhs.Z+rhs.Z);
		}
        public static Hex operator -(Hex lhs, Hex rhs ) 
		{
			return new Hex(lhs.X-rhs.X,lhs.Y-rhs.Y,lhs.Z-rhs.Z);
		}
		public static bool operator ==(Hex lhs, Hex rhs ) 
		{
			return (lhs.X == rhs.X) && (lhs.Y == rhs.Y) && (lhs.Z == rhs.Z);
		}
		public static bool operator !=(Hex lhs, Hex rhs ) 
		{
			return (lhs.X != rhs.X) && (lhs.Y != rhs.Y) && (lhs.Z != rhs.Z);
		}
		public override bool Equals(Object o) 
		{
		    Hex rhs = ((Hex)o);
			return (this.X == rhs.X) && (this.Y == rhs.Y) && (this.Z == rhs.Z);
		}
		
        public static int Dist(Hex src, Hex dst) {
            return (Math.Abs(src.X - dst.X) + Math.Abs(src.Y - dst.Y) + Math.Abs(src.Z - dst.Z)) / 2;
        }

        public override int GetHashCode()
        {
            return this.X + this.Y * MAX_X + this.Z * MAX_X * MAX_X;
        }
        
        public override String ToString() {
            return X + "," + Y + "," + Z;
        }
    }

    enum EType {
        SHIP,
        BARREL,
        MINE,
        CANNONBALL
    }
    class Entity {
        public Entity (int id, EType t, int x, int y) {
            Id = id; Pos = new Coord(x, y).ToHex(); T = t;
        }
        public int Id { get; set; } // id
        public EType T { get; set; } // type
        public Hex Pos { get; set; }        
        public int Owner { get; set; }
        public int Speed { get; set; }
        public int Stock { get; set; }
        public int Angle { get; set; }
        public int CoolOff { get; set; }
        public override string ToString() {
            return String.Format("{0}.{1}: ({2})", T.ToString()[0], Id, Pos.ToOffset());
        }
    }
    class Action {
        public Action(string verb) {
            Verb = verb;
        }
        public string Verb { get; private set; }
        public int Arg1 { get; private set; }
        public int Arg2 { get; private set; }

        public override string ToString() {
            if(Verb == "MOVE" || Verb == "FIRE") {
                return Verb + " " + Arg1 + " " + Arg2;
            }else 
                return Verb;
        }
        public static Action Move(int x, int y) { return new Action("MOVE") { Arg1 = x, Arg2 = y }; }
        public static Action Move(Coord c) { return new Action("MOVE") { Arg1 = c.X, Arg2 = c.Y }; }
        public static Action Fire(int x, int y) { return new Action("FIRE") { Arg1 = x, Arg2 = y }; }
        public static Action Fire(Coord c) { return new Action("FIRE") { Arg1 = c.X, Arg2 = c.Y }; }
        
        public static Action Wait() { return new Action("WAIT"); }
        public static Action Mine() { return new Action("MINE"); }
        public static Action Slow() { return new Action("SLOWER"); }
    }
    
    static void Main(string[] args)
    {        
        // game loop
        int lastClosest = -1; Board prevBoard = null;
        while (true)
        {
            Board board = new Board();
            board.MyShipCount = int.Parse(Console.ReadLine()); // the number of remaining ships
            int entityCount = int.Parse(Console.ReadLine()); // the number of entities (e.g. ships, mines or cannonballs)
            for (int i = 0; i < entityCount; i++)
            {
                string[] inputs = Console.ReadLine().Split(' ');
                Entity ent = new Entity(int.Parse(inputs[0]), (EType)Enum.Parse(typeof(EType), inputs[1]), int.Parse(inputs[2]), int.Parse(inputs[3]));
                if(ent.T == EType.SHIP) {
                    ent.Angle = int.Parse(inputs[4]);
                    ent.Speed = int.Parse(inputs[5]);
                    ent.Stock = int.Parse(inputs[6]);
                    ent.Owner = int.Parse(inputs[7]);
                }else if(ent.T == EType.BARREL) {
                    ent.Stock = int.Parse(inputs[4]);
                }
                
                if(prevBoard != null && prevBoard.Entities.ContainsKey(ent.Id)) {
                    ent.CoolOff = Math.Max(0, prevBoard.Entities[ent.Id].CoolOff - 1);
                }
                board.Entities.Add(ent.Id, ent);                
            }

            foreach(var ship in board.MyShips)
            {
                Hex shipFront = ship.Pos.Neighbor(ship.Angle);
                Console.Error.Write(" -- Ship " + ship.Id + " at " + ship.Pos.ToOffset()); 
                Console.Error.WriteLine(" facing " + ship.Angle + " (front at: " + shipFront.ToOffset() + ") ------------ ");
                
                Hex? canonTarget = null; bool firePriority = false;
                if (ship.CoolOff == 0) {
                    // if I can fire the canon, let's see what targets we can hit..
                    foreach(var enemyShip in board.Entities.Values.Where(e => e.Owner == 0 && e.T == EType.SHIP)) {
                        var enemyFront = enemyShip.Pos.Neighbor(enemyShip.Angle);
                        var canonDist = Hex.Dist(shipFront, enemyFront);
                        var canonTime = 1 + (canonDist / 3);
                        if (canonTime < 3) {
                            canonTarget = enemyFront;
                            if(canonTime == 1) firePriority = true;
                            break;
                        }
                        Console.Error.WriteLine(" * Enemy Ship " + enemyShip.Id + " facing " + enemyShip.Angle + " (front at: " + enemyFront.ToOffset() + "), dist = " + canonDist + ", turns = " + canonTime);                                                                                     
                    }
                }
                
                
                Action nextAction = null;
                Entity closest;
                if(board.Entities.ContainsKey(lastClosest)) {
                    closest = board.Entities[lastClosest];
                }else {
                    closest = board.Entities.Values.Where(e => e.T == EType.BARREL)
                        .OrderByDescending(b => b.Stock - Hex.Dist(ship.Pos, b.Pos)).FirstOrDefault();
                        
                    if(closest != null) {
                        lastClosest = closest.Id;
                    }
                }
                if(closest != null) {
                    Console.Error.WriteLine("closest is :" + closest);                
                    nextAction = Action.Move(closest.Pos.ToOffset());                    
                }else {
                    Hex upperLeft = new Coord(0,0).ToHex();
                    Hex upperRight = new Coord(22,0).ToHex();
                    
                    int upperLeftDist = 0; int upperRightDist = 0;
                    foreach(var e in board.Entities.Values) {                        
                        if(e.T == EType.SHIP && e.Owner == 0) {
                            // enemy boat..
                            upperLeftDist += Hex.Dist(e.Pos, upperLeft);
                            upperRightDist += Hex.Dist(e.Pos, upperRight);
                        }
                    }
                    //Console.Error.WriteLine("upperLeft: " + upperLeftDist + ", upperRight: " + upperRightDist);
                    
                    if(upperLeftDist < upperRightDist) {
                        nextAction = Action.Move(upperRight.ToOffset());
                    }else if(upperLeftDist > upperRightDist) {
                        nextAction = Action.Move(upperLeft.ToOffset());
                    }else {
                        if (Hex.Dist(ship.Pos, upperLeft) < Hex.Dist(ship.Pos, upperRight)) {
                            nextAction = Action.Move(upperRight.ToOffset());
                        }else {
                            nextAction = Action.Move(upperLeft.ToOffset());
                        }
                    }                    
                }
                
                if(canonTarget.HasValue && (firePriority || ship.Speed > 0)) {
                    ship.CoolOff = 2;
                    Console.WriteLine(Action.Fire(canonTarget.Value.ToOffset()));                    
                }else {
                    Console.WriteLine(nextAction);
                }                
            }
            
            prevBoard = board;
        }
    }
}