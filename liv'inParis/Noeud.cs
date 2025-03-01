using System.Drawing;

namespace liv_inParis
{
    public class Noeud
    {
        public int Id { get; } // Identifiant unique du nœud
        public Point Position { get; set; } // Position du nœud sur le graphique
        public int Rayon { get; set; } // Rayon du nœud (pour l'affichage)

        public Noeud(int id, Point position, int rayon)
        {
            Id = id;
            Position = position;
            Rayon = rayon;
        }
    }
}