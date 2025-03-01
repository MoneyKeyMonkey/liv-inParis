namespace liv_inParis
{
    public class Lien
    {
        public Noeud Depart { get; } // Nœud de départ
        public Noeud Arrivee { get; } // Nœud d'arrivée

        public Lien(Noeud depart, Noeud arrivee)
        {
            Depart = depart;
            Arrivee = arrivee;
        }
    }
}

