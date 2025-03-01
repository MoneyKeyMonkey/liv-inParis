DROP DATABASE IF EXISTS livinparis;
CREATE DATABASE livinparis;
USE livinparis;

CREATE TABLE utilisateur(
    id CHAR(36) PRIMARY KEY,
    prenom VARCHAR(50) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    num_tel VARCHAR(15),
    adresse VARCHAR(100),
    entreprise VARCHAR(100),
    mdp VARCHAR(255) NOT NULL, -- Mot de passe haché
    nom VARCHAR(50) NOT NULL
);

CREATE TABLE client(
    id_client CHAR(36) PRIMARY KEY,
    id CHAR(36) NOT NULL UNIQUE,
    FOREIGN KEY(id) REFERENCES utilisateur(id) ON DELETE CASCADE
);

CREATE TABLE cuisinier(
    id_cuisinier CHAR(36) PRIMARY KEY,
    id CHAR(36) NOT NULL UNIQUE,
    FOREIGN KEY(id) REFERENCES utilisateur(id) ON DELETE CASCADE
);

CREATE TABLE commande(
    id_commande CHAR(36) PRIMARY KEY,
    date_commande DATETIME NOT NULL,
    id_cuisinier CHAR(36) NOT NULL,
    id_client CHAR(36) NOT NULL,
    FOREIGN KEY(id_cuisinier) REFERENCES cuisinier(id_cuisinier) ON DELETE CASCADE,
    FOREIGN KEY(id_client) REFERENCES client(id_client) ON DELETE CASCADE
);

CREATE TABLE avis(
    id_avis CHAR(36) PRIMARY KEY,
    retour_cuisinier TEXT,
    retour_client TEXT,
    date_avis DATETIME NOT NULL,
    id_commande CHAR(36) NOT NULL,
    FOREIGN KEY(id_commande) REFERENCES commande(id_commande) ON DELETE CASCADE
);

CREATE TABLE livraison(
    id_livraison CHAR(36) PRIMARY KEY,
    date_livraison DATETIME NOT NULL
);

CREATE TABLE plat(
    id_plat CHAR(36) PRIMARY KEY,
    nom VARCHAR(100) NOT NULL,
    prix DECIMAL(10, 2) NOT NULL,
    origine_region VARCHAR(100),
    photo_du_plat VARCHAR(255)
);

CREATE TABLE ingredient(
    id_ingredient CHAR(36) PRIMARY KEY,
    nom_ingredient VARCHAR(100) NOT NULL,
    provenance VARCHAR(100),
    type VARCHAR(50)
);

CREATE TABLE ligne_de_commande(
    id_ligne_de_commande CHAR(36) PRIMARY KEY,
    id_commande CHAR(36) NOT NULL,
    id_client CHAR(36) NOT NULL,
    id_livraison CHAR(36) NOT NULL,
    FOREIGN KEY(id_commande) REFERENCES commande(id_commande) ON DELETE CASCADE,
    FOREIGN KEY(id_client) REFERENCES client(id_client) ON DELETE CASCADE,
    FOREIGN KEY(id_livraison) REFERENCES livraison(id_livraison) ON DELETE CASCADE
);

CREATE TABLE ligne_commande_plat(
    id_ligne_de_commande CHAR(36),
    id_plat CHAR(36),
    PRIMARY KEY(id_ligne_de_commande, id_plat),
    FOREIGN KEY(id_ligne_de_commande) REFERENCES ligne_de_commande(id_ligne_de_commande) ON DELETE CASCADE,
    FOREIGN KEY(id_plat) REFERENCES plat(id_plat) ON DELETE CASCADE
);

CREATE TABLE plat_ingredient(
    id_plat CHAR(36),
    id_ingredient CHAR(36),
    quantite INT NOT NULL,
    PRIMARY KEY(id_plat, id_ingredient),
    FOREIGN KEY(id_plat) REFERENCES plat(id_plat) ON DELETE CASCADE,
    FOREIGN KEY(id_ingredient) REFERENCES ingredient(id_ingredient) ON DELETE CASCADE
);