using System.Text.Json;
using System.Text.Json.Serialization;

namespace pixelConquest;

// Persistance des profils de stratégie dans un unique fichier catalogue JSON.
public class StrategyCatalog
{
    private readonly string _path;

    // Options de (dé)sérialisation partagées : JSON indenté + enums en texte.
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public List<StrategyProfile> Profiles { get; private set; } = new();

    public StrategyCatalog(string path)
    {
        _path = path;
    }

    // Charge les profils depuis le fichier. Si absent, amorce avec des profils
    // de démo variés (et les sauvegarde) pour ne jamais démarrer à vide.
    public void Load()
    {
        if (!File.Exists(_path))
        {
            Profiles = DefaultProfiles();
            Save();
            return;
        }

        string json = File.ReadAllText(_path);
        List<StrategyProfile>? loaded =
            JsonSerializer.Deserialize<List<StrategyProfile>>(json, JsonOptions);

        // Clamp défensif : le fichier a pu être édité à la main hors bornes.
        Profiles = loaded?.Select(p => p.Clamped()).ToList() ?? new();
    }

    // Profils de démonstration livrés au premier lancement.
    private static List<StrategyProfile> DefaultProfiles() => new()
    {
        new StrategyProfile { Name = "Rusher", Type = StrategyType.Bfs,
            Color = "#e63946", Randomness = 0.05, Compactness = 0.1 },
        new StrategyProfile { Name = "Turtle", Type = StrategyType.Defensive,
            Color = "#457b9d", Compactness = 0.95 },
        new StrategyProfile { Name = "Predator", Type = StrategyType.Aggressive,
            Color = "#2a9d8f", Aggressiveness = 0.8, Randomness = 0.1 },
        new StrategyProfile { Name = "Chaos", Type = StrategyType.Random,
            Color = "#f4a261", Randomness = 1.0 },
    };

    // Écrit tous les profils dans le fichier.
    public void Save()
    {
        string json = JsonSerializer.Serialize(Profiles, JsonOptions);
        File.WriteAllText(_path, json);
    }

    // Ajoute ou remplace un profil (unicité par nom, insensible à la casse).
    public void AddOrUpdate(StrategyProfile profile)
    {
        profile = profile.Clamped();
        int index = Profiles.FindIndex(
            p => string.Equals(p.Name, profile.Name, StringComparison.OrdinalIgnoreCase));

        if (index >= 0)
        {
            Profiles[index] = profile;
        }
        else
        {
            Profiles.Add(profile);
        }
    }

    public bool Remove(string name) =>
        Profiles.RemoveAll(
            p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase)) > 0;

    public StrategyProfile? FindByName(string name) =>
        Profiles.FirstOrDefault(
            p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
}
