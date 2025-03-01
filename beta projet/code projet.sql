#pour tout relancer mais att suppression de toute la data.
DROP DATABASE IF EXISTS projetInfo;
CREATE DATABASE projetInfo;
USE projetInfo;

#table utilisateur	 clients, cuisiniers et livreurs et administrateurs a faire plus tard
CREATE TABLE utilisateur (
    Id_utilisateur INT AUTO_INCREMENT PRIMARY KEY,
    nom VARCHAR(50) NOT NULL,
    prenom VARCHAR(50),
    email VARCHAR(100) UNIQUE NOT NULL,
    cb VARCHAR(19) UNIQUE,
    telephone VARCHAR(15) UNIQUE,
    inscription TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    score DECIMAL(4,2) DEFAULT 0 CHECK (score >= 0 AND score <= 5),
    ratio DECIMAL(5,2) CHECK (ratio >= 0 AND ratio <= 1),
    profil VARCHAR(50),
    role ENUM('client', 'cuisinier') NOT NULL,  
    mdp CHAR(15) NOT NULL, 
    adresse VARCHAR(100)
);

#table entreprise a 
CREATE TABLE entreprise (
    Id_entreprise INT AUTO_INCREMENT PRIMARY KEY,
    nom VARCHAR(100) NOT NULL,
    contact VARCHAR(100),
    adresse VARCHAR(100) NOT NULL,
    telephone VARCHAR(15) UNIQUE,
    Id_utilisateur INT UNIQUE NOT NULL,
    FOREIGN KEY (Id_utilisateur) REFERENCES utilisateur(Id_utilisateur) ON DELETE CASCADE
);

-- Table plat : ajout de catégories et restrictions
CREATE TABLE plat (
    Id_plat INT AUTO_INCREMENT PRIMARY KEY,
    Id_cuisinier INT NOT NULL,  
    titre VARCHAR(100) NOT NULL,
    description TEXT,
    prix DECIMAL(10,2) CHECK (prix >= 0),
    nombre INT CHECK (nombre >= 0),
    type ENUM('entrée', 'plat principal', 'dessert') NOT NULL,
    nationalite VARCHAR(50),
    regime_alimentaire VARCHAR(100), 
    date_preparation DATE NOT NULL,
    date_peremption DATE NOT NULL,
    photo VARCHAR(255),
    FOREIGN KEY (Id_cuisinier) REFERENCES utilisateur(Id_utilisateur) ON DELETE CASCADE
);

-- Table commandes : gestion des achats
CREATE TABLE commande (
    Id_commande INT AUTO_INCREMENT PRIMARY KEY,
    Id_client INT NOT NULL, 
    facture DECIMAL(10,2) CHECK (facture >= 0),
    date_commande TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (Id_client) REFERENCES utilisateur(Id_utilisateur) ON DELETE CASCADE
);

-- Table ingredient : gestion des ingrédients
CREATE TABLE ingredient (
    Id_ingredient INT AUTO_INCREMENT PRIMARY KEY,
    nom VARCHAR(50) UNIQUE NOT NULL
);

-- Table composition des plats avec les ingrédients
CREATE TABLE compose (
    Id_plat INT,
    Id_ingredient INT,
    quantite DECIMAL(6,2) CHECK (quantite > 0),
    unite VARCHAR(20),  
    PRIMARY KEY(Id_plat, Id_ingredient),
    FOREIGN KEY(Id_plat) REFERENCES plat(Id_plat) ON DELETE CASCADE,
    FOREIGN KEY(Id_ingredient) REFERENCES ingredient(Id_ingredient) ON DELETE CASCADE
);

-- Table commande_plat : relation entre commande et plat
CREATE TABLE commande_plat (
    Id_commande INT,
    Id_plat INT,
    quantite INT CHECK (quantite > 0), 
    PRIMARY KEY(Id_commande, Id_plat),
    FOREIGN KEY(Id_commande) REFERENCES commande(Id_commande) ON DELETE CASCADE,
    FOREIGN KEY(Id_plat) REFERENCES plat(Id_plat) ON DELETE CASCADE
);

-- Table livraison : gestion des livreurs et livraisons
CREATE TABLE livraison (
    Id_livraison INT AUTO_INCREMENT PRIMARY KEY,
    Id_livreur INT NOT NULL,
    Id_commande INT NOT NULL,
    adresse_livraison VARCHAR(100) NOT NULL,
    statut ENUM('en attente', 'en cours', 'livré') DEFAULT 'en attente',
    date_livraison TIMESTAMP NULL,
    FOREIGN KEY(Id_livreur) REFERENCES utilisateur(Id_utilisateur) ON DELETE CASCADE,
    FOREIGN KEY(Id_commande) REFERENCES commande(Id_commande) ON DELETE CASCADE
);

-- Table avis : notation et commentaire des utilisateurs
CREATE TABLE avis (
    Id_avis INT AUTO_INCREMENT PRIMARY KEY,
    Id_utilisateur INT NOT NULL,  -- Celui qui laisse l'avis (client)
    Id_cuisinier INT,  -- Le cuisinier évalué
    Id_livreur INT,  -- Le livreur évalué (optionnel)
    Id_commande INT NOT NULL,
    score TINYINT CHECK (score BETWEEN 1 AND 5), 
    commentaire TEXT,
    date_avis TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    signalement BOOLEAN DEFAULT FALSE,
    FOREIGN KEY (Id_utilisateur) REFERENCES utilisateur(Id_utilisateur) ON DELETE CASCADE,
    FOREIGN KEY (Id_cuisinier) REFERENCES utilisateur(Id_utilisateur) ON DELETE CASCADE,
    FOREIGN KEY (Id_livreur) REFERENCES utilisateur(Id_utilisateur) ON DELETE CASCADE,
    FOREIGN KEY (Id_commande) REFERENCES commande(Id_commande) ON DELETE CASCADE
);

-- **Optimisation des performances**
CREATE INDEX idx_utilisateur_email ON utilisateur(email);
CREATE INDEX idx_utilisateur_role ON utilisateur(role);
CREATE INDEX idx_avis_cuisinier ON avis(Id_cuisinier);
CREATE INDEX idx_avis_livreur ON avis(Id_livreur);
CREATE INDEX idx_plat_categorie ON plat(type);
CREATE INDEX idx_livraison_statut ON livraison(statut);

-- **Vue pour calculer automatiquement le score moyen des cuisiniers et livreurs**
CREATE VIEW score_utilisateur AS
SELECT 
    u.Id_utilisateur, u.nom, u.prenom, 
    COALESCE(AVG(a.score), 0) AS score_moyen
FROM utilisateur u
LEFT JOIN avis a ON u.Id_utilisateur = a.Id_cuisinier OR u.Id_utilisateur = a.Id_livreur
GROUP BY u.Id_utilisateur;

SELECT * FROM utilisateur;