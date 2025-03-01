using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace liv_inParis
{
    public partial class Form1 : Form
    {
        private Graphe graphe; // Le graphe
        private List<int> noeudsSelectionnes = new List<int>(); // Liste des n�uds s�lectionn�s
        private List<int> cheminPlusCourt = new List<int>(); // Chemin le plus court entre les n�uds s�lectionn�s
        private List<Lien> circuit = new List<Lien>(); // Ar�tes du circuit d�tect�

        public Form1()
        {
            InitializeComponent();
            this.Size = new Size(1000, 800); // Agrandir la fen�tre
            InitialiserGraphe(); // Initialiser le graphe
            this.MouseClick += Form1_MouseClick; // Ajouter le gestionnaire d'�v�nements
        }

        private void InitialiserGraphe()
        {
            // Initialisation des ar�tes (connexions entre n�uds)
            var aretes = new List<Tuple<int, int>>
            {
                Tuple.Create(2, 1), Tuple.Create(3, 1), Tuple.Create(4, 1),
                Tuple.Create(5, 1), Tuple.Create(6, 1), Tuple.Create(7, 1),
                Tuple.Create(8, 1), Tuple.Create(9, 1), Tuple.Create(11, 1),
                Tuple.Create(12, 1), Tuple.Create(13, 1), Tuple.Create(14, 1),
                Tuple.Create(18, 1), Tuple.Create(20, 1), Tuple.Create(22, 1),
                Tuple.Create(32, 1), Tuple.Create(3, 2), Tuple.Create(4, 2),
                Tuple.Create(8, 2), Tuple.Create(14, 2), Tuple.Create(18, 2),
                Tuple.Create(20, 2), Tuple.Create(22, 2), Tuple.Create(31, 2),
                Tuple.Create(4, 3), Tuple.Create(8, 3), Tuple.Create(9, 3),
                Tuple.Create(10, 3), Tuple.Create(14, 3), Tuple.Create(28, 3),
                Tuple.Create(29, 3), Tuple.Create(33, 3), Tuple.Create(8, 4),
                Tuple.Create(13, 4), Tuple.Create(14, 4), Tuple.Create(7, 5),
                Tuple.Create(11, 5), Tuple.Create(7, 6), Tuple.Create(11, 6),
                Tuple.Create(17, 6), Tuple.Create(17, 7), Tuple.Create(31, 9),
                Tuple.Create(33, 9), Tuple.Create(34, 9), Tuple.Create(34, 10),
                Tuple.Create(34, 14), Tuple.Create(33, 15), Tuple.Create(34, 15),
                Tuple.Create(33, 16), Tuple.Create(34, 16), Tuple.Create(33, 19),
                Tuple.Create(34, 19), Tuple.Create(34, 20), Tuple.Create(33, 21),
                Tuple.Create(34, 21), Tuple.Create(33, 23), Tuple.Create(34, 23),
                Tuple.Create(26, 24), Tuple.Create(28, 24), Tuple.Create(30, 24),
                Tuple.Create(33, 24), Tuple.Create(34, 24), Tuple.Create(26, 25),
                Tuple.Create(28, 25), Tuple.Create(32, 25), Tuple.Create(32, 26),
                Tuple.Create(30, 27), Tuple.Create(34, 27), Tuple.Create(34, 28),
                Tuple.Create(32, 29), Tuple.Create(34, 29), Tuple.Create(33, 30),
                Tuple.Create(34, 30), Tuple.Create(33, 31), Tuple.Create(34, 31),
                Tuple.Create(33, 32), Tuple.Create(34, 32), Tuple.Create(34, 33)
            };

            graphe = new Graphe(aretes); // Cr�er le graphe
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DessinerGraphe(e.Graphics); // Dessiner le graphe
            DessinerLegende(e.Graphics); // Dessiner la l�gende

            // Afficher les ordres de d�couverte
            if (noeudsSelectionnes.Count > 0)
            {
                var ordreBFS = graphe.BFS(noeudsSelectionnes[0]);
                var ordreDFS = graphe.DFS(noeudsSelectionnes[0]);
                AfficherOrdresDecouverte(e.Graphics, ordreBFS, ordreDFS);
            }

            // Afficher la connexit� et les circuits
            AfficherConnexite(e.Graphics);
            AfficherCyclicite(e.Graphics);

        }

        private void DessinerGraphe(Graphics g)
        {
            // Dessiner les ar�tes
            foreach (var lien in graphe.Liens)
            {
                var pointDepart = lien.Depart.Position;
                var pointArrivee = lien.Arrivee.Position;

                // V�rifier si l'ar�te fait partie du circuit
                if (circuit.Contains(lien))
                {
                    g.DrawLine(new Pen(Color.Purple, 3), pointDepart, pointArrivee);
                }
                // V�rifier si l'ar�te relie un voisin direct du sommet s�lectionn�
                else if (noeudsSelectionnes.Count > 0 && graphe.EstAdjacent(noeudsSelectionnes[0], lien))
                {
                    g.DrawLine(new Pen(Color.Green, 3), pointDepart, pointArrivee);
                }
                else
                {
                    g.DrawLine(Pens.Black, pointDepart, pointArrivee);
                }
            }

            // Dessiner les n�uds
            foreach (var noeud in graphe.Noeuds)
            {
                int rayon = noeud.Rayon;
                var point = noeud.Position;

                // Changer la couleur du n�ud s'il est s�lectionn�
                if (noeudsSelectionnes.Contains(noeud.Id))
                {
                    g.FillEllipse(Brushes.Red, point.X - rayon, point.Y - rayon, 2 * rayon, 2 * rayon);
                }
                else
                {
                    g.FillEllipse(Brushes.Blue, point.X - rayon, point.Y - rayon, 2 * rayon, 2 * rayon);
                }

                g.DrawString(noeud.Id.ToString(), this.Font, Brushes.White, point.X - 5, point.Y - 5);
            }

            //chemin le plus court entre deux sommets
            if (cheminPlusCourt.Count > 0)
            {
                for (int i = 0; i < cheminPlusCourt.Count - 1; i++)
                {
                    var noeudDepart = graphe.Noeuds.First(n => n.Id == cheminPlusCourt[i]);
                    var noeudArrivee = graphe.Noeuds.First(n => n.Id == cheminPlusCourt[i + 1]);

                    // �paissir progressivement le trait
                    float epaisseur = 3 + (i * 0.5f); // Augmenter l'�paisseur discr�tement
                    g.DrawLine(new Pen(Color.Red, epaisseur), noeudDepart.Position, noeudArrivee.Position);
                }
            }
        }

        private void DessinerLegende(Graphics g)
        {
            // Afficher la l�gende en haut � gauche
            int x = 20;
            int y = 20;
            int hauteurLigne = 20;

            g.DrawString("L�gende :", this.Font, Brushes.Black, x, y);
            y += hauteurLigne;

            g.FillEllipse(Brushes.Blue, x, y, 10, 10);
            g.DrawString("N�ud", this.Font, Brushes.Black, x + 15, y);
            y += hauteurLigne;

            g.FillEllipse(Brushes.Red, x, y, 10, 10);
            g.DrawString("N�ud s�lectionn�", this.Font, Brushes.Black, x + 15, y);
            y += hauteurLigne;

            g.DrawLine(new Pen(Color.Green, 3), x, y + 5, x + 20, y + 5);
            g.DrawString("Arc d'adjacence", this.Font, Brushes.Black, x + 25, y);
            y += hauteurLigne;

            g.DrawLine(new Pen(Color.Red, 3), x, y + 5, x + 20, y + 5);
            g.DrawString("Chemin le plus court", this.Font, Brushes.Black, x + 25, y);
            y += hauteurLigne;

            g.DrawLine(new Pen(Color.Purple, 3), x, y + 5, x + 20, y + 5);
            g.DrawString("Cycle ", this.Font, Brushes.Black, x + 25, y);

            g.DrawString("Noeud 0 : pour d�monstration", this.Font, Brushes.Black, x, y + 20);
        }

        private void AfficherOrdresDecouverte(Graphics g, List<int> ordreBFS, List<int> ordreDFS)
        {
            int x = 20;
            int y = this.ClientSize.Height - 80; // Position en bas de la fen�tre
            g.DrawString($"BFS : {string.Join(", ", ordreBFS)}", this.Font, Brushes.Black, x, y);
            y += 20;
            g.DrawString($"DFS : {string.Join(", ", ordreDFS)}", this.Font, Brushes.Black, x, y);
        }

        private void AfficherConnexite(Graphics g)
        {
            int x = 20;
            int y = this.ClientSize.Height - 40; // axe invers�

            // V�rifier la connexit� du graphe ou des sous-graphe
            bool estConnexe;
            if (noeudsSelectionnes.Count == 0)
            {
                estConnexe = graphe.EstConnexe(graphe.BFS(1)); //0 n'est pas pris en compte
                g.DrawString($"Connexit� du graphe : {(estConnexe ? "Oui" : "Non")}", this.Font, Brushes.Black, x, y);
            }
            else
            { //fonctionnalit� innutile pour l'instant mais je souhaite l'utiliser pour demontrer la forte connexit�
                estConnexe = graphe.EstConnexe(noeudsSelectionnes);
                g.DrawString($"Connexit� du sous-graphe [{string.Join(", ", noeudsSelectionnes)}] : {(estConnexe ? "Oui" : "Non")}", this.Font, Brushes.Black, x, y);
            }

            y += 20;

        }

        private void AfficherCyclicite(Graphics g)
        {
            int x = 20;
            int y = this.ClientSize.Height - 20; // Position en bas de la fen�tre

            // V�rifier la connexit� du graphe ou des sous-graphe
            bool estCyclique;
            if (noeudsSelectionnes.Count == 0)
            {
                estCyclique = graphe.EstCyclique(graphe.DFS(1));
                g.DrawString($"Cylicit�e du graphe : {(estCyclique ? "Oui" : "Non")}", this.Font, Brushes.Black, x, y);
            }
            else
            {
                estCyclique = graphe.EstCyclique(noeudsSelectionnes);
                g.DrawString($"Cyclicit�e du sous-graphe [{string.Join(", ", noeudsSelectionnes)}] : {(estCyclique ? "Oui" : "Non")}", this.Font, Brushes.Black, x, y);
            }

            y += 20;

        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            // V�rifier si l'utilisateur a cliqu� sur un n�ud
            foreach (var noeud in graphe.Noeuds)
            {
                int rayon = noeud.Rayon;
                var point = noeud.Position;

                // V�rifier si le clic est � l'int�rieur du cercle du n�ud
                if (Math.Pow(e.X - point.X, 2) + Math.Pow(e.Y - point.Y, 2) <= Math.Pow(rayon, 2))
                {
                    // Si le n�ud est d�j� s�lectionn�, le d�s�lectionner
                    if (noeudsSelectionnes.Contains(noeud.Id))
                    {
                        noeudsSelectionnes.Remove(noeud.Id);
                    }
                    // Sinon, s�lectionner le n�ud
                    else
                    {
                        noeudsSelectionnes.Add(noeud.Id);
                    }

                    // Si au moins deux n�uds sont s�lectionn�s, calculer le chemin le plus court
                    if (noeudsSelectionnes.Count >= 2)
                    {
                        cheminPlusCourt = graphe.CheminPlusCourt(noeudsSelectionnes);
                    }
                    else
                    {
                        cheminPlusCourt.Clear();
                    }

                    // Redessiner le formulaire pour afficher les changements
                    this.Invalidate();
                    break;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Optionnel : Forcer le redessin du formulaire au chargement
            this.Invalidate();
        }
    }
}