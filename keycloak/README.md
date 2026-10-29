# Keycloak

`realm-gestion.json` est importé automatiquement au premier démarrage (`start-dev --import-realm`).

- Console d'admin : http://localhost:8180 (admin / admin)
- Realm `gestion`, client public `gestion-client` (code + PKCE)
- Rôles realm `Admin` / `Gestionnaire`, exposés dans le claim `role` des tokens (mapper du client)
- Utilisateurs (mot de passe `password`) : `user1` (Admin), `user2` et `user3` (Gestionnaire)
