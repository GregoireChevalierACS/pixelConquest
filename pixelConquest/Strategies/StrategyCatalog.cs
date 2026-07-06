using System.Text.Json;
using System.Text.Json.Serialization;

namespace pixelConquest;

// Persistance des profils de stratégie dans un unique fichier catalogue JSON.
class StrategyCatalog
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

    // Charge les profils depuis le fichier. Si absent, laisse une liste vide.
    public void Load()
    {
        if (!File.Exists(_path))
        {
            Profiles = new();
            return;
        }

        string json = File.ReadAllText(_path);
        List<StrategyProfile>? loaded =
            JsonSerializer.Deserialize<List<StrategyProfile>>(json, JsonOptions);

        // Clamp défensif : le fichier a pu être édité à la main hors bornes.
        Profiles = loaded?.Select(p => p.Clamped()).ToList() ?? new();
    }

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
