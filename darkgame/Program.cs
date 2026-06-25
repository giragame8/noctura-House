using System;
using System.Collections.Generic;

namespace YuriDarkFantasy
{
    class Piece
    {
        public int X, Y, Largeur, Hauteur;

        public int CentreX => X + Largeur / 2;
        public int CentreY => Y + Hauteur / 2;

        public Piece(int x, int y, int largeur, int hauteur)
        {
            X = x;
            Y = y;
            Largeur = largeur;
            Hauteur = hauteur;
        }

        public bool Intersecte(Piece autre)
        {
            return (X <= autre.X + autre.Largeur + 1 && X + Largeur + 1 >= autre.X &&
                    Y <= autre.Y + autre.Hauteur + 1 && Y + Hauteur + 1 >= autre.Y);
        }
    }

    class Program
    {
        static int LARGEUR = 60;
        static int HAUTEUR = 25;

        static int RAYON_VISION_CARRE = 5 * 5;

        static char[,] carte;
        static int yuriX, yuriY;
        static int objectifX, objectifY;

        static int ennemiX, ennemiY;
        static bool ennemiActif = false;

        static int niveau = 1;

        static Random random = new Random();

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.Clear();

            GenererNiveauProcedural();

            bool jeuEnCours = true;

            while (jeuEnCours)
            {
                Console.SetCursorPosition(0, 0);

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"--- LA DEMEURE DES DEMONS | NIVEAU {niveau} ---");
                Console.ResetColor();

                for (int y = 0; y < HAUTEUR; y++)
                {
                    for (int x = 0; x < LARGEUR; x++)
                    {
                        int distanceX = x - yuriX;
                        int distanceY = y - yuriY;
                        int distanceTotaleCarre = (distanceX * distanceX) + (distanceY * distanceY);

                        if (distanceTotaleCarre <= RAYON_VISION_CARRE)
                        {
                            if (x == yuriX && y == yuriY)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("Y");
                                Console.ResetColor();
                            }
                            else if (ennemiActif && x == ennemiX && y == ennemiY)
                            {
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.Write("M");
                                Console.ResetColor();
                            }
                            else if (x == objectifX && y == objectifY)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write("O");
                                Console.ResetColor();
                            }
                            else
                            {
                                char tuile = carte[y, x];
                                if (tuile == '#')
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                    Console.Write("#");
                                    Console.ResetColor();
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                                    Console.Write(".");
                                    Console.ResetColor();
                                }
                            }
                        }
                        else
                        {
                            Console.Write(" ");
                        }
                    }
                    Console.WriteLine();
                }

                while (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                }

                ConsoleKeyInfo touche = Console.ReadKey(true);

                int futurX = yuriX;
                int futurY = yuriY;

                if (touche.Key == ConsoleKey.UpArrow) futurY--;
                else if (touche.Key == ConsoleKey.DownArrow) futurY++;
                else if (touche.Key == ConsoleKey.LeftArrow) futurX--;
                else if (touche.Key == ConsoleKey.RightArrow) futurX++;
                else if (touche.Key == ConsoleKey.Escape) jeuEnCours = false;

                if (futurX >= 0 && futurX < LARGEUR && futurY >= 0 && futurY < HAUTEUR)
                {
                    if (carte[futurY, futurX] != '#')
                    {
                        yuriX = futurX;
                        yuriY = futurY;
                    }
                }

                if (ennemiActif)
                {
                    DeplacerEnnemi();

                    if (yuriX == ennemiX && yuriY == ennemiY)
                    {
                        DeclencherScreamer();
                        jeuEnCours = false;
                    }
                }

                if (jeuEnCours && yuriX == objectifX && yuriY == objectifY)
                {
                    niveau++;
                    GenererNiveauProcedural();
                    Console.Clear();
                }
            }
        }

        static void DeplacerEnnemi()
        {
            int distX = Math.Abs(yuriX - ennemiX);
            int distY = Math.Abs(yuriY - ennemiY);

            int futurEnnemiX = ennemiX;
            int futurEnnemiY = ennemiY;

            if (distX > distY)
            {
                futurEnnemiX += (yuriX > ennemiX) ? 1 : -1;
                if (carte[ennemiY, futurEnnemiX] != '#') ennemiX = futurEnnemiX;
                else
                {
                    futurEnnemiY += (yuriY > ennemiY) ? 1 : -1;
                    if (carte[futurEnnemiY, ennemiX] != '#') ennemiY = futurEnnemiY;
                }
            }
            else
            {
                futurEnnemiY += (yuriY > ennemiY) ? 1 : -1;
                if (carte[futurEnnemiY, ennemiX] != '#') ennemiY = futurEnnemiY;
                else
                {
                    futurEnnemiX += (yuriX > ennemiX) ? 1 : -1;
                    if (carte[ennemiY, futurEnnemiX] != '#') ennemiX = futurEnnemiX;
                }
            }
        }

        static void DeclencherScreamer()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Clear();

            string[] visageDemon = {
                "                                                ",
                "   @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@   ",
                "  @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@  ",
                "  @@@@                                    @@@@  ",
                "  @@@@    @@@@@@@@            @@@@@@@@    @@@@  ",
                "  @@@@  @@@@@@@@@@@@        @@@@@@@@@@@@  @@@@  ",
                "  @@@@  @@@@@@@@@@@@        @@@@@@@@@@@@  @@@@  ",
                "  @@@@    @@@@@@@@            @@@@@@@@    @@@@  ",
                "  @@@@                                    @@@@  ",
                "  @@@@                                    @@@@  ",
                "  @@@@        @@@@@@@@@@@@@@@@@@@@        @@@@  ",
                "  @@@@        @@@@@@@@@@@@@@@@@@@@        @@@@  ",
                "  @@@@          @@@@@@@@@@@@@@@@          @@@@  ",
                "  @@@@                                    @@@@  ",
                "  @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@  ",
                "   @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@   "
            };

            int startY = (HAUTEUR / 2) - (visageDemon.Length / 2);
            for (int i = 0; i < visageDemon.Length; i++)
            {
                Console.SetCursorPosition((LARGEUR / 2) - (visageDemon[i].Length / 2), startY + i);
                Console.WriteLine(visageDemon[i]);
            }

            Console.Beep(800, 200);
            Console.Beep(1200, 200);
            Console.Beep(2000, 600);

            Console.ResetColor();
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n\n\n\t\tYURI A ETE ATTRAPEE PAR LE DEMON...\n\n\t\tGAME OVER");
            Console.WriteLine("\n\t\tAppuie sur une touche pour quitter.");
            Console.ReadKey();
        }

        static void GenererNiveauProcedural()
        {
            carte = new char[HAUTEUR, LARGEUR];

            for (int y = 0; y < HAUTEUR; y++)
            {
                for (int x = 0; x < LARGEUR; x++)
                {
                    carte[y, x] = '#';
                }
            }

            List<Piece> pieces = new List<Piece>();
            int maxPieces = 8;

            for (int i = 0; i < 30; i++)
            {
                int l = random.Next(4, 10);
                int h = random.Next(4, 8);
                int x = random.Next(1, LARGEUR - l - 1);
                int y = random.Next(1, HAUTEUR - h - 1);

                Piece nouvellePiece = new Piece(x, y, l, h);
                bool chevauche = false;

                foreach (Piece p in pieces)
                {
                    if (nouvellePiece.Intersecte(p))
                    {
                        chevauche = true;
                        break;
                    }
                }

                if (!chevauche)
                {
                    CreuserPiece(nouvellePiece);

                    if (pieces.Count == 0)
                    {
                        yuriX = nouvellePiece.CentreX;
                        yuriY = nouvellePiece.CentreY;
                    }
                    else
                    {
                        Piece precedente = pieces[pieces.Count - 1];
                        CreuserCouloir(precedente.CentreX, precedente.CentreY, nouvellePiece.CentreX, nouvellePiece.CentreY);
                    }

                    pieces.Add(nouvellePiece);

                    objectifX = nouvellePiece.CentreX;
                    objectifY = nouvellePiece.CentreY;

                    if (pieces.Count >= maxPieces) break;
                }
            }

            if (pieces.Count > 1)
            {
                Piece pieceEnnemi = pieces[pieces.Count - 2];
                ennemiX = pieceEnnemi.CentreX;
                ennemiY = pieceEnnemi.CentreY;
                ennemiActif = true;
            }
            else
            {
                ennemiActif = false;
            }
        }

        static void CreuserPiece(Piece piece)
        {
            for (int y = piece.Y; y < piece.Y + piece.Hauteur; y++)
            {
                for (int x = piece.X; x < piece.X + piece.Largeur; x++)
                {
                    carte[y, x] = '.';
                }
            }
        }

        static void CreuserCouloir(int x1, int y1, int x2, int y2)
        {
            int x = x1;
            int y = y1;

            if (random.Next(0, 2) == 0)
            {
                while (x != x2) { carte[y, x] = '.'; x += (x < x2) ? 1 : -1; }
                while (y != y2) { carte[y, x] = '.'; y += (y < y2) ? 1 : -1; }
            }
            else
            {
                while (y != y2) { carte[y, x] = '.'; y += (y < y2) ? 1 : -1; }
                while (x != x2) { carte[y, x] = '.'; x += (x < x2) ? 1 : -1; }
            }
        }
    }
}