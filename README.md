# Wishmon - Projet Unity

**Moteur :** Unity 6000.4.3f1  
**Rendu :** Universal Render Pipeline (URP)  
**Input :** Unity Input System

---

## C'est quoi ce projet

Jeu mobile top-down 3D inspiré de Pokemon, développé en cours.  
Le perso se déplace dans un monde ouvert, un Wishemon le suit, et des combats se déclenchent dans les hautes herbes.

---

## Structure du projet

```
wishmon/
├── Assets/
│   ├── Art/           # Modèles 3D, matériaux, animations
│   ├── Inputs/        # InputSystem actions
│   ├── Prefabs/       # Prefabs Unity
│   ├── Scenes/        # Scènes (World, Menu...)
│   ├── Scriptable/    # WishemonCards (ScriptableObjects)
│   ├── Scripts/       # Tout le code C#
│   └── Settings/      # Config URP
├── Packages/          # Dépendances Unity
└── ProjectSettings/   # Config du projet
```

---

## Scripts

| Script              | Rôle                                                     |
| ------------------- | -------------------------------------------------------- |
| `Player.cs`         | Déplacement, interaction, spawn du wishemon starter      |
| `PlayerFollower.cs` | Caméra qui suit le joueur                                |
| `Wishemon.cs`       | Instancie le modèle 3D du wishemon dans la scène         |
| `WishemonCard.cs`   | ScriptableObject : données d'un wishemon (stats, prefab) |
| `TallGrass.cs`      | Zone de rencontre aléatoire                              |
| `FightManager.cs`   | Singleton qui gère le déclenchement des combats          |
| `PauseManager.cs`   | Pause / Reprise du jeu                                   |
| `MenuManager.cs`    | Navigation entre scènes (menu principal)                 |

---

## Comment ouvrir

1. Unity Hub > **Open** > sélectionner ce dossier
2. Unity version : **6000.4.3f1** (LTS)
3. Ouvrir la scène `Assets/Scenes/World.unity`
4. Play !

**Contrôles :**

- `WASD` / Joystick → déplacement
- `E` / Bouton Sud → interaction
- `Escape` / Start → pause

---

## État actuel (J+2)

- [x] Déplacement joueur + animations
- [x] Caméra qui suit
- [x] Wishemon starter qui suit le joueur
- [x] Encounters aléatoires dans les hautes herbes
- [x] Système pause
- [ ] Scène de combat (TODO)
- [ ] UI mobile (joystick virtuel)
- [ ] Build Android/iOS
