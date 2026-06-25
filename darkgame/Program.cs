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

        // Rayon de la lampe de poche (5 cases) multiplié par lui-même pour l'optimisation
        static int RAYON_VISION_CARRE = 5 * 5;

        static char[,] carte;
        static int yuriX, yuriY;
        static int objectifX, objectifY;
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
                        // Astuce Anti-Lag : Calcul de la distance au carré entre la case actuelle et Yuri
                        int distanceX = x - yuriX;
                        int distanceY = y - yuriY;
                        int distanceTotaleCarre = (distanceX * distanceX) + (distanceY * distanceY);

                        // Si la case est DANS le cercle de lumière de la lampe de poche
                        if (distanceTotaleCarre <= RAYON_VISION_CARRE)
                        {
                            if (x == yuriX && y == yuriY)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("Y");
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
                                    // Affichage du sol visible avec un point discret
                                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                                    Console.Write(".");
                                    Console.ResetColor();
                                }
                            }
                        }
                        // Si la case est HORS du cercle de lumière (Dans le noir)
                        else
                        {
                            Console.Write(" "); // On affiche juste du vide noir
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

                if (yuriX == objectifX && yuriY == objectifY)
                {
                    niveau++;
                    GenererNiveauProcedural();
                    Console.Clear();
                }
            }
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