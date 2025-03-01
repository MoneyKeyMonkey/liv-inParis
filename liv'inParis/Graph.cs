using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace liv_inParis
{
    public class Graphe
    {
        public List<Noeud> Noeuds { get; private set; } // Liste des nœuds
        public List<Lien> Liens { get; private set; } // Liste des arêtes

        // Représentation en liste d'adjacence
        private Dictionary<int, List<int>> listeAdjacence;

        public Graphe(List<Tuple<int, int>> aretes)
        {
            Noeuds = new List<Noeud>();
            Liens = new List<Lien>();
            InitialiserGraphe(aretes);
        }
        private void InitialiserGraphe(List<Tuple<int, int>> aretes)
        {
            if (aretes == null || aretes.Count == 0)
            {
                // Si la liste d'arêtes est vide, initialiser un graphe vide
                listeAdjacence = new Dictionary<int, List<int>>();
                return;
            }

            // Trouver le nœud maximum
            int maxNoeud = aretes.Max(arete => Math.Max(arete.Item1, arete.Item2));

            // Créer les nœuds avec des positions aléatoires sans chevauchement
            Random rand = new Random();
            for (int i = 0; i <= maxNoeud; i++)
            {
                Point position;
                bool chevauchement;
                int tentatives = 0;

                do
                {
                    chevauchement = false;
                    position = new Point(rand.Next(200, 900), rand.Next(50, 650));

                    // Vérifier si la nouvelle position est trop proche d'un nœud existant
                    foreach (var noeud in Noeuds)
                    {
                        if (Math.Pow(position.X - noeud.Position.X, 2) + Math.Pow(position.Y - noeud.Position.Y, 2) < 2500) // Distance minimale de 50 pixels
                        {
                            chevauchement = true;
                            break;
                        }
                    }

                    tentatives++;
                } while (chevauchement && tentatives < 100); // Limiter le nombre de tentatives

                Noeuds.Add(new Noeud(i, position, 10)); // Ajouter le nœud avec sa position
            }

            // Créer les arêtes
            foreach (var arete in aretes)
            {
                var noeudDepart = Noeuds.FirstOrDefault(n => n.Id == arete.Item1);
                var noeudArrivee = Noeuds.FirstOrDefault(n => n.Id == arete.Item2);
                if (noeudDepart != null && noeudArrivee != null)
                {
                    Liens.Add(new Lien(noeudDepart, noeudArrivee));
                }
            }

            // Initialiser la liste d'adjacence
            listeAdjacence = new Dictionary<int, List<int>>();
            foreach (var noeud in Noeuds)
            {
                listeAdjacence[noeud.Id] = new List<int>();
            }
            foreach (var lien in Liens)
            {
                listeAdjacence[lien.Depart.Id].Add(lien.Arrivee.Id);
                listeAdjacence[lien.Arrivee.Id].Add(lien.Depart.Id); // Pour un graphe non orienté
            }

            // Ajuster le rayon des nœuds en fonction de leur degré
            foreach (var noeud in Noeuds)
            {
                int degre = listeAdjacence[noeud.Id].Count;
                noeud.Rayon = 10 + degre * 2; // Rayon de base + 2 pixels par degré
            }
        }

        /// <summary>
        /// /sera réutiliser pour démontrer les composantes fortemement connexe 
        /// </summary>
        /// <param name="noeuds"></param>
        /// <returns></returns>
        public bool EstConnexe(List<int> noeuds)
        {
            if (noeuds == null || noeuds.Count == 0)
                return true; // Un ensemble vide est considéré comme connexe.

            List<int> visites = DFS(noeuds[0]);

            // Vérifie que tous les noeuds sont atteints
            return noeuds.All(n => visites.Contains(n));
        }

        public bool EstCyclique(List<int> noeuds)
        {
            if (noeuds == null || noeuds.Count < 3)
                return false; // Il doit y avoir au moins 3 noeuds pour former un cycle 

            // Vérifie chaque paire de noeuds pour voir s'il existe un lien entre eux
            for (int i = 0; i < noeuds.Count - 1; i++)
            {
                // Vérifie si ces deux noeuds sont adjacents
                    if (EstAdjacent2(noeuds[i], noeuds[i + 1]))
                        return false; // Si un lien est manquant, retourne false
            }
            return false;
        }
            public bool EstAdjacent2(int idNoeud1, int idNoeud2)
        {
            // Vérifie si le nœud idNoeud1 a des voisins et si idNoeud2 est dans la liste des voisins
            if (listeAdjacence.ContainsKey(idNoeud1))
            {
                return listeAdjacence[idNoeud1].Contains(idNoeud2);
            }
            return false;
        }

        public bool EstAdjacent(int idNoeud, Lien lien)
        {
            return (lien.Depart.Id == idNoeud && listeAdjacence[idNoeud].Contains(lien.Arrivee.Id)) ||
                   (lien.Arrivee.Id == idNoeud && listeAdjacence[idNoeud].Contains(lien.Depart.Id));
        }

        // Parcours en largeur (BFS)
        public List<int> BFS(int idNoeudDepart)
        {
            var visites = new List<int>();
            var file = new Queue<int>();
            file.Enqueue(idNoeudDepart);

            while (file.Count > 0)
            {
                int idNoeudCourant = file.Dequeue();
                if (!visites.Contains(idNoeudCourant))
                {
                    visites.Add(idNoeudCourant);
                    foreach (var voisin in listeAdjacence[idNoeudCourant])
                    {
                        if (!visites.Contains(voisin))
                        {
                            file.Enqueue(voisin);
                        }
                    }
                }
            }

            return visites;
        }

        // Parcours en profondeur (DFS)
        public List<int> DFS(int idNoeudDepart)
        {
            var visites = new List<int>();
            DFSRecursif(idNoeudDepart, visites);
            return visites;
        }
            private void DFSRecursif(int idNoeudCourant, List<int> visites)
        {
            if (!visites.Contains(idNoeudCourant))
            {
                visites.Add(idNoeudCourant);
                foreach (var voisin in listeAdjacence[idNoeudCourant])
                {
                    DFSRecursif(voisin, visites);
                }
            }
        }

    
        // Trouver le chemin le plus court entre deux nœuds (BFS)
        public List<int> CheminPlusCourt(List<int> noeuds)
        {
            if (noeuds.Count < 2)
                return new List<int>();

            int depart = noeuds[0];
            int arrivee = noeuds[1];

            var predecesseurs = new Dictionary<int, int>();
            var file = new Queue<int>();
            file.Enqueue(depart);
            predecesseurs[depart] = -1;

            while (file.Count > 0)
            {
                int noeudCourant = file.Dequeue();

                if (noeudCourant == arrivee)
                {
                    // Reconstruire le chemin
                    var chemin = new List<int>();
                    while (noeudCourant != -1)
                    {
                        chemin.Add(noeudCourant);
                        noeudCourant = predecesseurs[noeudCourant];
                    }
                    chemin.Reverse();
                    return chemin;
                }

                foreach (var voisin in listeAdjacence[noeudCourant])
                {
                    if (!predecesseurs.ContainsKey(voisin))
                    {
                        predecesseurs[voisin] = noeudCourant;
                        file.Enqueue(voisin);
                    }
                }
            }

            return new List<int>(); // Aucun chemin trouvé
        }
    }
}